using EtherealModManagerGUI.Configuration;
using EtherealModManagerGUI.Handlers;
using EtherealModManagerGUI.UI;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace EtherealModManagerGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Game.GameExited += () => Dispatcher.Invoke(() => { BtnPlay.Content = "Play"; BtnPlay.IsHitTestVisible = true; WindowState = WindowState.Normal; Topmost = true; Topmost = false; Discord.ClearPresence(); });
            Game.GameStarted += () => Dispatcher.Invoke(() => { if (EtherealConfig.Config.DiscordPresence) Discord.UpdatePresence("Playing", "Halo Wars: Definitive Edition"); });
        }

        private void BtnExitMainWindow_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void PnlDragWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (e.ChangedButton == MouseButton.Left) this.DragMove(); }
        private void TxBoxChangelog_Loaded(object sender, RoutedEventArgs e) => TxBoxChangelog.Document = Changelog.ToDocument();
        private void Window_Loaded(object sender, RoutedEventArgs e) => ShowWelcomeAndDistributionMessages();

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EtherealConfig.Config.CachedExePath))
            {
                if (PromptForGameDetection())
                {
                    Game.StartAndKill();
                    return;
                }

                string selectedFilePath = PromptForGameExecutable();
                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    EtherealConfig.Config.CachedExePath = selectedFilePath;
                    EtherealConfig.Config.Save();
                    return;
                }
            }

            if (string.IsNullOrEmpty(EtherealConfig.Config.CachedExePath))
                return;

            StartGame();
        }

        private static bool PromptForGameDetection() => new EtherealBox(EtherealBox.EtherealBoxDialog.YesNo, "Game not found", "The game was not found. The program can attempt to automatically detect the game by running it for a few seconds.", "Yes", "No").ShowDialog() == true;
        private static string PromptForGameExecutable()
        {
            OpenFileDialog fileDialog = new()
            {
                Filter = "Game Executable|*.exe",
                Title = "Select the game executable"
            };
            return fileDialog.ShowDialog() == true ? fileDialog.FileName : string.Empty;
        }
        private void StartGame() { Game.Start(); BtnPlay.Content = "Playing"; BtnPlay.IsHitTestVisible = false; WindowState = WindowState.Minimized; }

        private static void ShowWelcomeAndDistributionMessages()
        {
            if (EtherealConfig.Config.WelcomeMessage) { new EtherealBox(EtherealBox.EtherealBoxDialog.Yes, "Welcome to Ethereal Mod Manager!", "This is a simple mod manager for Halo Wars: Definitive Edition. You can use this tool to install, update, and remove mods with ease. Enjoy!", "Got it!").ShowDialog(); EtherealConfig.Config.WelcomeMessage = false; EtherealConfig.Config.Save(); }
            if (EtherealConfig.Config.DistributionMessage) { EtherealConfig.Config.Distribution = new EtherealBox(EtherealBox.EtherealBoxDialog.YesNo, "Distribution", "Please select your distributed version of the game. \nYou can change your distribution in the settings.", "Steam", "MS").ShowDialog() == true ? "Steam" : "MS"; EtherealConfig.Config.DistributionMessage = false; EtherealConfig.Config.Save(); }
        }
    }
}
