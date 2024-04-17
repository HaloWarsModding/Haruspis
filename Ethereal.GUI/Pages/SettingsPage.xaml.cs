using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Ethereal.GUI.Pages
{
    public partial class SettingsPage : Window
    {
        public string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.xml");

        public SettingsPage()
        {
            InitializeComponent();
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            TBoxModDir.Text = MainWindow.config.Mods.Path;
        }

        #region UI Event Handlers

        private void ExitPage(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DragPage(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void DialogModDir(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vistaFolderBrowserDialog = new()
            {
                Multiselect = false
            };

            if (vistaFolderBrowserDialog.ShowDialog().Value == true)
            {
                TBoxModDir.Text = vistaFolderBrowserDialog.SelectedPath;
            }
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            MainWindow.config.Mods.Path = TBoxModDir.Text;

            MainWindow.config.ToFile(configPath);
        }

        #endregion
    }
}
