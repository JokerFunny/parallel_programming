namespace Lab2
{
    public interface IWorkerGrain : IGrainWithStringKey
    {
        ValueTask<(string, IEnumerable<int>)> DoStuff(List<string> stringsToProceed);
    }
}
