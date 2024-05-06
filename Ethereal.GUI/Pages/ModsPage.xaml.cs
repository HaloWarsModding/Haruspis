using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Ethereal.GUI.Pages.Mods.UserControls;

using Image = System.Drawing.Image;

namespace Ethereal.GUI.Pages
{
    public partial class ModsPage : Page
    {
        public static event EventHandler<ModChangedEventArgs> ModChanged;
        public readonly List<Mod> Mods = [];
        public static ModControl selectedModControl;

        private readonly DispatcherTimer timer = new();
        private bool wasGameRunning = false;

        public ModsPage()
        {
            InitializeComponent();
            InitializeModList();

            ModChanged += ModsPage_ModChanged;
            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            BtnStart.IsEnabled = !Core.IsGameRunning;
            BtnStart.Content = Core.IsGameRunning ? "Launched" : "Launch";

            if (wasGameRunning != Core.IsGameRunning)
            {
                if (wasGameRunning && !Core.IsGameRunning)
                {
                    UpdateInfo(SelectedModControl.currentMod);
                }

                wasGameRunning = Core.IsGameRunning;
            }
        }

        private void ModsPage_ModChanged(object sender, ModChangedEventArgs e)
        {
            UpdateInfo(e.ChangedMod);
        }

        public async void InitializeModList()
        {
            ModPanel.Children.Clear();
            Mods.Clear();

            Mod vanillaMod = new()
            {
                Name = "Halo Wars: Definitive Edition",
                Description = "The Spirit of Fire is sent to the ruined planet Harvest to investigate Covenant activity, where Cutter learns that the Covenant has excavated something at the planet's northern pole. When the UNSC's main outpost on Harvest is captured, Cutter orders Forge to retake it. Soon after, Forge scouts the Covenant excavation and discovers that they, under the direction of the Arbiter, have discovered a Forerunner facility. Forge's troops defeat the Covenant forces before they can destroy the installation, and Anders arrives. She determines that the facility is an interstellar map, and recognizes a set of coordinates that points to the human colony of Arcadia.",
                Author = "Ensemble Studios",
                ModPath = Core.config.HaloWars.Path,

            };

            if (vanillaMod.ModPath != null)
                vanillaMod.DataPath = Path.Combine(Path.GetDirectoryName(vanillaMod.ModPath), "data.eth");


            Mods.Add(vanillaMod);
            SelectedModControl = null;

            await Task.Run(() =>
            {
                foreach (string directoryPath in Directory.EnumerateDirectories(Core.config.Mods.Path))
                {
                    Mod mod = new();
                    mod.FromPath(directoryPath);

                    if (mod.IsMod)
                    {
                        Mods.Add(mod);
                    }
                }
            });

            foreach (Mod mod in Mods)
            {
                ModControl modBox = new(mod);
                modBox.MouseDown += ModBox_MouseDown;
                _ = ModPanel.Children.Add(modBox);
            }

            if (ModPanel.Children.Count >= Core.config.Settings.LastSelectedMod)
            {
                SelectedModControl = (ModControl)ModPanel.Children[Core.config.Settings.LastSelectedMod];
                SelectedModControl.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x6C, 0x75, 0x7D));
                SetCurrentMod(SelectedModControl.currentMod);
            }
        }


        private void ModBox_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ModControl clickedModControl = (ModControl)sender;

            if (clickedModControl != SelectedModControl)
            {
                SelectedModControl?.ResetBackgroundColor();

                clickedModControl.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x6C, 0x75, 0x7D));

                SelectedModControl = clickedModControl;

                SetCurrentMod(clickedModControl.currentMod);
                Core.config.Settings.LastSelectedMod = ModPanel.Children.IndexOf(clickedModControl);
                Core.config.ToFile(Core.configPath);
            }
        }


        public static ModControl SelectedModControl
        {
            get => selectedModControl;
            set
            {
                selectedModControl?.ResetBackgroundColor();

                selectedModControl = value;
            }
        }

        public static void SetCurrentMod(Mod mod)
        {
            ModChanged?.Invoke(null, new ModChangedEventArgs(mod));
        }

        private void UpdateInfo(Mod mod)
        {
            BitmapImage bitmap = new();
            string path = "";

            switch (mod.Banner)
            {
                case "":
                    using (MemoryStream stream = new())
                    {
                        Properties.Resources.Background.Save(stream, ImageFormat.Png);
                        stream.Position = 0;
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                    break;
                case string bannerPath when bannerPath.StartsWith("ModData\\"):
                    path = Path.Combine(mod.ModPath, bannerPath["ModData\\".Length..]);
                    goto case "file";
                case string bannerPath when File.Exists(bannerPath):
                    path = bannerPath;
                    goto case "file";
                case "file":
                    if (!string.IsNullOrEmpty(path))
                    {
                        using Image image = Image.FromFile(path);
                        using MemoryStream stream = new();
                        image.Save(stream, ImageFormat.Png);
                        stream.Position = 0;
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                    break;
                default:
                    using (MemoryStream stream = new())
                    {
                        Properties.Resources.Background.Save(stream, ImageFormat.Png);
                        stream.Position = 0;
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                    break;
            }

            ImgBanner.Source = bitmap;

            string dataPath = mod.DataPath;
            ETHData data = new();

            if (!File.Exists(dataPath))
            {
                data.ToFile(dataPath);
                return;
            }

            data.FromFile(dataPath);

            TimeSpan playTime = data.Data.PlayTime;

            string playTimeString = playTime.TotalHours >= 1
                ? $"{(int)playTime.TotalHours} Hours"
                : playTime.TotalMinutes >= 1 ? $"{(int)playTime.TotalMinutes} Minutes" : "0 Hours";
            LblTime.Content = playTimeString;

            DateTime lastPlayed = data.Data.LastPlayed;
            string lastPlayedString = lastPlayed == DateTime.MinValue
                ? "Never"
                : lastPlayed.Date == DateTime.Today
                    ? "Today"
                    : lastPlayed.Date == DateTime.Today.AddDays(-1)
                        ? "Yesterday"
                        : lastPlayed.ToString("d MMMM");
            LblDate.Content = lastPlayedString;

        }

        #region Event Handlers
        private void LaunchGame(object sender, System.Windows.RoutedEventArgs e)
        {
            string modName = SelectedModControl.currentMod.Name;
            string modPath = SelectedModControl.currentMod.ModPath;

            Core.manifest.Content = modName == "Halo Wars: Definitive Edition" ? string.Empty : modPath;
            Core.config.Mods.LastPlayedMod = modName == "Halo Wars: Definitive Edition" ? "Vanilla" : modName;

            Core.manifest.ToFile(Core.ManifestPath);
            Core.config.ToFile(Core.configPath);

            GameProcess.Instance.StartGame(Core.config.HaloWars.CurrentDistribution, false);
            GameProcess.Instance.StartMonitoring();
        }


        private void ShowModSettings(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
        #endregion
    }

    public class ModChangedEventArgs(Mod changedMod) : EventArgs
    {
        public Mod ChangedMod { get; } = changedMod;
    }
}
