using System.IO;

namespace EtherealModManagerGUI
{
    internal class ETHPath
    {
        private static readonly string baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        static ETHPath()
        {
            Directory.CreateDirectory(Data.Paths);
            Directory.CreateDirectory(Logging.Paths);
            Directory.CreateDirectory(Mods.Paths);
            Directory.CreateDirectory(Temp.Paths);
            Directory.CreateDirectory(Data.Cache);
        }

        public static class Data
        {
            public static string Paths => Path.Combine(baseDirectory, "data");
            public static string ConfigFile => Path.Combine(Paths, "config.json");
            public static string Cache => Path.Combine(Paths, "cache");
        }

        public static class Logging
        {
            public static string Paths => Path.Combine(Data.Paths, "logging");
        }

        public static class Mods
        {
            public static string Paths => Path.Combine(baseDirectory, "mods");
        }

        public static class Temp
        {
            public static string Paths => Path.Combine(baseDirectory, "temp");
        }

        public static string ProgramVersion => DateTime.Now.ToString("yyyy.MM.dd");
        public static string DiscordInviteLink => "https://discord.gg/nFB89Wu5n9";
        public static string ChangelogFile => Path.Combine(baseDirectory, "Changelog.md");
        public static string LocalHaloWars => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Halo Wars");
        public static string ModManifest => Path.Combine(LocalHaloWars, "ModManifest.txt");
        public static string MicrosoftStoreDistribution => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", "Microsoft.BulldogThreshold_8wekyb3d8bbwe", "LocalState");
    }
}
