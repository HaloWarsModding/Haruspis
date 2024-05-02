namespace Ethereal.Logging
{
    public sealed class Logger : Singleton<Logger>
    {
        private static LogLevel MinimumLevel { get; set; } = LogLevel.Debug;
        private static string LogPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        private static int FileLimit { get; set; } = 10;
        private static LogWriter _writer = new(MinimumLevel, LogPath, FileLimit);

        public static void Log(LogLevel level, string message)
        {
            _writer.Log(level, message);
        }
        public static void SetPath(string path)
        {
            LogPath = path;
            _writer = new(MinimumLevel, LogPath, FileLimit);
        }
    }

    public enum LogLevel
    {
        Verbose,
        Debug,
        Information,
        Warning,
        Error
    }
}