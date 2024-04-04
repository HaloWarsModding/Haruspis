using Newtonsoft.Json;
using System.IO;

namespace EtherealModManagerGUI
{
    public class Configuration
    {
        public AppConfig Settings { get; set; } = new AppConfig();
        public GameConfig Game { get; set; } = new GameConfig();
        public BoxesConfig Boxes { get; set; } = new BoxesConfig();

        public void Save()
        {
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Saving config to file");

            var configJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ETHPath.Data.ConfigFile, configJson);

            ETHLogging.Log(ETHLogging.LogLevel.Information, "Config saved successfully");
        }
    }

    public class AppConfig
    {
        public string Language { get; set; } = "fr_FR";
        public bool DiscordRichPresence { get; set; } = true;
    }

    public class GameConfig
    {
        public string ModsDirectory { get; set; } = ETHPath.Mods.Paths;
        public string CurrentDistribution { get; set; } = "Steam";
        public string GameExecutablePath { get; set; } = string.Empty;
    }

    public class BoxesConfig
    {
        public bool ShowWelcomeBox { get; set; } = true;
        public bool ShowDistributionBox { get; set; } = true;
    }
}
