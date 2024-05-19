using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using UI.Core;
using UI.Pages.Mods.UserControls;

using static Data.Mod;

namespace UI.Pages.Mods
{
    public partial class ModsPage : Page
    {
        public static event EventHandler<ModChangedEventArgs> ModChanged;
        public readonly List<Mod> Mods = [];
#pragma warning disable CA2211 
        public static UserControls.ModControl SelectedModControl;
#pragma warning restore CA2211 

        private readonly DispatcherTimer _timer = new();
        private bool _wasGameRunning;

        /// <summary>
        /// Initializes a new instance of the ModsPage class.
        /// </summary>
        public ModsPage()
        {
            InitializeComponent();
            PopulateModList();

            ModChanged += OnModChanged;
            _timer.Interval = TimeSpan.FromSeconds(0.01);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        /// <summary>
        /// Handles the timer tick event to update UI elements.
        /// </summary>
        private void OnTimerTick(object sender, EventArgs e)
        {
            BtnStart.IsEnabled = !Main.IsGameRunning;
            BtnStart.Content = Main.IsGameRunning ? "Launched" : "Launch";

            if (_wasGameRunning != Main.IsGameRunning)
            {
                if (_wasGameRunning && !Main.IsGameRunning)
                {
                    UpdateInfo(SelectedModControl?.CurrentMod);
                }

                _wasGameRunning = Main.IsGameRunning;
            }
        }

        /// <summary>
        /// Handles the ModChanged event to update the displayed mod information.
        /// </summary>
        private void OnModChanged(object sender, ModChangedEventArgs e)
        {
            UpdateInfo(e.ChangedMod);
        }

        /// <summary>
        /// Populates the mod list by loading mods from directories.
        /// </summary>
        public async void PopulateModList()
        {
            Mods.Clear();
            Mods.Add(CreateVanillaMod());
            SelectedModControl = null;

            await LoadModsFromDirectories();

            modPanelControl.InitializeModList(Mods);
            modPanelControl.SelectLastMod(Main.Config.Settings.LastSelectedMod);
            SelectedModControl = modPanelControl.SelectedModControl;
        }

        /// <summary>
        /// Creates a vanilla mod representing the base game.
        /// </summary>
        private static Mod CreateVanillaMod()
        {
            return new Mod
            {
                Properties = new ModProperties
                {
                    Name = "Halo Wars: Definitive Edition",
                    Description = "The Spirit of Fire is sent to the ruined planet Harvest to investigate Covenant activity...",
                    Author = "Ensemble Studios",
                    ModPath = Main.Config.HaloWars.Path,
                    DataPath = !string.IsNullOrEmpty(Main.Config.HaloWars.Path)
                        ? Path.Combine(Path.GetDirectoryName(Main.Config.HaloWars.Path), "data.eth")
                        : string.Empty
                }
            };
        }

        /// <summary>
        /// Loads mods from directories asynchronously.
        /// </summary>
        private async Task LoadModsFromDirectories()
        {
            await Task.Run(() =>
            {
                foreach (string directoryPath in Directory.EnumerateDirectories(Main.Config.Mods.Path))
                {
                    Mod mod = new();
                    mod.FromPath(directoryPath);

                    if (mod.Properties.IsValid)
                    {
                        Mods.Add(mod);
                    }
                }
            });
        }

        /// <summary>
        /// Sets the current mod and raises the ModChanged event.
        /// </summary>
        public static void SetCurrentMod(Mod mod)
        {
            ModChanged?.Invoke(null, new ModChangedEventArgs(mod));
        }

        /// <summary>
        /// Updates the displayed mod information.
        /// </summary>
        private void UpdateInfo(Mod mod)
        {
            if (mod == null)
                return;

            ImgBanner.Source = ImageProcessing.LoadBannerImage(mod, Properties.Resources.Background);

            ShowPopupSettings.Visibility = mod.Properties.Name != "Halo Wars: Definitive Edition"
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (mod.Properties.Name != "Halo Wars: Definitive Edition")
            {
                ModSettingControl modSettingControl = new(mod, this)
                {
                    Width = 236,
                    Height = 96,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };

                modSettingControl.CurrentMod = mod;
                modSettingControl.Page = this;

                ModSettingPopUp.Child = modSettingControl;
            }

            UpdateModData(mod);
            UpdateChangelog(mod);
        }

        /// <summary>
        /// Updates the mod data including play time and last played date.
        /// </summary>
        private void UpdateModData(Mod mod)
        {
            string dataPath = mod.Properties.DataPath;
            InGameTimer data = new();

            if (!File.Exists(dataPath))
            {
                data.ToFile(dataPath);
                return;
            }

            data.FromFile(dataPath);
            playTimeControl.UpdatePlayTime(data.Data.PlayTime);
            lastPlayedControl.UpdateLastPlayedDate(data.Data.LastPlayed);
        }

        /// <summary>
        /// Updates the changelog display or uses the mod description as a fallback.
        /// </summary>
        private static void UpdateChangelog(Mod mod)
        {
            string changelogPath = mod.Properties.ChangelogPath;

            if (!string.IsNullOrEmpty(changelogPath) && File.Exists(changelogPath))
            {
                // Load and display the changelog
            }
            else if (!string.IsNullOrEmpty(mod.Properties.Description))
            {
                // Display the description as a fallback
            }
        }

        #region Event Handlers

        /// <summary>
        /// Event handler to launch the game with the selected mod.
        /// </summary>
        private void LaunchGame(object sender, RoutedEventArgs e)
        {
            if (SelectedModControl == null) return;

            string modName = SelectedModControl.CurrentMod.Properties.Name;
            string modPath = SelectedModControl.CurrentMod.Properties.ModPath;

            Manifest.Content = modName == "Halo Wars: Definitive Edition" ? string.Empty : modPath;
            Main.Config.Mods.LastPlayedMod = modName == "Halo Wars: Definitive Edition" ? "Vanilla" : modName;

            Manifest.ToFile(Main.ManifestPath);
            Main.Config.ToFile(Main.ConfigPath);

            HaloWars.Instance.StartGame(Main.Config.HaloWars.CurrentDistribution, false);
            HaloWars.Instance.StartMonitoring();
        }

        private void ShowModSettings(object sender, MouseEventArgs e)
        {
            ModSettingPopUp.IsOpen = true;
        }

        private void HideModSettings(object sender, MouseEventArgs e)
        {
            ModSettingPopUp.IsOpen = false;
        }

        #endregion
    }

    /// <summary>
    /// Provides data for the ModChanged event.
    /// </summary>
    public class ModChangedEventArgs(Mod changedMod) : EventArgs
    {

        /// <summary>
        /// Gets the mod that has changed.
        /// </summary>
        public Mod ChangedMod { get; } = changedMod;
    }
}
