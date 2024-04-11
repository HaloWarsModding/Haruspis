using Ethereal.Core.HaloWars;
using Ethereal.Core.Logging;
using System.Windows;
using System.Windows.Input;

namespace Ethereal.ModManager.UI
{
    public partial class ModsPage : Window
    {
        private readonly LogWriter writer = MainWindow.logWriter;
        private readonly Manager manager = MainWindow.manager;
        private readonly List<Mod> mods;
        private readonly string modPath = MainWindow.CurrentConfiguration.Game.ModsDirectory?.ToString();
        public ModsPage()
        {
            InitializeComponent();

            modPath ??= (MainWindow.dynamicProperties.TryGetProperty("modsDir", out object path) ? (string)path : null);

            mods = manager.GetMods(modPath);
            foreach (Mod mod in mods)
            {
                _ = ListBoxMods.Items.Add(mod.Name);
            }

            Mod vanilla = new() { Name = "Vanilla" };
            _ = ListBoxMods.Items.Add(vanilla.Name);

            ListBoxMods.MouseDoubleClick += ListBoxMods_MouseDoubleClick;
        }

        private void ListBoxMods_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string selectedModTitle = (string)ListBoxMods.SelectedItem;

            writer.Log(Core.LogLevel.Information, $"User double-clicked on mod: {selectedModTitle}");

            Mod selectedMod = mods.Find(mod => mod.Name == selectedModTitle);

            if (selectedMod != null)
            {
                writer.Log(Core.LogLevel.Information, $"Selected mod details - Name: {selectedMod.Name}, Version: {selectedMod.Version}");

                manager.SetCurrentMod(selectedMod);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void PnlDragWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
