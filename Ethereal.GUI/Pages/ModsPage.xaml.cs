using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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


            try
            {
                InitializeModList();

                ModChanged += ModsPage_ModChanged;
                timer.Interval = TimeSpan.FromSeconds(0.01);
                timer.Tick += Timer_Tick;
                timer.Start();

            }
            catch (Exception ex) { Logger.Log(LogLevel.Error, ex.Message); }
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

            if (vanillaMod.ModPath != string.Empty)
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
            int lastSelectedIndex = Core.config.Settings.LastSelectedMod;

            if (lastSelectedIndex >= ModPanel.Children.Count)
            {
                lastSelectedIndex = 0;
            }

            if (ModPanel.Children.Count > 0)
            {
                SelectedModControl = (ModControl)ModPanel.Children[lastSelectedIndex];
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
            BitmapImage bitmap = LoadBannerImage(mod);
            ImgBanner.Source = bitmap;

            UpdateModData(mod);

            UpdateChangelog(mod);
        }

        private static BitmapImage LoadBannerImage(Mod mod)
        {
            BitmapImage bitmap = new();
            string bannerPath = mod.Banner;
            string path;
            switch (bannerPath)
            {
                case "":
                    bitmap = LoadDefaultImage();
                    break;
                case string bPath when bPath.StartsWith("ModData\\"):
                    path = Path.Combine(mod.ModPath, bPath["ModData\\".Length..]);
                    bitmap = LoadImageFromFile(path);
                    break;
                case string bPath when File.Exists(bPath):
                    path = bPath;
                    bitmap = LoadImageFromFile(path);
                    break;
                default:
                    break;
            }

            return bitmap;
        }

        private static BitmapImage LoadDefaultImage()
        {
            BitmapImage bitmap = new();
            using (MemoryStream stream = new())
            {
                Properties.Resources.Background.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            return bitmap;
        }

        private static BitmapImage LoadImageFromFile(string path)
        {
            BitmapImage bitmap = new();
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
            return bitmap;
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

            TimeSpan playTime = data.Data.PlayTime;
            string playTimeString = playTime.TotalHours >= 1
                ? $"{(int)playTime.TotalHours} Hours"
                : playTime.TotalMinutes >= 1 ? $"{(int)playTime.TotalMinutes} Minutes" : "0 Hours";
            LblTime.Content = playTimeString;

            DateTime lastPlayed = data.Data.LastPlayed;
            lastPlayedControl.UpdateLastPlayedDate(lastPlayed);
        }

        private static void UpdateChangelog(Mod mod)
        {
            string changelogPath = mod.ChangelogPath;
            switch (changelogPath)
            {
                case string cPath when File.Exists(cPath):
                    break;
                default:
                    if (!string.IsNullOrEmpty(mod.Description))
                    {

                    }
                    else
                    {

                    }
                    break;
            }
        }



        #region Event Handlers
        private void LaunchGame(object sender, RoutedEventArgs e)
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


        private void ShowModSettings(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion
    }

    public class ModChangedEventArgs(Mod changedMod) : EventArgs
    {
        public Mod ChangedMod { get; } = changedMod;
    }
}
