using System;
using System.IO;
using System.Reflection;

using Ethereal.Data;
using Ethereal.GUI.Pages;

namespace Ethereal.GUI
{
    public static class Core
    {
        public static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.xml");
        public static readonly string ManifestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Halo Wars", "ModManifest.txt");
        private static DateTime _gameStartTime;

#pragma warning disable CA2211
        public static Manifest Manifest = new();
        public static Configuration Config = new();
        public static bool IsGameRunning = false;
#pragma warning restore CA2211

        public static void Initialize()
        {
            InitializeFolderStructure();
            InitializeConfig();
            InitializeManifest();
            InitializeDiscord();

            SubscribeToEvents();
        }

        #region Initializations

        private static void InitializeFolderStructure()
        {
            string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            Directory.CreateDirectory(dataDirectory);

            string logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logsDirectory);
            Logger.SetPath(logsDirectory);

            string modsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
            Directory.CreateDirectory(modsDirectory);
        }

        private static void InitializeConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                Config.ToFile(ConfigPath);
            }
            else
            {
                Config.FromFile(ConfigPath);
            }
        }

        private static void InitializeManifest()
        {
            if (!File.Exists(ManifestPath) || !Config.Mods.CustomManifest)
            {
                Manifest.FromFile(ManifestPath);
                Config.Mods.CustomManifest = true;
                Config.ToFile(ConfigPath);
            }
            else
            {
                Manifest.FromFile(ManifestPath);
                var mod = new Mod();
                mod.FromPath(Manifest.Content);
                Config.Mods.LastPlayedMod = mod.Name;
                Config.ToFile(ConfigPath);
            }
        }

        private static void InitializeDiscord()
        {
            Utility.Instance.InitializeDiscord();
        }

        #endregion

        #region Event Handlers

        private static void SubscribeToEvents()
        {
            GameProcess.Instance.GameStarted += HandleGameStarted;
            GameProcess.Instance.GameExited += HandleGameExited;
            GameProcess.Instance.FoundProcessExecutable += HandleProcessExecutableFound;
            Logger.ErrorLogged += ErrorFound;
        }

        private static void HandleGameStarted(object sender, EventArgs e)
        {
            Utility.Instance.SetDiscordPresence(
                $"Playing {Config.Mods.LastPlayedMod}",
                "Halo Wars: Definitive Edition",
                "ethereal",
                "Ethereal: Mod Manager",
                "W.I.P",
                "https://github.com/HaloWarsModding/Ethereal"
            );

            IsGameRunning = true;
            _gameStartTime = DateTime.Now;
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
            Config.HaloWars.Path = executablePath;
            Config.ToFile(ConfigPath);

            Utility.Instance.ClearDiscordPresence();
            GameProcess.Instance.StopMonitoring();
            IsGameRunning = false;
        }

        private static void ErrorFound(object sender, LogEventArgs e)
        {
            if (Config.Settings.SendReportOnError && Utility.IsInternetAvailable())
            {
                Utility.Instance.SendReport(e.ErrorMessage, Assembly.GetExecutingAssembly().GetName().Version);
            }
        }

        private static void RecordTimers()
        {
            string path = ModsPage.SelectedModControl.CurrentMod.DataPath;
            var data = new ETHData();

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

        #endregion
    }
}
