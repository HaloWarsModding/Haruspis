using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using Ookii.Dialogs.Wpf;

using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

using UI.Core;
using UI.Pages.Mods;

namespace UI
{
    namespace Core
    {
        public class Main
        {
            public static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.xml");
            public static readonly string ManifestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Halo Wars", "ModManifest.txt");

            public static readonly Configuration Config = new();
            public static readonly DiscordRichPresence DiscordPresence = new();
#pragma warning disable CA2211
            public static bool IsGameRunning = false;
#pragma warning restore CA2211

            static Main()
            {
                Initialize.InitializeAll();
            }
        }

        public static class Initialize
        {
            private static DateTime _gameStartTime;

            static Initialize()
            {
                InitializeAll();
            }

            /// <summary>
            /// Initialize all components and folder structure
            /// </summary>
            public static void InitializeAll()
            {
                InitializeFolderStructure();
                InitializeConfig();
                InitializeManifest();
                InitializeDiscord();
                SubscribeToEvents();
            }

            /// <summary>
            /// Initialize the necessary folder structure
            /// </summary>
            private static void InitializeFolderStructure()
            {
                string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                _ = Directory.CreateDirectory(dataDirectory);

                string modsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
                _ = Directory.CreateDirectory(modsDirectory);
            }

            /// <summary>
            /// Initialize the configuration file
            /// </summary>
            private static void InitializeConfig()
            {
                if (!File.Exists(Main.ConfigPath))
                {
                    Main.Config.ToFile(Main.ConfigPath);
                }
                else
                {
                    Main.Config.FromFile(Main.ConfigPath);
                }
            }

            /// <summary>
            /// Initialize the manifest file
            /// </summary>
            private static void InitializeManifest()
            {
                if (!File.Exists(Main.ManifestPath) || !Main.Config.Mods.CustomManifest)
                {
                    Manifest.FromFile(Main.ManifestPath);
                    Main.Config.Mods.CustomManifest = true;
                    Main.Config.ToFile(Main.ConfigPath);
                }
                else
                {
                    Manifest.FromFile(Main.ManifestPath);
                    Mod mod = new();
                    mod.FromPath(Manifest.Content);
                    Main.Config.Mods.LastPlayedMod = mod.Properties.Name;
                    Main.Config.ToFile(Main.ConfigPath);
                }
            }

            /// <summary>
            /// Initialize Discord Rich Presence
            /// </summary>
            private static void InitializeDiscord()
            {
                _ = Main.DiscordPresence.TryInitializeClient();
            }

            /// <summary>
            /// Subscribe to game events
            /// </summary>
            private static void SubscribeToEvents()
            {
                HaloWars.Instance.GameStarted += HandleGameStarted;
                HaloWars.Instance.GameExited += HandleGameExited;
                HaloWars.Instance.FoundProcessExecutable += HandleProcessExecutableFound;
            }

            /// <summary>
            /// Handle game started event
            /// </summary>
            private static void HandleGameStarted(object sender, EventArgs e)
            {
                RichPresenceButton button = new()
                {
                    Url = "https://github.com/HaloWarsModding/Haruspis",
                    Label = "Haruspis: Mod Manager"
                };

                RichPresenceData data = new()
                {
                    State = "Halo Wars: Definitive Edition",
                    Details = $"Playing {Main.Config.Mods.LastPlayedMod}",
                    LargeImageKey = "haruspis",
                    Buttons = [button]
                };

                Main.DiscordPresence.UpdatePresence(data);

                Main.IsGameRunning = true;
                _gameStartTime = DateTime.Now;
            }

            /// <summary>
            /// Handle game exited event
            /// </summary>
            private static void HandleGameExited(object sender, EventArgs e)
            {
                Main.DiscordPresence.ClearPresence();
                HaloWars.Instance.StopMonitoring();
                Main.IsGameRunning = false;
                RecordTimers();
            }

            /// <summary>
            /// Handle process executable found event
            /// </summary>
            private static void HandleProcessExecutableFound(object sender, string executablePath)
            {
                Main.Config.HaloWars.Path = executablePath;
                Main.Config.ToFile(Main.ConfigPath);

                Main.DiscordPresence.ClearPresence();
                HaloWars.Instance.StopMonitoring();
                Main.IsGameRunning = false;
            }

