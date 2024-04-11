using Ethereal.Core.HaloWars;
using Ethereal.Core.Logging;
using Ethereal.ModManager.Dialogs;
using Ookii.Dialogs.Wpf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Ethereal.ModManager.UI
{
    public partial class ModsPage : Window
    {
        private readonly LogWriter writer = MainWindow.logWriter;
        private readonly Manager manager = MainWindow.manager;
        private List<Mod> mods;
        private readonly string modPath = MainWindow.CurrentConfiguration.Game.ModsDirectory?.ToString();

        public ModsPage()
        {
            InitializeComponent();
            modPath ??= MainWindow.dynamicProperties.TryGetProperty("modsDir", out object path) ? (string)path : null;
            InitializeModList();
        }

        private void InitializeModList()
        {
            ListBoxMods.Items.Clear();
            mods = manager.GetMods(modPath);
            mods.ForEach(mod => ListBoxMods.Items.Add(mod.Name));
            ListBoxMods.Items.Add("Vanilla");
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void PnlDragWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => InitializeModList();

        private void BtnRemoveMod_Click(object sender, RoutedEventArgs e)
        {
            string selectedModTitle = ListBoxMods.SelectedItem as string;
            if (selectedModTitle == null)
            {
                var removeModDialog = DBoxes.CreateDialogBox(DBoxType.RemoveMod);
                _ = removeModDialog.ShowDialog();
                return;
            }

            var info = new DirectoryInfo(modPath);
            var toDelete = info.EnumerateDirectories(selectedModTitle).First();
            toDelete.Delete(true);
            InitializeModList();
        }

        private void BtnAddMod_Click(object sender, RoutedEventArgs e)
        {
            var addModDialog = DDialogs.CreateDialogFolder(DFolderType.AddMod);
            _ = addModDialog.ShowDialog();

            if (addModDialog.SelectedPath == null) return;

            var info = new DirectoryInfo(addModDialog.SelectedPath);
            string newPath = Path.Combine(modPath, info.Name);
            _ = Directory.CreateDirectory(newPath);
            CopyFilesRecursively(info.FullName, newPath);
            Directory.Delete(addModDialog.SelectedPath, true);
            InitializeModList();
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                _ = Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }

        private void ListBoxMods_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedMod = ListBoxMods.SelectedItem;
            BtnRemoveMod.IsEnabled = BtnPlayThis.IsEnabled = selectedMod != null && selectedMod.ToString() != "Vanilla";
        }

        private void BtnModPagePlayThis_Click(object sender, RoutedEventArgs e)
        {
            string selectedModTitle = ListBoxMods.SelectedItem as string;
            if (selectedModTitle != null)
            {
                var selectModDialog = DBoxes.CreateDialogBox(DBoxType.SelectMod, selectedModTitle);
                _ = selectModDialog.ShowDialog();

                if (selectModDialog.DialogResult == true)
                {
                    writer.Log(Core.LogLevel.Information, $"User clicked on mod: {selectedModTitle}");

                    Mod selectedMod = mods.Find(mod => mod.Name == selectedModTitle);

                    if (selectedMod != null)
                    {
                        writer.Log(Core.LogLevel.Information, $"Selected mod details - Name: {selectedMod.Name}, Version: {selectedMod.Version}");
                        manager.SetCurrentMod(selectedMod);
                    }
                }
            }
        }
    }
}
