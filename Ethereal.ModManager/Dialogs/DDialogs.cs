using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace Ethereal.ModManager.Dialogs
{
    internal class DDialogs
    {
        public static FileDialog CreateDialogFile(DFileType type)
        {
            return type switch
            {
                DFileType.GameExecutable => new OpenFileDialog()
                {
                    Filter = "Game Executable|*.exe",
                    Multiselect = false,
                    Title = Properties.Resources.GameExecutableDialog
                },
                _ => throw new ArgumentException("Invalid dialog file type"),
            };
        }

        public static VistaFolderBrowserDialog CreateDialogFolder(DFolderType type)
        {
            return type switch
            {
                DFolderType.AddMod => new VistaFolderBrowserDialog()
                {
                    UseDescriptionForTitle = true,
                    Description = Properties.Resources.AddModDialog
                },
                _ => throw new ArgumentException("Invalid dialog folder type"),
            };
        }
    }

    public enum DFileType
    {
        GameExecutable
    }

    public enum DFolderType
    {
        AddMod
    }
}
