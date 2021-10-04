using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using System.Drawing;
using System.Threading.Tasks.Dataflow;
using System.Threading;

namespace YOLO
{
    public static class Detector
    {
        static readonly string[] classesNames = new string[] { "person", "bicycle", "car", "motorbike", "aeroplane", "bus", "train", "truck", "boat", "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "sofa", "pottedplant", "bed", "diningtable", "toilet", "tvmonitor", "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };
        static readonly string modelPath = @"D:\And\Prog\onnx\models-master\vision\object_detection_segmentation\yolov4\model\yolov4.onnx";
        static BufferBlock<IReadOnlyList<YoloResult>> outputBuffer = new();
        static BufferBlock<float> progress = new();
        public static List<IReadOnlyList<YoloResult>> output = new();

        public static int ImageAmount;
        public static BufferBlock<float> Progress => progress;
        public static List<IReadOnlyList<YoloResult>> Output => output;


        public static async void ExecuteAsync(string directoryPath, CancellationToken token)
        {
            MLContext mlContext = new MLContext();

            var pipeline = mlContext.Transforms.ResizeImages(inputColumnName: "bitmap", outputColumnName: "input_1:0", imageWidth: 416, imageHeight: 416, resizing: ResizingKind.IsoPad)
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input_1:0", scaleImage: 1f / 255f, interleavePixelColors: true))
                .Append(mlContext.Transforms.ApplyOnnxModel(
                    shapeDictionary: new Dictionary<string, int[]>()
                        {
                            { "input_1:0", new[] { 1, 416, 416, 3 } },
                            { "Identity:0", new[] { 1, 52, 52, 3, 85 } },
                            { "Identity_1:0", new[] { 1, 26, 26, 3, 85 } },
                            { "Identity_2:0", new[] { 1, 13, 13, 3, 85 } },
                        },
                    inputColumnNames: new[]
                        {
                            "input_1:0"
                        },
                    outputColumnNames: new[]
                        {
                            "Identity:0",
                            "Identity_1:0",
                            "Identity_2:0"
                        },
                    modelFile: modelPath, recursionLimit: 100));

            var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<YoloBitmapData>()));

            string[] filenames = Directory.GetFiles(directoryPath, "*.jpg");
            ImageAmount = filenames.Length;
            Console.WriteLine(ImageAmount + " images were found.");

            BufferBlock<int> processingImageBuffer = new();
            Parallel.For(0, filenames.Length, i => processingImageBuffer.Post(i));
            processingImageBuffer.Complete();

            ActionBlock<int> processingImageActionBlock = new ActionBlock<int>(
                async x =>
                {
                    //Console.WriteLine("ActionBlock Start");
                    if (token.IsCancellationRequested) return;
                    int filenameIndex = await processingImageBuffer.ReceiveAsync();
                    await Task.Run(() =>
                    {
                        //Console.WriteLine("Processing...");
                        using (var bitmap = new Bitmap(Image.FromFile(filenames[filenameIndex])))
                        {
                            if (token.IsCancellationRequested) return;
                            var predictionEngine = mlContext.Model.CreatePredictionEngine<YoloBitmapData, YoloPrediction>(model);
                            var predict = predictionEngine.Predict(new YoloBitmapData() { Image = bitmap });
                            if (token.IsCancellationRequested) return;
                            var results = predict.GetResults(classesNames, 0.3f, 0.7f);
                            outputBuffer.Post(results);
                            progress.Post((float)outputBuffer.Count / ImageAmount);
                        }
                    });
                    //Console.WriteLine("ActionBlock End");
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 8
                }
            );

            Parallel.For(0, filenames.Length, i => processingImageActionBlock.Post(i));
            processingImageActionBlock.Complete();
            await processingImageActionBlock.Completion;

            progress.Complete();
            outputBuffer.Complete();

            output.Clear();
            while (outputBuffer.Count > 0)
            {
                output.Add(outputBuffer.Receive());
            }
        }
    }
}
