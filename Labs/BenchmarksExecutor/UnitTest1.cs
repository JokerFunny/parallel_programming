using BenchmarkDotNet.Running;
using Labs;

namespace BenchmarksExecutor
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            BenchmarkRunner.Run<TheEasiestBenchmark>();
        }
    }
}