using System.IO;
using System.Reflection;

using Ethereal.Data;
using Ethereal.GUI.Pages;

namespace Ethereal.GUI
{
    public static class Core
    {
        public static readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.xml");
        public static readonly string ManifestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Halo Wars", "ModManifest.txt");
        private static DateTime gameStartTime;

#pragma warning disable CA2211
        public static Manifest manifest = new();
        public static Configuration config = new();
        public static bool IsGameRunning = false;
#pragma warning restore CA2211

        public static void Initialize()
        {
            InitializeFolderStructure();
            InitializeConfig();
            InitializeManifest();
            InitializeDiscord();

            GameProcess.Instance.GameStarted += HandleGameStarted;
            GameProcess.Instance.GameExited += HandleGameExited;
            GameProcess.Instance.FoundProcessExecutable += HandleProcessExecutableFound;
            Logger.ErrorLogged += ErrorFound;
        }

        #region Initializations
        private static void InitializeFolderStructure()
        {
            string data = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            _ = Directory.CreateDirectory(data);

            string logs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            _ = Directory.CreateDirectory(logs);
            Logger.SetPath(logs);

            string mods = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
            _ = Directory.CreateDirectory(mods);
        }
        private static void InitializeConfig()
        {
            if (!File.Exists(configPath))
            {
                config.ToFile(configPath);
                return;
            }

            config.FromFile(configPath);
        }
        private static void InitializeManifest()
        {
            if (!File.Exists(ManifestPath) || config.Mods.CustomManifest == false)
            {
                manifest.FromFile(ManifestPath);

                config.Mods.CustomManifest = true;
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
            Utility.Instance.InitializeDiscord();
        }
        #endregion
        #region Events Handlers
        private static void HandleGameStarted(object sender, EventArgs e)
        {
            Utility.Instance.SetDiscordPresence($"Playing {config.Mods.LastPlayedMod}", "Halo Wars: Definitive Edition", "ethereal", "Ethereal: Mod Manager", "W.I.P", "https://github.com/HaloWarsModding/Ethereal");

            IsGameRunning = true;
            gameStartTime = DateTime.Now;
        }

        private static void RecordTimers()
        {
            string path = ModsPage.selectedModControl.currentMod.DataPath;
            ETHData data = new();

            if (!File.Exists(path))
            {
                data.ToFile(path);
                return;
            }

            data.FromFile(path);

            TimeSpan playTime = DateTime.Now - gameStartTime;
            data.Data.LastPlayed = DateTime.Now;
            data.Data.PlayTime += playTime;

            data.ToFile(path);
        }

        private static void HandleGameExited(object sender, EventArgs e)
        {
            Utility.Instance.ClearDiscordPresence();
            GameProcess.Instance.StopMonitoring();
            IsGameRunning = false;
            RecordTimers();
        }
        private static void HandleProcessExecutableFound(object sender, string executablePath)
        {
            config.HaloWars.Path = executablePath;
            config.ToFile(configPath);

            Utility.Instance.ClearDiscordPresence();
            GameProcess.Instance.StopMonitoring();
            IsGameRunning = false;
        }
        private static void ErrorFound(object sender, LogEventArgs e)
        {
            if (!config.Settings.SendReportOnError)
            { return; }

            if (Utility.IsInternetAvailable())
            {
                Utility.Instance.SendReport(e.ErrorMessage, Assembly.GetExecutingAssembly().GetName().Version);
            }
        }
        #endregion
    }
}
