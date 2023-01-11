using BenchmarkDotNet.Attributes;
using Common;
using System.Collections.Concurrent;

namespace Lab1
{
    [MemoryDiagnoser]
    [Config(typeof(FastAndDirtyConfig))]
    public class Lab1Benchmark
    {
        public static string[] FileContent = FileReader.GetFileContent("..\\..\\..\\..\\test.txt");

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
        [Arguments(16)]
        [Arguments(32)]
        [Arguments(64)]
        [Arguments(128)]
        [Arguments(256)]
        [Arguments(512)]
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

        [Benchmark]
        [Arguments(32)]
        [Arguments(64)]
        [Arguments(128)]
        [Arguments(256)]
        [Arguments(512)]
        [Arguments(1024)]
        [Arguments(2048)]
        public async Task<int> ParallelExecutorToCompareWith2(int maxThreadsCount)
        {
            ThreadPool.SetMinThreads(maxThreadsCount, maxThreadsCount);
            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);

            Dictionary<int, int[]> resultOfCalculations = new Dictionary<int, int[]>();

            var textToProceed = FileContent.ToList();

            int chunkSize = textToProceed.Count / maxThreadsCount;

            var chunksToProceed = Enumerable.Range(0, maxThreadsCount).Select(el =>
                _GetChunkToProceed(el, chunkSize, textToProceed, maxThreadsCount)).ToList();

            Task<int[]>[] tasksToProceed = new Task<int[]>[maxThreadsCount];

            for (int i = 0; i < chunksToProceed.Count; i++)
            {
                List<string> chuckTextToProceed = chunksToProceed[i];

                tasksToProceed[i] = new Task<int[]>(() => _sDoWork(chuckTextToProceed));
            }
            foreach (var task in tasksToProceed)
                task.Start();

            var results = await Task.WhenAll(tasksToProceed);

            for (int i = 0; i < results.Length; i++)
                resultOfCalculations.Add(i, results[i]);

            return 1;
        }

        static int[] _sDoWork(List<string> textToProceed)
        {
            List<int> result = new List<int>();

            for (int j = 0; j < textToProceed.Count; j++)
                result.AddRange(Worker.DoStuff(textToProceed[j]));

            return result.ToArray();
        }

        static List<string> _GetChunkToProceed(int elementIndex, int chunkSize, List<string> textToProceed, int numberOfGrains)
        {
            IEnumerable<string> chunk = textToProceed.Skip(chunkSize * elementIndex);

            return elementIndex++ != numberOfGrains ? chunk.Take(chunkSize).ToList() : chunk.ToList();
        }
    }
}
