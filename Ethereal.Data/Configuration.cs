
using System.Xml.Serialization;

using Ethereal.Logging;

namespace Ethereal.Data
{
    public sealed class Configuration
    {
        #region Properties

        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public AppConfig Settings { get; set; } = new AppConfig();
        public GameConfig HaloWars { get; set; } = new GameConfig();
        public ModsConfig Mods { get; set; } = new ModsConfig();

        #endregion

        private readonly XmlSerializer _serializer = new(typeof(Configuration));

        public void ToFile(string path)
        {
            try
            {
                using StreamWriter streamWriter = new(path);
                _serializer.Serialize(streamWriter, this);
                Logger.Log(LogLevel.Information, $"Config file successfully written to: {path}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Log(LogLevel.Error, $"Access denied while writing config file: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Logger.Log(LogLevel.Error, $"Directory not found while writing config file: {ex.Message}");
            }
            catch (IOException ex)
            {
                Logger.Log(LogLevel.Error, $"IO error while writing config file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Unexpected error writing config file: {ex.Message}");
            }
        }

        public void FromFile(string path)
        {
            if (!File.Exists(path))
            {
                Logger.Log(LogLevel.Error, $"Configuration file does not exist: {path}");
                return;
            }

            try
            {
                using StreamReader streamReader = new(path);
                if (_serializer.Deserialize(streamReader) is Configuration configObject)
                {
                    Settings = configObject.Settings;
                    HaloWars = configObject.HaloWars;
                    Mods = configObject.Mods;
                    Logger.Log(LogLevel.Information, $"Config file successfully read from: {path}");
                }
                else
                {
                    Logger.Log(LogLevel.Error, $"Failed to deserialize configuration object from file: {path}");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Log(LogLevel.Error, $"Access denied while reading config file: {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                Logger.Log(LogLevel.Error, $"File not found while reading config file: {ex.Message}");
            }
            catch (IOException ex)
            {
                Logger.Log(LogLevel.Error, $"IO error while reading config file: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Logger.Log(LogLevel.Error, $"Invalid operation while reading config file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Unexpected error reading config file: {ex.Message}");
            }
        }

        public class AppConfig
        {
            public string Language { get; set; } = "en_GB";
            public int LastSelectedMod { get; set; } = 0;
            public bool DiscordRP { get; set; } = true;
            public bool SendReportOnError { get; set; } = false;
        }

        public class GameConfig
        {
            public string Path { get; set; } = string.Empty;
            public DistributionType CurrentDistribution { get; set; } = DistributionType.Steam;
        }

        public class ModsConfig()
        {
            public string Path { get; set; } = System.IO.Path.Combine(BaseDirectory, "mods");
            public bool CustomManifest { get; set; } = false;
            public string LastPlayedMod { get; set; } = "Vanilla";
        }

        public enum DistributionType
        {
            Steam,
            MicrosoftStore
        }
    }

    public class AppConfig
    {
        public string Language { get; set; } = "en_GB";
        public int LastSelectedMod { get; set; } = 0;
        public bool DiscordRP { get; set; } = true;
        public bool SendReportOnError { get; set; } = false;
    }

    public class GameConfig
    {
        public string Path { get; set; } = string.Empty;
        public DistributionType CurrentDistribution { get; set; } = DistributionType.Steam;
    }

    public class ModsConfig(string baseDirectory)
    {
        public string Path { get; set; } = System.IO.Path.Combine(baseDirectory, "mods");
        public bool CustomManifest { get; set; } = false;
        public string LastPlayedMod { get; set; } = "Vanilla";
    }

    public enum DistributionType
    {
        Steam,
        MicrosoftStore
    }
}



