using Ethereal.GUI.Box;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Ethereal.GUI
{
    public partial class MainWindow : Window
    {
        public string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.xml");
        public static Configuration config = new();

        public MainWindow()
        {
            InitializeComponent();
            InitializeFolderStructure();
            InitializeConfig();
            InitializeDiscord();
            InitializeChangelog();

            ShowWelcomeBox();
            ShowDistributionBox();
            ShowGameDetectionBox();

            HWProcess.GetInstance().GameStarted += HandleGameStarted;
            HWProcess.GetInstance().GameExited += HandleGameExited;
            HWProcess.GetInstance().FoundProcessExecutable += HandleProcessExecutableFound;
        }

        #region Boxes

        private void ShowGameDetectionBox()
        {
            if (!config.Settings.ShowGameDetectionDialog)
            {
                return;
            }

            EtherealBox box = new(EtherealBox.EtherealBoxDialog.YesNo, Properties.Resources.GameDetectionTitle, Properties.Resources.GameDetectionContent, Properties.Resources.GameDetectionYes, Properties.Resources.GameDetectionNo);

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

            EtherealBox box = new(EtherealBox.EtherealBoxDialog.YesNo, Properties.Resources.DistributionTitle, Properties.Resources.DistributionContent, Properties.Resources.DistributionYes, Properties.Resources.DistributionNo);
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

            EtherealBox box = new(EtherealBox.EtherealBoxDialog.Yes, Properties.Resources.WelcomeTitle, Properties.Resources.WelcomeContent, Properties.Resources.WelcomeYes);
            if (box.ShowDialog().Value == true)
            {
                config.Settings.ShowWelcomeDialog = false;
            }

            config.ToFile(configPath);
        }

        #endregion

        #region Initializations

        private void InitializeDiscord()
        {
            Utility.GetInstance().InitializeDiscord();
        }

        private void InitializeChangelog()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "changelog.md");
            string changelog = File.ReadAllText(path);
            LblChangelog.Content = Utility.GetInstance().MarkdownToFlowDocument(changelog);
        }

        private void InitializeFolderStructure()
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

        #endregion
    }
}