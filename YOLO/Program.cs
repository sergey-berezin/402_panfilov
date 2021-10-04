using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace YOLO
{
    class Program
    {
        const string inputPath = @"..\..\..\Assets\InputImages";

        static async Task Main()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Detector.ExecuteAsync(inputPath, tokenSource.Token);

            //tokenSource.CancelAfter(2000);

            await PrintProgressAsync(Detector.Progress);

            PrintResult(Detector.Output);
        }

        static async Task PrintProgressAsync(ISourceBlock<float> source)
        {
            while (await source.OutputAvailableAsync())
            {
                var data = source.Receive();

                Console.WriteLine($"{(100f * data):F1}% of images were processed.");
            }
            //Console.WriteLine("End of print progress async");
        }

        static void PrintResult(List<IReadOnlyList<YoloResult>> source)
        {
            Dictionary<string, int> foundClass = new();

            foreach (var imageData in source)
            {
                foreach (var item in imageData)
                {
                    if (!foundClass.ContainsKey(item.Label))
                        foundClass.Add(item.Label, 1);
                    else
                        foundClass[item.Label]++;
                }
            }

            Console.WriteLine("All found classes:");
            foreach (var item in foundClass)
            {
                Console.WriteLine($"      {item.Value} {item.Key}");
            }
        }
    }
}
