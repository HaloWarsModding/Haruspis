using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Ethereal.GUI.Pages.Mods.UserControls;

namespace Ethereal.GUI.Pages
{
    public partial class ModsPage : Page
    {
        public static event EventHandler<ModChangedEventArgs> ModChanged;
        public readonly List<Mod> Mods = [];
        public static ModControl SelectedModControl;

        private readonly DispatcherTimer timer = new();
        private bool wasGameRunning = false;

        public ModsPage()
        {
            InitializeComponent();
            PopulateModList();

            ModChanged += ModsPage_ModChanged;
            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            BtnStart.IsEnabled = !Core.IsGameRunning;
            BtnStart.Content = Core.IsGameRunning ? "Launched" : "Launch";

            if (wasGameRunning != Core.IsGameRunning)
            {
                if (wasGameRunning && !Core.IsGameRunning)
                {
                    UpdateInfo(SelectedModControl.CurrentMod);
                }

                wasGameRunning = Core.IsGameRunning;
            }
        }

        private void ModsPage_ModChanged(object sender, ModChangedEventArgs e)
        {
            UpdateInfo(e.ChangedMod);
        }

        public async void PopulateModList()
        {
            Mods.Clear();

            Mod vanillaMod = CreateVanillaMod();
            Mods.Add(vanillaMod);
            SelectedModControl = null;

            await LoadModsFromDirectories();

            modPanelControl.InitializeModList(Mods);
            modPanelControl.SelectLastMod(Core.Config.Settings.LastSelectedMod);

            SelectedModControl = modPanelControl.SelectedModControl;
        }

        private static Mod CreateVanillaMod()
        {
            return new Mod
            {
                Name = "Halo Wars: Definitive Edition",
                Description = "The Spirit of Fire is sent to the ruined planet Harvest to investigate Covenant activity...",
                Author = "Ensemble Studios",
                ModPath = Core.Config.HaloWars.Path,
                DataPath = Core.Config.HaloWars.Path != string.Empty
                           ? Path.Combine(Path.GetDirectoryName(Core.Config.HaloWars.Path), "data.eth")
                           : string.Empty
            };
        }

        private async Task LoadModsFromDirectories()
        {
            await Task.Run(() =>
            {
                foreach (string directoryPath in Directory.EnumerateDirectories(Core.Config.Mods.Path))
                {
                    Mod mod = new();
                    mod.FromPath(directoryPath);

                    if (mod.IsMod)
                    {
                        Mods.Add(mod);
                    }
                }
            });
        }

        public static void SetCurrentMod(Mod mod)
        {
            ModChanged?.Invoke(null, new ModChangedEventArgs(mod));
        }

        private void UpdateInfo(Mod mod)
        {
            BitmapImage bitmap = ImageProcessing.LoadBannerImage(mod);
            ImgBanner.Source = bitmap;

            UpdateModData(mod);
            UpdateChangelog(mod);
        }

        private void UpdateModData(Mod mod)
        {
            string dataPath = mod.DataPath;
            ETHData data = new();

            if (!File.Exists(dataPath))
            {
                data.ToFile(dataPath);
                return;
            }

            data.FromFile(dataPath);

            playTimeControl.UpdatePlayTime(data.Data.PlayTime);
            lastPlayedControl.UpdateLastPlayedDate(data.Data.LastPlayed);
        }

        private static void UpdateChangelog(Mod mod)
        {
            string changelogPath = mod.ChangelogPath;

            if (!string.IsNullOrEmpty(changelogPath) && File.Exists(changelogPath))
            {
                // Load and display the changelog
            }
            else if (!string.IsNullOrEmpty(mod.Description))
            {
                // Display the description as a fallback
            }
        }

        #region Event Handlers
        private void LaunchGame(object sender, RoutedEventArgs e)
        {
            if (SelectedModControl == null) return;

            string modName = SelectedModControl.CurrentMod.Name;
            string modPath = SelectedModControl.CurrentMod.ModPath;

            Core.Manifest.Content = modName == "Halo Wars: Definitive Edition" ? string.Empty : modPath;
            Core.Config.Mods.LastPlayedMod = modName == "Halo Wars: Definitive Edition" ? "Vanilla" : modName;

            Core.Manifest.ToFile(Core.ManifestPath);
            Core.Config.ToFile(Core.ConfigPath);

            GameProcess.Instance.StartGame(Core.Config.HaloWars.CurrentDistribution, false);
            GameProcess.Instance.StartMonitoring();
        }

        private void ShowModSettings(object sender, MouseButtonEventArgs e)
        {
            // Implement the event handler logic
        }
        #endregion
    }

    public class ModChangedEventArgs(Mod changedMod) : EventArgs
    {
        public Mod ChangedMod { get; } = changedMod;
    }
}
