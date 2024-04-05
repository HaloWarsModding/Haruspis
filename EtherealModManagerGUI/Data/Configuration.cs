namespace EtherealModManager
{
    public class Configuration
    {
        public AppConfig Settings { get; set; } = new AppConfig();
        public GameConfig Game { get; set; } = new GameConfig();
        public BoxesConfig Boxes { get; set; } = new BoxesConfig();
    }

    public class AppConfig
    {
        public string Language { get; set; } = "fr_FR";
        public bool DiscordRichPresence { get; set; } = true;
    }

    public class GameConfig
    {
        public string ModsDirectory { get; set; } = "TODO";
        public string CurrentDistribution { get; set; } = "Steam";
        public string GameExecutablePath { get; set; } = string.Empty;
    }

    public class BoxesConfig
    {
        public bool ShowWelcomeBox { get; set; } = true;
        public bool ShowDistributionBox { get; set; } = true;
    }
}
