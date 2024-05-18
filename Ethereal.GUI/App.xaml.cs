using System.Runtime.InteropServices;
using System.Windows;

namespace Ethereal.GUI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (Environment.OSVersion.Version.Major >= 6)
            {
                _ = SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
            }
        }

        [DllImport("user32.dll")]
#pragma warning disable SYSLIB1054 
        private static extern bool SetProcessDPIAware();
#pragma warning restore SYSLIB1054 

        [DllImport("shcore.dll")]
#pragma warning disable SYSLIB1054 
        private static extern int SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value);
#pragma warning restore SYSLIB1054 

        private enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }

    }

}
