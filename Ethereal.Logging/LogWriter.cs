namespace Ethereal.Logging
{
    public class LogWriter
    {
        private readonly LogLevel minimumLogLevel;
        private readonly string logPath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly int fileLimit;
        private static readonly object fileWriteLock = new();

        public LogWriter(LogLevel minimumLogLevel, string logDir, int fileLimit)
        {
            if (logDir == null)
            {
                LogToConsole(LogLevel.Error, "Error: Log directory cannot be null.");
                return;
            }

            if (fileLimit <= 0)
            {
                LogToConsole(LogLevel.Error, "Error: File limit must be a positive value.");
                return;
            }

            if (!Directory.Exists(logDir))
            {
                LogToConsole(LogLevel.Error, $"Error: Log directory '{logDir}' does not exist.");
                return;
            }

            this.minimumLogLevel = minimumLogLevel;
            logPath = logDir;
            this.fileLimit = fileLimit;
        }
        public void Log(LogLevel level, string message)
        {
            if (level < minimumLogLevel)
            {
                return;
            }

            string logMessage = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss.fff} [{level}] {message}";
            LogToFile(logMessage);
            LogToConsole(level, logMessage);
        }

        private void LogToFile(string message)
        {
            string logFileName = $"{DateTime.Now:yyyy-MM-dd}.ethlog";
            string logFilePath = Path.Combine(logPath, logFileName);
            lock (fileWriteLock)
            {
                try
                {
                    using StreamWriter writer = new(logFilePath, true);
                    writer.WriteLine(message);
                }
                catch (Exception ex)
                {
                    LogToConsole(LogLevel.Error, $"Error writing to log file: {ex.Message}");
                }
            }
            DeleteOldFilesIfLimitExceeded();
        }
        private void DeleteOldFilesIfLimitExceeded()
        {
            string[] logFiles = Directory.GetFiles(logPath, "*.ethlog");
            if (logFiles.Length <= fileLimit || logFiles.Length <= 0)
            {
                return;
            }

            Array.Sort(logFiles);

            try
            {
                File.Delete(logFiles[0]);
            }
            catch (Exception ex)
            {
                LogToConsole(LogLevel.Error, $"Error deleting log file: {ex.Message}");
            }
        }
        private static void LogToConsole(LogLevel level, string message)
        {
            Console.ResetColor();

            switch (level)
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
