using Microsoft.Win32;

namespace EtherealModManager.Dialogs
{
    internal class DFiles
    {

        public static OpenFileDialog CreateDialogFile(DFileType type)
        {
            return type switch
            {
                DFileType.GameExecutable => new OpenFileDialog()
                {
                    Filter = "Game Executable|*.exe",
                    Multiselect = false,
                    Title = Properties.Resources.SelectGameExecutable
                },


                _ => throw new ArgumentException("Invalid dialog file type"),
            };
        }
    }

    public enum DFileType
    {
        GameExecutable,
    }

}
