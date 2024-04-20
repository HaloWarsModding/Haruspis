using Ethereal.GUI.Boxes;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Ethereal.GUI.Pages
{
    public partial class ModsPage : Window
    {
        public string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.xml");
        public readonly List<Mod> Mods = [];

        public ModsPage()
        {
            InitializeComponent();
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

        private async void InitializeModsList(object sender, RoutedEventArgs e)
        {
            ModList.Children.Clear();

            string path = MainWindow.config.Mods.Path;

            Mod vanillaMod = new() { Name = "Vanilla", Description = "The Spirit of Fire is sent to the ruined planet Harvest to investigate Covenant activity, where Cutter learns that the Covenant has excavated something at the planet's northern pole. When the UNSC's main outpost on Harvest is captured, Cutter orders Forge to retake it. Soon after, Forge scouts the Covenant excavation and discovers that they, under the direction of the Arbiter, have discovered a Forerunner facility. Forge's troops defeat the Covenant forces before they can destroy the installation, and Anders arrives. She determines that the facility is an interstellar map, and recognizes a set of coordinates that points to the human colony of Arcadia.", Author = "Ensemble Studios" };
            Mods.Add(vanillaMod);

            Mods.Clear();

            await Task.Run(() =>
            {
                _ = Parallel.ForEach(Directory.EnumerateDirectories(path), directoryPath =>
                {
                    Mod mod = new();
                    mod.FromPath(directoryPath);

                    if (mod.IsMod)
                    {
                        Mods.Add(mod);
                    }
                });
            });

            foreach (Mod mod in Mods)
            {
                ModBox modBox = new(mod);
                _ = ModList.Children.Add(modBox);
            }
        }

        private void ListViewMods_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        #endregion
    }
}
