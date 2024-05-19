namespace IO
{
    public static class FolderMover
    {
        /// <summary>
        /// Moves the contents of a source folder to a destination folder asynchronously.
        /// </summary>
        /// <param name="sourceFolder">The source folder to move.</param>
        /// <param name="destinationFolder">The destination folder to move to.</param>
        /// <param name="progress">The progress reporter for reporting move progress.</param>
        public static async Task MoveFolderAsync(string sourceFolder, string destinationFolder, IProgress<double> progress)
        {
            try
            {
                string mainDestinationFolder = destinationFolder;

                CreateDirectoryIfNotExists(mainDestinationFolder);

                string[] files = Directory.GetFiles(sourceFolder);
                string[] directories = Directory.GetDirectories(sourceFolder);

                int totalItems = files.Length + directories.Length;
                ProgressTracker progressTracker = new() { TotalItems = totalItems, Progress = progress };

                List<Task> fileTasks = [];
                foreach (string file in files)
                {
                    string destFile = Path.Combine(mainDestinationFolder, Path.GetFileName(file));
                    fileTasks.Add(MoveFileAsync(file, destFile, progressTracker));
                }
                await Task.WhenAll(fileTasks);

                List<Task> directoryTasks = [];
                foreach (string directory in directories)
                {
                    string destDir = Path.Combine(mainDestinationFolder, Path.GetFileName(directory));
                    directoryTasks.Add(MoveFolderAsync(directory, destDir, progress));
                }
                await Task.WhenAll(directoryTasks);

                Directory.Delete(sourceFolder, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to move folder {sourceFolder}: {ex.Message}");
            }
        }

        /// <summary>
        /// Moves a single file to the destination path asynchronously.
        /// </summary>
        /// <param name="sourceFile">The source file to move.</param>
        /// <param name="destinationFile">The destination path for the file.</param>
        /// <param name="progressTracker">The progress tracker for reporting move progress.</param>
        private static async Task MoveFileAsync(string sourceFile, string destinationFile, ProgressTracker progressTracker)
        {
            try
            {
                await Task.Run(() => File.Move(sourceFile, destinationFile, true));
                progressTracker.IncrementMovedItems();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to move file {sourceFile} to {destinationFile}: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates the directory if it does not exist.
        /// </summary>
        /// <param name="directory">The directory to create.</param>
        private static void CreateDirectoryIfNotExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                _ = Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// A class to track the progress of moved items.
        /// </summary>
        private class ProgressTracker
        {
            public int TotalItems { get; set; }
            public int MovedItems { get; private set; }
            public IProgress<double> Progress { get; set; }

            public void IncrementMovedItems()
            {
                MovedItems++;
                Progress?.Report((double)MovedItems / TotalItems * 100);
            }
        }
    }
}
