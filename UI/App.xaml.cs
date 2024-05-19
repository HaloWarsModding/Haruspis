using System.Runtime.InteropServices;
using System.Windows;

namespace UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Environment.OSVersion.Version.Major >= 6)
            {
                _ = SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            }

            if (e.Args.Length > 0 && e.Args[0].Equals("-console", StringComparison.OrdinalIgnoreCase))
            {
                ShowConsoleWindow();
            }
        }

        private static void ShowConsoleWindow()
        {
            _ = AllocConsole();
        }

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT value);

        private enum DPI_AWARENESS_CONTEXT
        {
            DPI_AWARENESS_CONTEXT_UNAWARE = 16,
            DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = 17,
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = 18,
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = 34
        }

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool AllocConsole();
    }
}
