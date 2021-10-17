using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using YOLO;

namespace ConsoleApp
{
    class Program
    {
        const string inputPath = @"..\..\..\Assets\InputImages";

        static async Task Main()
        {
            var foundClass = new Dictionary<string, int>();
            var tokenSource = new CancellationTokenSource();
            var output = new BufferBlock<IReadOnlyList<YoloResult>>();

            var t = Detector.ExecuteAsync(inputPath, tokenSource.Token, output);

            //tokenSource.CancelAfter(2000);

            await DetectionProcessingAsync(output, foundClass);

            PrintResult(foundClass);
        }

        static async Task DetectionProcessingAsync(ISourceBlock<IReadOnlyList<YoloResult>> source, Dictionary<string, int> dict)
        {
            int imageAmount = Detector.GetImageFilenames(inputPath).Length;
            int i = 0;

            Console.WriteLine(imageAmount + " images were found.");
            while (await source.OutputAvailableAsync())
            {
                var data = source.Receive();
                i++;

                foreach (var item in data)
                {
                    if (!dict.ContainsKey(item.Label))
                        dict.Add(item.Label, 1);
                    else
                        dict[item.Label]++;
                }

                Console.WriteLine($"{(100f * i / imageAmount):F1}% of images were processed.");
            }
            //Console.WriteLine("End of print progress async");
        }

        static void PrintResult(Dictionary<string, int> dict)
        {
            Console.WriteLine("All found classes:");
            foreach (var item in dict)
            {
                Console.WriteLine($"      {item.Value} {item.Key}");
            }
        }
    }
}
