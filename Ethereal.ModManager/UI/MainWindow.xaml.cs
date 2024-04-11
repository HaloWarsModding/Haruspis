using Ethereal.Core;
using Ethereal.Core.Configuration;
using Ethereal.Core.HaloWars;
using Ethereal.Core.Logging;
using Ethereal.Core.Utils;
using Ethereal.ModManager.Data;
using Ethereal.ModManager.Dialogs;
using Ethereal.ModManager.UI;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Ethereal.ModManager
{
    public partial class MainWindow : Window
    {
        public static Configuration CurrentConfiguration = new();
        public string Changelog;
        private static readonly string baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public static LogWriter logWriter = new(LogLevel.Verbose, Path.Combine(baseDirectory, "data", "logs"), "EtherealModManager", 2);
        public static LogReader logReader;
        public static DynamicProperties dynamicProperties;
        public static ConfigWriter configWriter;
        public static ConfigReader configReader;
        public static DiscordRichPresence discord;
        public static GameProcess gameProcess;
        public static ConvertMarkdown convertMarkdown;
        public static Manifest manifest;
        public static Manager manager;

        public MainWindow()
        {
            InitializeComponent();

            logReader = new LogReader(Path.Combine(baseDirectory, "data", "logs"), logWriter);
            dynamicProperties = new DynamicProperties();
            DynProperties.GenerateProperties(dynamicProperties);
            discord = new DiscordRichPresence("1224459522278555711", logWriter);
            convertMarkdown = new ConvertMarkdown();
            Changelog = File.ReadAllText(Path.Combine(baseDirectory, "changelog.md"));
            gameProcess = new GameProcess(logWriter);
            if (dynamicProperties.TryGetProperty("configFile", out object configPath))
            {
                configWriter = new ConfigWriter((string)configPath, logWriter);
                configReader = new ConfigReader((string)configPath, logWriter);
            }
            if (configReader.IsConfigFileValid())
            {
                _ = configReader.ReadConfigFile<Configuration>();
            }
            else
            {
                configWriter.WriteConfigFile(CurrentConfiguration);
                _ = configReader.ReadConfigFile<Configuration>();
            }

            CurrentConfiguration = (Configuration)ConfigReader.ConfigObject;

            gameProcess.GameExited += OnGameExited;
            gameProcess.GameStarted += OnGameStarted;

            CultureInfo culture = new(CurrentConfiguration.Settings.Language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (CurrentConfiguration.Boxes.ShowWelcomeBox)
            {
                EtherealBox welcomeBox = DBoxes.CreateDialogBox(DBoxType.Welcome);
                _ = welcomeBox.ShowDialog();

                CurrentConfiguration.Boxes.ShowWelcomeBox = false;
                configWriter.WriteConfigFile(CurrentConfiguration);
            }
            if (CurrentConfiguration.Boxes.ShowDistributionBox)
            {
                EtherealBox distributionBox = DBoxes.CreateDialogBox(DBoxType.Distribution);
                _ = distributionBox.ShowDialog();

                if (distributionBox.DialogResult == true)
                {
                    CurrentConfiguration.Game.CurrentDistribution = "Steam";
                    configWriter.WriteConfigFile(CurrentConfiguration);
                }
                else
                {
                    CurrentConfiguration.Game.CurrentDistribution = "MS";
                    configWriter.WriteConfigFile(CurrentConfiguration);
                }

                CurrentConfiguration.Boxes.ShowDistributionBox = false;
                configWriter.WriteConfigFile(CurrentConfiguration);
            }

            if (dynamicProperties.TryGetProperty("modManifest", out object path))
            {
                manifest = new Manifest((string)path, logWriter);
            }

            InitializeGame().Wait();

            manager = new Manager(CurrentConfiguration.Game.GameExecutablePath, manifest, logWriter);
        }

        private void OnGameStarted()
        {
            Dispatcher.Invoke(() =>
            {
                if (CurrentConfiguration.Settings.DiscordRichPresence)
                {
                    discord.UpdatePresence($"Playing {manager.CurrentMod.Name}", "Halo Wars: Definitive Edition", "ethereal", "Ethereal", "VIP", "https://github.com/HaloWarsModding/Ethereal");
                }
            });
        }

        private void OnGameExited()
        {
            Dispatcher.Invoke(() =>
            {
                ResetPlayButton();
                discord.ClearPresence();
            });
        }

        private void ResetPlayButton()
        {
            UpdateButtonContent(Properties.Resources.BtnPlay, true);
        }


        private void SetPlayingState()
        {
            UpdateButtonContent(Properties.Resources.BtnPlaying, false);
        }

        private void BtnModsWindow_Click(object sender, RoutedEventArgs e)
        {
            manager = new Manager(CurrentConfiguration.Game.GameExecutablePath, manifest, logWriter);

            _ = new ModsPage().ShowDialog();
        }

        private void PnlDragWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void TxBoxChangelog_Loaded(object sender, RoutedEventArgs e)
        {
            TxBoxChangelog.Document = convertMarkdown.ToFlowDocument(Changelog);
        }

        private async void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            await InitializeGame();
            await gameProcess.StartGame(false, CurrentConfiguration.Game.CurrentDistribution);
            SetPlayingState();
        }

        private static async Task InitializeGame()
        {
            if (string.IsNullOrWhiteSpace(CurrentConfiguration.Game.GameExecutablePath))
            {
                EtherealBox gameNotFoundDialog = DBoxes.CreateDialogBox(DBoxType.GameNotFound);
                _ = gameNotFoundDialog.ShowDialog();

                if (gameNotFoundDialog.DialogResult == true)
                {
                    await gameProcess.StartGame(true, CurrentConfiguration.Game.CurrentDistribution);
                    manifest.Uninstall();
                    CurrentConfiguration.Game.GameExecutablePath = gameProcess.GameExecutablePath;
                    configWriter.WriteConfigFile(CurrentConfiguration);
                }
                else
                {
                    OpenFileDialog gameExecutable = DFiles.CreateDialogFile(DFileType.GameExecutable);
                    string selectedFilePath = gameExecutable.ShowDialog() == true ? gameExecutable.FileName : string.Empty;

                    if (!string.IsNullOrWhiteSpace(selectedFilePath))
                    {
                        CurrentConfiguration.Game.GameExecutablePath = selectedFilePath;
                        configWriter.WriteConfigFile(CurrentConfiguration);
                    }
                }
            }
        }

        private void UpdateButtonContent(object content, bool isHitTestVisible)
        {
            BtnPlay.Content = content;
            BtnPlay.IsHitTestVisible = isHitTestVisible;
        }

        private void BtnExitMainWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}