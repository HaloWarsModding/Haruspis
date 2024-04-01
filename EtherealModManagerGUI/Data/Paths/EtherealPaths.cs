using System.IO;

namespace EtherealModManagerGUI.Paths
{
    internal class EtherealPaths
    {
        private static readonly string baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static string DataDirectory
        {
            get
            {
                var path = Path.Combine(baseDirectory, "data");
                Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string LogDirectory
        {
            get
            {
                var path = Path.Combine(DataDirectory, "logging");
                Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string ModsDirectory
        {
            get
            {
                var path = Path.Combine(baseDirectory, "mods");
                Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string TempDirectory
        {
            get
            {
                var path = Path.Combine(baseDirectory, "temp");
                Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string ProgramVersion => DateTime.Now.ToString("yyyy.MM.dd");

        public static string DiscordInviteLink => "https://discord.gg/nFB89Wu5n9";

        public static string ConfigFile => Path.Combine(DataDirectory, "config.json");
        public static string ChangelogFile => Path.Combine(baseDirectory, "Changelog.md");

        public static string LocalHaloWars => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Halo Wars");
        public static string ModManifest => Path.Combine(LocalHaloWars, "ModManifest.txt");
        public static string MicrosoftStoreDistribution => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", "Microsoft.BulldogThreshold_8wekyb3d8bbwe", "LocalState");
    }
}
