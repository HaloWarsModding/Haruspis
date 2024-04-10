//-----------------------------------------------------------------------------
// File: LogWriter.cs
// Description: Contains the LogWriter class responsible for logging messages with the specified log level.
//-----------------------------------------------------------------------------

using System.IO;

namespace Ethereal.Core.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Logs a message with the specified log level.
        /// </summary>
        /// <param name="level">The log level of the message.</param>
        /// <param name="message">The message to be logged.</param>
        void Log(LogLevel level, string message);
    }

    public class LogWriter(LogLevel minimumLogLevel, string logDir, string logName, int fileLimit) : ILogger
    {
        private readonly LogLevel minimumLogLevel = minimumLogLevel;
        private readonly string logName = logName;
        private readonly string logPath = logDir;
        private readonly int fileLimit = fileLimit;
        private static readonly object fileWriteLock = new();

        public void Log(LogLevel level, string message)
        {
            if (level >= minimumLogLevel)
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz} [{level}] {message}";
                LogToFile(logMessage);
                LogToConsole(logMessage);
            }
        }

        private void LogToFile(string message)
        {
            try
            {
                string logFilePath = Path.Combine(logPath, $"{logName}.ethlog");
                lock (fileWriteLock)
                {
                    using StreamWriter writer = new(logFilePath, true);
                    writer.WriteLine(message);
                }
                DeleteOldFilesIfLimitExceeded();
            }
            catch (IOException ex)
            {
                throw new IOException($"An error occurred while writing to the log file: {ex.Message}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Unauthorized access error: {ex.Message}", ex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Invalid argument error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}", ex);
            }
        }

        private void DeleteOldFilesIfLimitExceeded()
        {
            string[] logFiles = Directory.GetFiles(logPath, $"{logName}-*.ethlog");
            if (logFiles.Length > fileLimit)
            {
                Array.Sort(logFiles);
                File.Delete(logFiles[0]);
            }
        }

        private void LogToConsole(string message)
        {
            Console.ResetColor();
            switch (minimumLogLevel)
            {
                case LogLevel.Verbose:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine(message);
        }
    }
}