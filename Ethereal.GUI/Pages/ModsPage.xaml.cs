using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Ethereal.GUI.Pages
{
    public partial class ModsPage : Window
    {
        public string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.xml");
        private readonly string ManifestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Halo Wars", "ModManifest.txt");
        public readonly MainWindow window;
        private readonly List<Mod> Mods = []; 

        public ModsPage(MainWindow window)
        {
            InitializeComponent();
            this.window = window;
            InitializeModsList(this, new RoutedEventArgs());
        }

        #region UI Event Handlers

        private void DragPage(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitPage(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public void AddMod(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vistaFolderBrowserDialog = new()
            {
                Multiselect = false
            };

            if (vistaFolderBrowserDialog.ShowDialog().GetValueOrDefault())
            {
                string selectedFolderPath = vistaFolderBrowserDialog.SelectedPath;
                try
                {
                    string modDirectory = Path.Combine(MainWindow.config.Mods.Path, Path.GetFileName(selectedFolderPath));

                    Directory.Move(selectedFolderPath, modDirectory);

                    Logger.GetInstance().Log(LogLevel.Information, "Mod added successfully: " + modDirectory);

                    InitializeModsList(this, new RoutedEventArgs());
                }
                catch (Exception ex)
                {
                    Logger.GetInstance().Log(LogLevel.Error, "Error adding mod: " + ex.Message);
                }
            }
        }

        public void RemoveMod(object sender, RoutedEventArgs e)
        {
            if (ListViewMods.SelectedItem is Mod selectedMod && selectedMod.Name != "Vanilla")
            {
                try
                {
                    Directory.Delete(selectedMod.ModPath, true);

                    Logger.GetInstance().Log(LogLevel.Information, "Mod removed successfully: " + selectedMod.Name);

                    InitializeModsList(this, new RoutedEventArgs());
                }
                catch (Exception ex)
                {
                    Logger.GetInstance().Log(LogLevel.Error, "Error removing mod: " + ex.Message);
                }
            }
        }

        private void InitializeModsList(object sender, RoutedEventArgs e)
        {
            ListViewMods.Items.Clear();

            string path = MainWindow.config.Mods.Path;

            Mod vanillaMod = new() { Name = "Vanilla" };
            _ = ListViewMods.Items.Add(vanillaMod); 

            Mods.Clear();

            foreach (string directoryPath in Directory.GetDirectories(path))
            {
                Mod mod = new();
                mod.FromPath(directoryPath);

                if (mod.IsMod)
                {
                    Mods.Add(mod);
                    _ = ListViewMods.Items.Add(mod); 
                }
            }
        }

        private void ListViewMods_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ListViewMods.SelectedItem is Mod selectedMod)
            {
                if (selectedMod.Name == "Vanilla")
                {
                    MainWindow.manifest.Content = string.Empty;
                    MainWindow.manifest.ToFile(ManifestPath);

                    MainWindow.config.Mods.LastPlayedMod = "Vanilla";
                    MainWindow.config.ToFile(configPath);
                    return;
                }

                MainWindow.manifest.Content = selectedMod.ModPath;
                MainWindow.manifest.ToFile(ManifestPath);

                MainWindow.config.Mods.LastPlayedMod = selectedMod.Name;
                MainWindow.config.ToFile(configPath);

                if (selectedMod.Description != string.Empty)
                {
                    window.SetChangelogContent(selectedMod.Description);
                }
            }
        }

        #endregion
    }
}
