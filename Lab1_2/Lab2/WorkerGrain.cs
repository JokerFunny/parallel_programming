using Common;

namespace Lab2
{
    public class WorkerGrain : Grain, IWorkerGrain
    {
        public Guid GrainId = Guid.NewGuid();

        public ValueTask<(string, IEnumerable<int>)> DoStuff(List<string> stringsToProceed)
        {
            List<int> result = new();

            for (int i = 0; i < stringsToProceed.Count; i++)
                result.AddRange(Worker.DoStuff(stringsToProceed[i]));

            return ValueTask.FromResult((GrainId.ToString(), result.AsEnumerable()));
        }
    }
}
