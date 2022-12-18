using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

namespace Lab1
{
    [MemoryDiagnoser]
    //[Config(typeof(FastAndDirtyConfig))]
    public class Lab1Benchmark
    {
        public static string[] FileContent = FileReader.GetFileContent("test.txt");

        [Benchmark(Baseline = true)]
        public void ClassicExecutor()
        {
            Dictionary<int, int[]> resultOfCalculations = new Dictionary<int, int[]>();

            for (int i = 0; i < FileContent.Length; i++)
                resultOfCalculations.Add(i, TestReadingAndProcessingLinesFromFile_DoStuff(FileContent[i]));
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

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = maxThreadsCount };
            Parallel.For(0, FileContent.Length, options, x =>
            {
                //threadIds.TryAdd(Environment.CurrentManagedThreadId, 1);

                resultOfCalculations.TryAdd(x, TestReadingAndProcessingLinesFromFile_DoStuff(FileContent[x]));
            });

            //Console.WriteLine(threadIds.Count);
        }

        static int[] TestReadingAndProcessingLinesFromFile_DoStuff(string s)
        {
            string[] sa = s.Split(new char[' ']);
            int[] ia = new int[sa.Length];
            string charSymbol;
            for (int x = 0; x < sa.Length; x++)
            {
                foreach (char c in sa[x])
                {
                    charSymbol = c.ToString();
                    if (int.TryParse(charSymbol, out int num))
                    {   
                        //just doing some bogus mathematical calculations to simulate work
                        ia[x] = Enumerable.Range(1, 1000).Sum() + (int)((Math.Sqrt(Math.Log(num) % Math.Log10(num)) * Math.Log(Math.Log10(num) / Math.Sqrt(num)))
                            / (Math.Sqrt(Math.Log(num) % Math.Log10(num)) * Math.Sqrt(Math.Log(num) % Math.Log2(num))));
                    }
                }
            }

            //clean up
            Array.Clear(sa, 0, sa.Length);

            return ia;
        }
    }
}
