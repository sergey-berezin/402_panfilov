using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataBase;
using System.Threading.Tasks.Dataflow;
using YOLO;
using System.Threading;

namespace WebServer
{
    [ApiController]
    [Route("api/content")]
    public class ContentController : ControllerBase
    {
        static bool processing = false;
        static float progress;
        static string inputPath = "";
        CancellationTokenSource tokenSource = new();
        BufferBlock<IReadOnlyList<YoloResult>> output = new();

        ILibraryDB db;


        public ContentController(ILibraryDB _db)
        {
            db = _db;
        }


        [HttpPost("start_detection")]
        public void StartDetection([FromBody] string _inputPath)
        {
            if (processing) return;

            inputPath = _inputPath;
            Console.WriteLine("StartDetection inputPath = " + inputPath);
            processing = true;

            _ = DetectObjectsAsync();
        }

        [HttpDelete("clear_database")]
        public void ClearDatabase()
        {
            db.Clear();
        }

        [HttpGet("progress")]
        public string GetProgress()
        {
            return $"{(progress * 100f):F1}%";
        }

        [HttpGet("processing")]
        public string GetProcess()
        {
            return processing ? "true" : "false";
        }

        [HttpPost("stop_recognition")]
        public void StopRecognition()
        {
            tokenSource.Cancel();
        }

        [HttpGet("class_list")]
        public List<string> GetClassList()
        {
            return db.GetClassList().ToList();
        }

        [HttpGet("image_byte_list/{label}")]
        public List<byte[]> GetImageList(string label)
        {
            return db.GetImageList(label).ToList();
        }


        async Task DetectObjectsAsync()
        {
            _ = Detector.ExecuteAsync(inputPath, tokenSource.Token, output);
            try
            {
                await UpdateDatabaseAsync(output);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }

            processing = false;
        }

        async Task UpdateDatabaseAsync(ISourceBlock<IReadOnlyList<YoloResult>> source)
        {
            int imageAmount = Detector.GetImageFilenames(inputPath).Length;
            int i = 0;

            Console.WriteLine("UpdateDatabaseAsync 01");
            progress = 0f;
            while (await source.OutputAvailableAsync())
            {
                var data = source.Receive();
                i++;

                Console.WriteLine("UpdateDatabaseAsync 02: progress = " + progress);

                foreach (var item in data)
                {
                    await db.AddAsync(new YoloItem(item));
                }

                Console.WriteLine("UpdateDatabaseAsync 03: progress = " + progress);

                progress = (float)i / imageAmount;

                Console.WriteLine("UpdateDatabaseAsync 04: progress = " + progress);
            }
        }
    }
}