namespace Common
{
    public static class ValueTaskExtensions
    {
        public static async ValueTask<T[]> WhenAll<T>(this ValueTask<T>[] tasks)
        {
            ArgumentNullException.ThrowIfNull(tasks);
            if (tasks.Length == 0)
                return Array.Empty<T>();

            var results = new T[tasks.Length];
            for (var i = 0; i < tasks.Length; i++)
                results[i] = await tasks[i].ConfigureAwait(false);

            return results;
        }
    }
}
