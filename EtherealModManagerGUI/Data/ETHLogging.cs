using Serilog;
using Serilog.Events;
using System.IO;

namespace EtherealModManagerGUI
{
    internal class ETHLogging
    {
        private static readonly Serilog.Core.Logger logger;

        public enum LogLevel
        {
            Verbose = LogEventLevel.Verbose,
            Debug = LogEventLevel.Debug,
            Information = LogEventLevel.Information,
            Warning = LogEventLevel.Warning,
            Error = LogEventLevel.Error
        }

        public static LogLevel MinimumLogLevel { get; set; } = LogLevel.Verbose;

        static ETHLogging()
        {
            string path = $"log-{DateTime.Now:yyyyMMddHHmmss}.log";
            string text = Path.Combine(ETHPath.Logging.Paths, path);
            logger = new LoggerConfiguration().MinimumLevel.Is((LogEventLevel)MinimumLogLevel).WriteTo.File(text, LogEventLevel.Verbose, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null, retainedFileCountLimit: 15, fileSizeLimitBytes: 1073741824L).CreateLogger();
        }

        public static void Log(LogLevel level, string message)
        {
            if (logger.IsEnabled((LogEventLevel)level))
            {
                logger.Write((LogEventLevel)level, message);
            }
        }
    }
}
