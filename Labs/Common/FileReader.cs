namespace Common
{
    public static class FileReader
    {
        /// <summary>
        /// Get file content by <paramref name="targetPath"/>.
        /// </summary>
        /// <param name="targetPath">Path to the file.</param>
        /// <returns>
        ///     File content splitted by lines.
        /// </returns>
        public static string[] GetFileContent(string targetPath)
        {
            return File.ReadAllLines(targetPath);
        }
    }
}
