using System.Diagnostics;

namespace IO
{
    public partial class Compacting
    {
        public static async Task<(bool Success, string Output, string Error, double Progress)> CompressFolderAsync(string FolderPath, IProgress<double> progress)
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    return (false, string.Empty, "The directory does not exist.", 0);
                }

                string quotedFolderPath = $"\"{FolderPath}\"";

                ProcessStartInfo processStartInfo = new()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C compact /c /i /s:{quotedFolderPath}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process process = new()
                {
                    StartInfo = processStartInfo
                };

                _ = process.Start();

                // Periodically check the compression progress
                Task progressTask = Task.Run(() =>
                {
                    while (!process.HasExited)
                    {
                        double progressValue = CalculateCompressionProgress(FolderPath);
                        progress.Report(progressValue);
                        Thread.Sleep(1000); // Check every second
                    }
                });

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();
                await progressTask; // Ensure progress task completes

                if (process.ExitCode == 0)
                {
                    double finalProgress = CalculateCompressionProgress(FolderPath);
                    return (true, output, string.Empty, finalProgress);
                }
                else
                {
                    return (false, string.Empty, error, 0);
                }
            }
            catch (Exception ex)
            {
                return (false, string.Empty, ex.Message, 0);
            }
        }

        private static double CalculateCompressionProgress(string folderPath)
        {
            int totalFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories).Length;
            int compressedFiles = 0;

            foreach (string file in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                FileInfo fileInfo = new(file);
                if ((fileInfo.Attributes & FileAttributes.Compressed) != 0)
                {
                    compressedFiles++;
                }
            }

            return totalFiles > 0 ? (double)compressedFiles / totalFiles * 100 : 0;
        }
    }
}