            /// <summary>
            /// Record game timers
            /// </summary>
            private static void RecordTimers()
            {
                string path = ModsPage.SelectedModControl.CurrentMod.Properties.DataPath;
                InGameTimer data = new();

                if (!File.Exists(path))
                {
                    data.ToFile(path);
                    return;
                }

                data.FromFile(path);

                TimeSpan playTime = DateTime.Now - _gameStartTime;
                data.Data.LastPlayed = DateTime.Now;
                data.Data.PlayTime += playTime;

                data.ToFile(path);
            }
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize.InitializeAll();
            CultureInfo culture = new(Main.Config.Settings.Language);
            LoadLanguage(culture);
            LblVersion.Content = $"v{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}.{Assembly.GetExecutingAssembly().GetName().Version.Build}";
        }

        private void LoadLanguage(CultureInfo culture)
        {
            ResourceDictionary dict = [];
            switch (culture.TwoLetterISOLanguageName)
            {
                case "fr":
                    dict.Source = new Uri("..\\Properties\\Resources.fr.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Properties\\Resources.xaml", UriKind.Relative);
                    break;
            }
            Resources.MergedDictionaries.Add(dict);
        }

        public void SwitchLanguage(string cultureCode)
        {
            CultureInfo culture = new(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            LoadLanguage(culture);
        }

        /// <summary>
        /// Handles the dragging of the window to move it.
        /// </summary>
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Minimizes the window.
        /// </summary>
        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Toggles between maximizing and restoring the window.
        /// </summary>
        private void MaximizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            Main.DiscordPresence.Dispose();
            GC.Collect();
            Application.Current.Shutdown();
        }

        private async void AddMod(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult choice = MessageBox.Show("Do you want to select an archive (zip/rar)?", "Select Type", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

            if (choice == MessageBoxResult.Cancel)
            {
                return;
            }

            string sourcePath = null;
            bool isArchive = false;

            if (choice == MessageBoxResult.Yes)
            {
                OpenFileDialog fileDialog = new()
                {
                    Multiselect = false,
                    Filter = "Zip Files|*.zip|Rar Files|*.rar",
                    ValidateNames = true,
                    CheckFileExists = true,
                    CheckPathExists = true
                };

                bool? result = fileDialog.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    sourcePath = fileDialog.FileName;
                    isArchive = true;
                }
            }
            else if (choice == MessageBoxResult.No)
            {
                VistaFolderBrowserDialog folderDialog = new() { Multiselect = false };
                bool? result = folderDialog.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    sourcePath = folderDialog.SelectedPath;
                }
            }

            if (!string.IsNullOrEmpty(sourcePath))
            {
                MainProgressBar.Visibility = Visibility.Visible;
                MainProgressBarLabel.Content = "Importing Mod...";
                Progress<double> progress = new(value =>
                {
                    _ = Dispatcher.Invoke(() => MainProgressBar.Value = value);
                });

                string destinationFolder = Main.Config.Mods.Path;

                await Task.Run(async () =>
                {
                    if (isArchive)
                    {
                        string fileExtension = Path.GetExtension(sourcePath).ToLower();
                        if (fileExtension == ".zip")
                        {
                            string extractPath = Path.Combine(Path.GetDirectoryName(sourcePath), Path.GetFileNameWithoutExtension(sourcePath));
                            ZipFile.ExtractToDirectory(sourcePath, extractPath);
                            File.Delete(sourcePath);
                            sourcePath = extractPath;
                        }
                        else if (fileExtension == ".rar")
                        {
                            string extractPath = Path.Combine(Path.GetDirectoryName(sourcePath), Path.GetFileNameWithoutExtension(sourcePath));
                            using (RarArchive archive = RarArchive.Open(sourcePath))
                            {
                                foreach (RarArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                {
                                    entry.WriteToDirectory(extractPath, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                                }
                            }
                            File.Delete(sourcePath);
                            sourcePath = extractPath;
                        }
                    }


                    if (Directory.Exists(sourcePath))
                    {
                        await FolderMover.MoveFolderAsync(sourcePath, destinationFolder, progress);
                    }
                });

                if (MainFrame.Content is ModsPage page)
                {
                    Dispatcher.Invoke(page.PopulateModList);
                }

                MainProgressBarLabel.Content = "";
                MainProgressBar.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Shows the workshop page.
        /// </summary>
        private void ShowWorkshopPage(object sender, RoutedEventArgs e)
        {
            // Implement workshop page navigation
        }

        /// <summary>
        /// Shows the settings page.
        /// </summary>
        private void ShowSettingsPage(object sender, RoutedEventArgs e)
        {
            // Uncomment and implement if Settings is defined
            // var settingsPage = new Settings(MainFrame.Content as ModsPage);
            // settingsPage.Closed += (s, args) => settingsPage = null;
            // settingsPage.ShowDialog();
        }
    }
}
