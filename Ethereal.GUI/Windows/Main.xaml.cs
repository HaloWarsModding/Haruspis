using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using Ethereal.GUI.Pages;
using Ethereal.GUI.Pages.UserControls;

using Octokit;

using Ookii.Dialogs.Wpf;

using static Ethereal.GUI.Pages.UserControls.NotificationControl;

namespace Ethereal.GUI
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            Core.Initialize();
            LblVersion.Content = $"v{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}.{Assembly.GetExecutingAssembly().GetName().Version.Build}b";

            Core.config.HaloWars.CurrentDistribution = Configuration.DistributionType.Steam;
            Core.config.ToFile(Core.configPath);
            ShowGameDetection();
            _ = CheckForUpdate();
        }

        private async Task CheckForUpdate()
        {
            try
            {
                GitHubClient github = new(new ProductHeaderValue("Ethereal"));
                IReadOnlyList<Release> releases = await github.Repository.Release.GetAll("HaloWarsModding", "Ethereal");

                if (releases.Count == 0) return;

                Release latestRelease = releases[0];
                Version latestVersion = new(latestRelease.TagName.TrimStart('v'));
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                if (latestVersion <= currentVersion) return;

                string downloadUrl = latestRelease.Assets.Count > 0 ? latestRelease.Assets[0].BrowserDownloadUrl : "";

                NotificationControl control = new(this, NotificationType.Information, "New Update Available!", latestVersion, downloadUrl);

                _ = NotificationGrid.Children.Add(control);
                BtnNotif.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.NotificationUp.GetHbitmap(),
                                                                          IntPtr.Zero,
                                                                          Int32Rect.Empty,
                                                                          BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error checking for updates: {ex.Message}");
            }
        }

        public void ClearNotifications()
        {
            NotificationGrid.Children.Clear();
        }


        private static void ShowGameDetection()
        {
            if (Core.config.HaloWars.Path != null)
            {
                return;
            }

            if (MessageBox.Show("Game Detection", "Ethereal can attempt to automatically detect the game by running it for a few seconds. \n(This feature is experimental and may not work)", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                GameProcess.Instance.StartGame(Core.config.HaloWars.CurrentDistribution, true);
                GameProcess.Instance.StartMonitoring();
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
                    Core.config.HaloWars.Path = dialog.FileName;
                }
                else
                {
                    _ = MessageBox.Show("This operation is needed for the program to function, please select your HWDE xgameFinal file.", "Attention Needed", MessageBoxButton.OK, MessageBoxImage.Error);

                    ShowGameDetection();
                }
            }
        }



        #region Header Handler's
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void MaximizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }
        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            Utility.Instance.DiscordDispose();
            GC.Collect();
            System.Windows.Application.Current.Shutdown();
        }
        private void NotificationPopUp(object sender, MouseButtonEventArgs e)
        {
            NotificationGrid.Visibility = Visibility.Visible;
            if (NotificationGrid.Parent is UIElement parent)
                parent.PreviewMouseDown += OutsideGridClickHandler;

            BtnNotif.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.Notification.GetHbitmap(),
                                                          IntPtr.Zero,
                                                          Int32Rect.Empty,
                                                          BitmapSizeOptions.FromEmptyOptions());

        }
        private void OutsideGridClickHandler(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(NotificationGrid);
            bool isOutsideGrid = NotificationGrid.InputHitTest(clickPoint) == null;

            if (isOutsideGrid)
            {
                NotificationGrid.Visibility = Visibility.Collapsed;
                if (NotificationGrid.Parent is UIElement parent)
                    parent.PreviewMouseDown -= OutsideGridClickHandler;
            }
        }
        #endregion
        #region Footer Handler's
        private void AddMod(object sender, MouseButtonEventArgs e)
        {
            _ = MessageBox.Show("TO-DO");
        }
        #endregion
        #region Pages Handler's
        private void ShowModsPage(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is not ModsPage)
            {
                MainFrame.Content = new ModsPage();
            }
        }
        private void ShowWorkshopPage(object sender, RoutedEventArgs e)
        {

        }
        private void ShowSettingsPage(object sender, RoutedEventArgs e)
        {
            _ = MessageBox.Show("TO-DO");
        }
        #endregion
    }
}
