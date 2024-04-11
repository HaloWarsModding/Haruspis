using System.IO;

namespace Ethereal.ModManager.Data
{
    public class Configuration
    {
        public AppConfig Settings { get; set; } = new AppConfig();
        public GameConfig Game { get; set; } = new GameConfig();
        public BoxesConfig Boxes { get; set; } = new BoxesConfig();
    }

    public class AppConfig
    {
        public string Language { get; set; } = "en_GB";
        public bool DiscordRichPresence { get; set; } = true;
    }

    public class GameConfig
    {
        private static readonly string baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public string ModsDirectory { get; set; } = Path.Combine(baseDirectory, "mods");
        public string CurrentDistribution { get; set; } = "Steam";
        public string GameExecutablePath { get; set; } = string.Empty;
        public bool CustomManifest { get; set; } = false;
    }

    public class BoxesConfig
    {
        public bool ShowWelcomeBox { get; set; } = true;
        public bool ShowDistributionBox { get; set; } = true;
    }
}
