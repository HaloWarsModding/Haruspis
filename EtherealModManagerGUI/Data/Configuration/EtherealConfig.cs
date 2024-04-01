using EtherealModManagerGUI.Logging;
using EtherealModManagerGUI.Paths;
using Newtonsoft.Json;
using System.IO;

namespace EtherealModManagerGUI.Configuration
{
    internal class EtherealConfig
    {
        private static Configuration _config;

        public static Configuration Config
        {
            get
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Accessing Config property");
                _config ??= LoadConfig();
                return _config;
            }
        }

        private static Configuration LoadConfig()
        {
            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Starting to load config");

            if (!File.Exists(EtherealPaths.ConfigFile))
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Warning, "Config file does not exist, creating default config");
                return CreateDefaultConfig();
            }

            try
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Config file exists, loading existing config");
                return LoadExistingConfig();
            }
            catch (Exception ex)
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Error, $"Failed to load config: {ex.Message}");
                EtherealLogging.Log(EtherealLogging.LogLevel.Warning, "Creating default config due to error");
                return CreateDefaultConfig();
            }
        }

        public static void SaveConfig()
        {
            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Saving config to file");

            var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);
            File.WriteAllText(EtherealPaths.ConfigFile, configJson);

            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Config saved successfully");
        }

        private static Configuration LoadExistingConfig()
        {
            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Reading config file");
            var configJson = File.ReadAllText(EtherealPaths.ConfigFile);

            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Deserializing config");
            _config = JsonConvert.DeserializeObject<Configuration>(configJson);

            LogConfigProperties();

            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Config loaded successfully");
            return _config;
        }

        private static Configuration CreateDefaultConfig()
        {
            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Creating default config");

            _config = new Configuration();
            var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);

            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Writing default config to file");
            File.WriteAllText(EtherealPaths.ConfigFile, configJson);

            LogConfigProperties();

            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Config file created with default values");
            return _config;
        }

        private static void LogConfigProperties()
        {
            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Logging config properties");

            foreach (var prop in typeof(Configuration).GetProperties())
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Information, $"{prop.Name}: {prop.GetValue(_config)}");
            }
        }

        public class Configuration
        {
            public bool WelcomeMessage { get; set; } = true;
            public bool DistributionMessage { get; set; } = true;
            public string Distribution { get; set; } = "Steam";
            public bool AutoUpdate { get; set; } = true;
            public bool AutoUpdateMods { get; set; } = true;
            public string CachedExePath { get; set; } = string.Empty;
            public bool DiscordPresence { get; set; } = true;

            public void Save()
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Saving config to file");

                var configJson = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(EtherealPaths.ConfigFile, configJson);

                EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Config saved successfully");
            }
        }
    }
}
