using Microsoft.Win32;

namespace EtherealModManagerGUI.Data.Dialogs
{
    internal class DFiles
    {
        public static OpenFileDialog GameExecutable()
        {
            return new OpenFileDialog
            {
                Filter = "Game Executable|*.exe",
                Title = Properties.Resources.SelectGameExecutable
            };
        }
    }
}
