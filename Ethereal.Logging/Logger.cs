//-----------------------------------------------------------------------------
// File: Logger.cs
// Description: Provides functionality for logging messages to files with customizable settings.
//-----------------------------------------------------------------------------

namespace Ethereal.Logging
{
    public sealed class Logger
    {
        private static Logger? _instance;
        private static LogLevel MinimumLevel { get; set; } = LogLevel.Debug;
        private static string LogPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        private static int FileLimit { get; set; } = 10;
        private static LogWriter _writer = new(MinimumLevel, LogPath, FileLimit);

        private Logger() { }

        public static Logger GetInstance()
        {
            if (_instance == null)
            {
                lock (typeof(Logger))
                {
                    _instance ??= new Logger();
                }
            }
            return _instance;
        }

        public void Log(LogLevel level, string message)
        {
            _writer.Log(level, message);
        }

        public void SetPath(string path)
        {
            LogPath = path;
            _writer = new(MinimumLevel, LogPath, FileLimit);
        }
    }

    /// <summary>
    /// Represents different levels of logging:
    /// - Verbose: Detailed information for debugging purposes.
    /// - Debug: Information useful for debugging the application.
    /// - Information: General information about the application's operation.
    /// - Warning: Indicates potential issues that are not critical.
    /// - Error: Indicates critical errors that require attention.
    /// </summary>
    public enum LogLevel
    {
        Verbose,
        Debug,
        Information,
        Warning,
        Error
    }
}
