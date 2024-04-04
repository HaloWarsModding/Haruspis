using EtherealModManagerGUI.Data.Dialogs;
using EtherealModManagerGUI.Handlers;
using EtherealModManagerGUI.UI;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace EtherealModManagerGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Game.GameExited += OnGameExited;
            Game.GameStarted += OnGameStarted;
        }

        private void OnGameExited()
        {
            Dispatcher.Invoke(() =>
            {
                ResetPlayButton();
                ResetWindowState();
                Discord.ClearPresence();
            });
        }

        private void ResetPlayButton()
        {
            BtnPlay.Content = Properties.Resources.BtnPlay;
            BtnPlay.IsHitTestVisible = true;
        }

        private void ResetWindowState()
        {
            WindowState = WindowState.Normal;
            Topmost = true;
            Topmost = false;
        }

        private void OnGameStarted()
        {
            Dispatcher.Invoke(() =>
            {
                UpdateDiscordPresence();
            });
        }

        private static void UpdateDiscordPresence()
        {
            if (ETHConfig.CurrentConfiguration.Settings.DiscordRichPresence)
            {
                var modName = ETHManager.currentMod.Name;
                Discord.UpdatePresence($"Playing {modName}", "Halo Wars: Definitive Edition");
            }
        }

        private void BtnExitMainWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PnlDragWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void TxBoxChangelog_Loaded(object sender, RoutedEventArgs e)
        {
            TxBoxChangelog.Document = Changelog.ToDocument();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ETHConfig.CurrentConfiguration.Game.GameExecutablePath))
            {
                if (DBoxes.GameNotFoundBox().ShowDialog() == true)
                {
                    Game.SilentStart();
                    return;
                }

                string selectedFilePath = DFiles.GameExecutable().ShowDialog() == true ? DFiles.GameExecutable().FileName : string.Empty; ;
                if (!string.IsNullOrWhiteSpace(selectedFilePath))
                {
                    UpdateGameExecutablePath(selectedFilePath);
                    return;
                }
            }

            StartGame();
        }

        private static void UpdateGameExecutablePath(string selectedFilePath)
        {
            ETHConfig.CurrentConfiguration.Game.GameExecutablePath = selectedFilePath;
            ETHConfig.CurrentConfiguration.Save();

            ETHManifest.Clear();
            ETHManager.TryCacheVideo();
        }

        private void BtnModsWindow_Click(object sender, RoutedEventArgs e)
        {
            ShowModsPage();
        }

        private static void ShowModsPage()
        {
            ModsPage modsPage = new();
            modsPage.ShowDialog();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            SetCulture();
            ShowWelcomeMessage();
            ShowDistributionMessage();
        }

        private static void SetCulture()
        {
            CultureInfo culture = new(ETHConfig.CurrentConfiguration.Settings.Language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        private void StartGame()
        {
            Game.Start();
            SetPlayingState();
        }

        private void SetPlayingState()
        {
            BtnPlay.Content = Properties.Resources.BtnPlaying;
            BtnPlay.IsHitTestVisible = false;
            WindowState = WindowState.Minimized;
        }

        private static void ShowWelcomeMessage()
        {
            if (ETHConfig.CurrentConfiguration.Boxes.ShowWelcomeBox)
            {
                DBoxes.Welcome().ShowDialog();

                ETHConfig.CurrentConfiguration.Boxes.ShowWelcomeBox = false;
                ETHConfig.CurrentConfiguration.Save();
            }
        }

        private static void ShowDistributionMessage()
        {
            if (ETHConfig.CurrentConfiguration.Boxes.ShowDistributionBox)
            {
                DBoxes.Distribution().ShowDialog();

                ETHConfig.CurrentConfiguration.Boxes.ShowDistributionBox = false;
                ETHConfig.CurrentConfiguration.Save();
            }
        }
    }
}
