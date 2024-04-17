using Ethereal.GUI.Pages;
using Ookii.Dialogs.Wpf;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Ethereal.GUI
{
    public partial class MainWindow : Window
    {
        public string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.xml");
        private readonly string ManifestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Halo Wars", "ModManifest.txt");

        public static Manifest manifest = new();
        public static Configuration config = new();

        public MainWindow()
        {
            InitializeComponent();
            InitializeFolderStructure();
            InitializeConfig();
            InitializeDiscord();
            InitializeChangelog();
            InitializeManifest();

            ShowWelcomeBox();
            ShowDistributionBox();
            ShowGameDetectionBox();

            HWProcess.GetInstance().GameStarted += HandleGameStarted;
            HWProcess.GetInstance().GameExited += HandleGameExited;
            HWProcess.GetInstance().FoundProcessExecutable += HandleProcessExecutableFound;
        }

        #region Initializations

        private void InitializeManifest()
        {
            if (!File.Exists(ManifestPath) || config.Mods.customManifest == false)
            {
                manifest.FromFile(ManifestPath);

                config.Mods.customManifest = true;
                config.ToFile(configPath);
                return;
            }

            manifest.FromFile(ManifestPath);

            Mod mod = new();
            mod.FromPath(manifest.Content);

            config.Mods.LastPlayedMod = mod.Name;
            config.ToFile(configPath);
        }

        private static void InitializeDiscord()
        {
            Utility.GetInstance().InitializeDiscord();
        }

        private void InitializeChangelog()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "changelog.md");
            string changelog = File.ReadAllText(path);
            FlowDocument document = Utility.GetInstance().MarkdownToFlowDocument(changelog);
            TxBoxChangelog.Document = document;
        }

        private static void InitializeFolderStructure()
        {
            string data = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            _ = Directory.CreateDirectory(data);

            string logs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            _ = Directory.CreateDirectory(logs);
            Logger.GetInstance().SetPath(logs);

            string mods = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
            _ = Directory.CreateDirectory(mods);
        }

        private void InitializeConfig()
        {
            if (!File.Exists(configPath))
            {
                config.ToFile(configPath);
                return;
            }

            config.FromFile(configPath);
        }

        #endregion

        #region Boxes

        private void ShowGameDetectionBox()
        {
            if (!config.Settings.ShowGameDetectionDialog)
            {
                return;
            }

            EtherealBox box = new(EtherealBox.EtherealBoxDialog.YesNo, "Game Detection", "Ethereal can attempt to automatically detect the game by running it for a few seconds. \n(This feature is experimental and may not work)", "Automatic", "Manual");

            if (box.ShowDialog().Value == true)
            {
                HWProcess.GetInstance().StartGame(config.HaloWars.CurrentDistribution, true);
                HWProcess.GetInstance().StartMonitoring();
            }
            else
            {
                VistaOpenFileDialog dialog = new()
                {
                    Title = "Select your xgameFinal.exe for HWDE",
                    DefaultExt = "HWDE xgameFinal (*.exe)|*.exe",
                    Filter = "HWDE xgameFinal (*.exe)|*.exe"
                };

                if (dialog.ShowDialog().Value == true)
                {
                    config.HaloWars.Path = dialog.FileName;
                }
                else
                {
                    _ = MessageBox.Show("This operation is needed for the program to function, please select your HWDE xgameFinal file.", "Attention Needed", MessageBoxButton.OK, MessageBoxImage.Error);

                    ShowGameDetectionBox();
                }
            }

            config.Settings.ShowGameDetectionDialog = false;
            config.ToFile(configPath);
        }

        private void ShowDistributionBox()
        {
            if (!config.Settings.ShowDistributionDialog)
            {
                return;
            }

            EtherealBox box = new(EtherealBox.EtherealBoxDialog.YesNo, "Distribution", "Please select your distributed version of the game. \nYou can change your distribution in the settings.", "Steam", "MS");

            if (box.ShowDialog().Value == true)
            {
                config.Settings.ShowDistributionDialog = false;
                config.HaloWars.CurrentDistribution = Configuration.DistributionType.Steam;
            }
            else
            {
                config.Settings.ShowDistributionDialog = false;
                config.HaloWars.CurrentDistribution = Configuration.DistributionType.MicrosoftStore;
            }

            config.ToFile(configPath);
        }

        private void ShowWelcomeBox()
        {
            if (!config.Settings.ShowWelcomeDialog)
            {
                return;
            }

            EtherealBox box = new(EtherealBox.EtherealBoxDialog.Yes, "Welcome Helljumper!", "Ethereal is a mod manager for Halo Wars: Definitive Edition. \nYou can use this tool to install, update, and remove mods with ease. Enjoy!", "Got it!");
            if (box.ShowDialog().Value == true)
            {
                config.Settings.ShowWelcomeDialog = false;
            }

            config.ToFile(configPath);
        }

        #endregion

        #region Events Handlers

        private void HandleGameStarted(object sender, EventArgs e)
        {
            Utility.GetInstance().SetDiscordPresence($"Playing {config.Mods.LastPlayedMod}", "Halo Wars: Definitive Edition", "ethereal", "Ethereal: Mod Manager", "W.I.P", "https://github.com/HaloWarsModding/Ethereal");
        }

        private void HandleGameExited(object sender, EventArgs e)
        {
            Utility.GetInstance().ClearDiscordPresence();
            HWProcess.GetInstance().StopMonitoring();
        }

        private void HandleProcessExecutableFound(object sender, string executablePath)
        {
            config.HaloWars.Path = executablePath;
            config.ToFile(configPath);
        }

        #endregion

        #region UI Event Handlers

        public void SetChangelogContent(string content)
        {
            FlowDocument document = Utility.GetInstance().MarkdownToFlowDocument(content);
            TxBoxChangelog.Document = document;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ExitWindow(object sender, RoutedEventArgs e)
        {
            Utility.GetInstance().DiscordDispose();
            GC.Collect();
            Application.Current.Shutdown();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            HWProcess.GetInstance().StartGame(config.HaloWars.CurrentDistribution, false);
            HWProcess.GetInstance().StartMonitoring();
        }

        private void ShowModPage(object sender, RoutedEventArgs e)
        {
            ModsPage modsPage = new(this);
            _ = modsPage.ShowDialog();
        }

        private void ShowWorkshopPage(object sender, RoutedEventArgs e)
        {
            WorkshopPage workshopPage = new();
            _ = workshopPage.ShowDialog();
        }

        private void ShowSettingsPage(object sender, RoutedEventArgs e)
        {
            SettingsPage settingsPage = new();
            _ = settingsPage.ShowDialog();
        }

        private void ShowHelp(object sender, RoutedEventArgs e)
        {
            _ = Process.Start("cmd", "/c start https://github.com/HaloWarsModding/Ethereal/wiki");
        }

        #endregion
    }
}