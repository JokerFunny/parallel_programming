using BenchmarkDotNet.Attributes;
using Common;
using System.Collections.Concurrent;

namespace Lab1
{
    [MemoryDiagnoser]
    //[Config(typeof(FastAndDirtyConfig))]
    public class Lab1Benchmark
    {
        public static string[] FileContent = FileReader.GetFileContent("..\\..\\..\\..\\Common\\test.txt");

        [Benchmark(Baseline = true)]
        public void ClassicExecutor()
        {
            Dictionary<int, int[]> resultOfCalculations = new Dictionary<int, int[]>();

            for (int i = 0; i < FileContent.Length; i++)
                resultOfCalculations.Add(i, Worker.DoStuff(FileContent[i]));
        }

        [Benchmark]
        [Arguments(2)]
        [Arguments(4)]
        [Arguments(8)]
        [Arguments(10)]
        [Arguments(12)]
        [Arguments(16)]
        [Arguments(24)]
        [Arguments(32)]
        [Arguments(64)]
        [Arguments(128)]
        public void ParallelExecutor(int maxThreadsCount)
        {
            ThreadPool.SetMinThreads(maxThreadsCount, maxThreadsCount);
            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);
            //ConcurrentDictionary<int, byte> threadIds = new ConcurrentDictionary<int, byte>();
            ConcurrentDictionary<int, int[]> resultOfCalculations = new();

            var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreadsCount };
            Parallel.For(0, FileContent.Length, options, x =>
            {
                //threadIds.TryAdd(Environment.CurrentManagedThreadId, 1);

                resultOfCalculations.TryAdd(x, Worker.DoStuff(FileContent[x]));
            });

            //Console.WriteLine(threadIds.Count);
        }
    }
}
