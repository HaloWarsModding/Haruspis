//-----------------------------------------------------------------------------
// File: Configuration.cs
// Description: Defines the Configuration class for managing application settings.
//-----------------------------------------------------------------------------

using Ethereal.Logging;
using System.Xml.Serialization;

namespace Ethereal.Config
{
    public sealed class Configuration
    {
        #region Properties
        private static readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public AppConfig Settings { get; set; } = new AppConfig();
        public GameConfig HaloWars { get; set; } = new GameConfig();
        public ModsConfig Mods { get; set; } = new ModsConfig();

        public class AppConfig
        {
            public string Language { get; set; } = "en_GB";
            public bool DiscordRP { get; set; } = true;

            public bool ShowWelcomeDialog { get; set; } = true;
            public bool ShowDistributionDialog { get; set; } = true;
            public bool ShowGameDetectionDialog { get; set; } = true;
        }

        public class GameConfig
        {
            public string Path { get; set; } = string.Empty;
            public DistributionType CurrentDistribution { get; set; } = DistributionType.Steam;
        }

        public class ModsConfig
        {
            public string Path { get; set; } = System.IO.Path.Combine(baseDirectory, "mods");
            public bool customManifest { get; set; } = false;
            public string LastPlayedMod { get; set; } = "Vanilla";
        }

        public enum DistributionType
        {
            Steam,
            MicrosoftStore
        }
        #endregion

        private readonly XmlSerializer serializer = new(typeof(Configuration));

        public void ToFile(string path)
        {
            try
            {
                using (StreamWriter streamWriter = new(path))
                {
                    serializer.Serialize(streamWriter, this);
                }

                Logger.GetInstance().Log(LogLevel.Information, "Config file successfully written to: " + path);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(LogLevel.Error, "Error writing config file: " + ex.Message);
            }
            finally
            {
                Logger.GetInstance().Log(LogLevel.Debug, "WriteConfigFile method finished executing");
            }
        }

        public void FromFile(string path)
        {
            if (!ConfigFileExists(path))
            {
                Logger.GetInstance().Log(LogLevel.Error, "Configuration file does not exist: " + path);
                return;
            }

            try
            {
                using (StreamReader streamReader = new(path))
                {
                    object? deserializedObject = serializer.Deserialize(streamReader);
                    if (deserializedObject is Configuration configObject)
                    {
                        Settings = configObject.Settings;
                        HaloWars = configObject.HaloWars;
                        Mods = configObject.Mods;
                    }
                    else
                    {
                        Logger.GetInstance().Log(LogLevel.Error, "Failed to deserialize configuration object from file: " + path);
                    }
                }

                Logger.GetInstance().Log(LogLevel.Information, "Config file successfully read from: " + path);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(LogLevel.Error, "Error reading config file: " + ex.Message);
            }
            finally
            {
                Logger.GetInstance().Log(LogLevel.Debug, "ReadConfigFile method finished executing");
            }
        }

        private bool ConfigFileExists(string path)
        {
            return File.Exists(path);
        }
    }
}