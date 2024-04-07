using EtherealEngine;
using EtherealEngine.HaloWars;
using Ethereal.ModManager.Dialogs;
using Ethereal.ModManager.UI;
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
                configReader.ReadConfigFile<Configuration>();
            }
            else
            {
                configWriter.WriteConfigFile(CurrentConfiguration);
                configReader.ReadConfigFile<Configuration>();
            }

            CurrentConfiguration = (Configuration)ConfigReader.ConfigObject;

            GameProcess.GameExited += OnGameExited;
            GameProcess.GameStarted += OnGameStarted;

            var culture = new CultureInfo(CurrentConfiguration.Settings.Language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (CurrentConfiguration.Boxes.ShowWelcomeBox)
            {
                var welcomeBox = DBoxes.CreateDialogBox(DBoxType.Welcome);
                welcomeBox.ShowDialog();

                CurrentConfiguration.Boxes.ShowWelcomeBox = false;
                configWriter.WriteConfigFile(CurrentConfiguration);
            }
            if (CurrentConfiguration.Boxes.ShowDistributionBox)
            {
                var distributionBox = DBoxes.CreateDialogBox(DBoxType.Distribution);
                distributionBox.ShowDialog();

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
        }

        private void OnGameStarted()
        {
            Dispatcher.Invoke(() =>
            {
                if (CurrentConfiguration.Settings.DiscordRichPresence)
                {
                    discord.UpdatePresence($"Playing ", "Halo Wars: Definitive Edition", "ethereal", "Ethereal", "VIP", "https://github.com/HaloWarsModding/Ethereal");
                }
            });
        }

        private void OnGameExited()
        {
            Dispatcher.Invoke(() =>
            {
                ResetPlayButton();
                ResetWindowState();
                discord.ClearPresence();
            });
        }

        private void ResetPlayButton()
        {
            UpdateButtonContent(Properties.Resources.BtnPlay, true);
        }

        private void ResetWindowState()
        {
            SetWindowState(WindowState.Normal, true, false);
        }

        private void SetPlayingState()
        {
            UpdateButtonContent(Properties.Resources.BtnPlaying, false);
            SetWindowState(WindowState.Minimized, false, true);
        }

        private void BtnModsWindow_Click(object sender, RoutedEventArgs e) => ShowModsPage();

        private static void ShowModsPage() => new ModsPage().ShowDialog();

        private void PnlDragWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void TxBoxChangelog_Loaded(object sender, RoutedEventArgs e) => TxBoxChangelog.Document = convertMarkdown.ToFlowDocument(Changelog);

        private async void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentConfiguration.Game.GameExecutablePath))
            {
                var gameNotFoundDialog = DBoxes.CreateDialogBox(DBoxType.GameNotFound);
                gameNotFoundDialog.ShowDialog();

                if (gameNotFoundDialog.DialogResult == true)
                {
                    await gameProcess.StartGame(true, CurrentConfiguration.Game.CurrentDistribution);

                    CurrentConfiguration.Game.GameExecutablePath = GameProcess.GameExecutablePath;
                    configWriter.WriteConfigFile(CurrentConfiguration);
                    return;
                }

                var gameExecutable = DFiles.CreateDialogFile(DFileType.GameExecutable);
                var selectedFilePath = gameExecutable.ShowDialog() == true ? gameExecutable.FileName : string.Empty;

                if (!string.IsNullOrWhiteSpace(selectedFilePath))
                {
                    CurrentConfiguration.Game.GameExecutablePath = selectedFilePath;
                    configWriter.WriteConfigFile(CurrentConfiguration);
                }
            }
            else
            {
                StartGame();
            }
        }

        private async void StartGame()
        {
            await gameProcess.StartGame(false, CurrentConfiguration.Game.CurrentDistribution);
            SetPlayingState();
        }

        private void UpdateButtonContent(object content, bool isHitTestVisible)
        {
            BtnPlay.Content = content;
            BtnPlay.IsHitTestVisible = isHitTestVisible;
        }

        private void SetWindowState(WindowState windowState, bool isTopmost, bool isNormal)
        {
            WindowState = windowState;
            Topmost = isTopmost;
            Topmost = isNormal;
        }

        private void BtnExitMainWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}