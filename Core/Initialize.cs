using Data;

using Discord;

using Process;

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
            RecordTimers(Main.Config.HaloWars.Path);
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
        private static void RecordTimers(string path)
        {
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
