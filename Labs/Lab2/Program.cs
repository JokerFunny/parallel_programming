using Common;
using Lab2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

internal class Program
{
    static bool _runSyncMethod = true;

    private static async Task Main(string[] args)
    {
        using var host = new HostBuilder()
            .UseOrleans(builder => builder.UseLocalhostClustering())
            .Build();

        // Start the host
        await host.StartAsync();

        // Get the grain factory
        var grainFactory = host.Services.GetRequiredService<IGrainFactory>();

        List<string> fileContent = FileReader.GetFileContent("..\\..\\..\\..\\Common\\test.txt").ToList();

        Console.WriteLine("Input number of grains to be created");

        string userInput = Console.ReadLine();

        if (!int.TryParse(userInput, out int numberOfGrains) || numberOfGrains < 1)
            throw new ArgumentOutOfRangeException("Incorrect input for number of grains.");

        string result1 = string.Empty;
        if (_runSyncMethod)
        {
            result1 = await RunSyncVersion(fileContent);

            Console.WriteLine(result1);
        }

        string result2 = await RunGrainsVersion(grainFactory, fileContent, numberOfGrains);

        Console.WriteLine(result2);

        _WriteResultToFile(result1 + result2);

        await host.StopAsync();
        Console.WriteLine("Orleans is stopping...");
    }

    public static async Task<string> RunSyncVersion(List<string> textToProceed)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        var worker = new WorkerGrain();

        var res = await worker.DoStuff(textToProceed.ToList());

        stopwatch.Stop();

        return $"Sync version elapsed - {stopwatch.ElapsedMilliseconds} ms.{Environment.NewLine}";
    }

    public static async Task<string> RunGrainsVersion(IGrainFactory grainFactory, List<string> textToProceed, int numberOfGrains)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        List<IWorkerGrain> grains = new();

        for (int i = 1; i <= numberOfGrains; i++)
            grains.Add(grainFactory.GetGrain<IWorkerGrain>($"grain{i}"));

        ValueTask<(string, IEnumerable<int>)>[] tasksToProceed = new ValueTask<(string, IEnumerable<int>)>[numberOfGrains];

        int chunkSize = textToProceed.Count / numberOfGrains;

        var chunksToProceed = Enumerable.Range(0, numberOfGrains).Select(el =>
            _GetChunkToProceed(el, chunkSize, textToProceed, numberOfGrains)).ToList();

        for (int i = 0; i < chunksToProceed.Count; i++)
            tasksToProceed[i] = grains[i].DoStuff(chunksToProceed[i]);

        var result = await tasksToProceed.WhenAll();

        stopwatch.Stop();

        return $"Number of grains - {numberOfGrains}, elapsed time - {stopwatch.ElapsedMilliseconds} ms.{Environment.NewLine}";
    }

    static List<string> _GetChunkToProceed(int elementIndex, int chunkSize, List<string> textToProceed, int numberOfGrains)
    {
        IEnumerable<string> chunk = textToProceed.Skip(chunkSize * elementIndex);

        return elementIndex++ != numberOfGrains ? chunk.Take(chunkSize).ToList() : chunk.ToList();
    }

    static void _WriteResultToFile(string result)
        => File.AppendAllLines("..\\..\\..\\output.txt", new[] { result });
}