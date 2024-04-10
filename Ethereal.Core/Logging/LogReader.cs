//-----------------------------------------------------------------------------
// File: LogReader.cs
// Description: Contains the LogReader class responsible for reading the content of the latest modified log file.
//-----------------------------------------------------------------------------

using System.IO;

namespace Ethereal.Core.Logging
{
    public interface ILogReader
    {
        /// <summary>
        /// Reads the content of the latest modified log file in the specified log directory.
        /// </summary>
        /// <returns>The content of the latest log file as a string. If no log files are found, returns a message indicating so. Throws exceptions if unable to read the log file.</returns>
        string ReadLatestLog();
    }

    public class LogReader(string logDir, LogWriter logWriter) : ILogReader
    {
        private readonly string logPath = logDir;
        private readonly LogWriter logWriter = logWriter;

        public string ReadLatestLog()
        {
            try
            {
                DirectoryInfo directory = new(logPath);
                FileInfo? latestLogFile = directory.GetFiles("*.ethlog").OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

                if (latestLogFile == null)
                {
                    logWriter.Log(LogLevel.Error, "No log files found in the directory");
                    throw new FileNotFoundException("No log files found in the directory.");
                }

                using FileStream fileStream = latestLogFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                string logContent = File.ReadAllText(latestLogFile.FullName);
                Console.WriteLine("Reading latest log file: " + latestLogFile.Name);
                logWriter.Log(LogLevel.Information, "Reading latest log file: " + latestLogFile.Name);

                return logContent;
            }
            catch (FileNotFoundException ex)
            {
                logWriter.Log(LogLevel.Error, "Custom error message for FileNotFoundException: " + ex.Message);
                throw new FileNotFoundException("Custom error message for FileNotFoundException", ex);
            }
            catch (Exception ex)
            {
                logWriter.Log(LogLevel.Error, "Custom error message for general Exception: " + ex.Message);
                throw new Exception("Custom error message for general Exception", ex);
            }
        }
    }
}