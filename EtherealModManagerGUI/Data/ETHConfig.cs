using Newtonsoft.Json;
using System.IO;

namespace EtherealModManagerGUI
{
    internal partial class ETHConfig
    {
        private static Configuration _config;

        public static Configuration CurrentConfiguration
        {
            get
            {
                ETHLogging.Log(ETHLogging.LogLevel.Information, "Accessing Config property");
                _config ??= LoadConfig();
                return _config;
            }
        }

        private static Configuration LoadConfig()
        {
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Starting to load config");

            if (!File.Exists(ETHPath.Data.ConfigFile))
            {
                ETHLogging.Log(ETHLogging.LogLevel.Warning, "Config file does not exist, creating default config");
                return CreateDefaultConfig();
            }

            try
            {
                ETHLogging.Log(ETHLogging.LogLevel.Information, "Config file exists, loading existing config");
                return LoadExistingConfig();
            }
            catch (Exception ex)
            {
                ETHLogging.Log(ETHLogging.LogLevel.Error, $"Failed to load config: {ex.Message}");
                ETHLogging.Log(ETHLogging.LogLevel.Warning, "Creating default config due to error");
                return CreateDefaultConfig();
            }
        }

        public static void SaveConfig()
        {
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Saving config to file");

            var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);
            File.WriteAllText(ETHPath.Data.ConfigFile, configJson);

            ETHLogging.Log(ETHLogging.LogLevel.Information, "Config saved successfully");
        }

        private static Configuration LoadExistingConfig()
        {
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Reading config file");
            var configJson = File.ReadAllText(ETHPath.Data.ConfigFile);

            ETHLogging.Log(ETHLogging.LogLevel.Information, "Deserializing config");
            _config = JsonConvert.DeserializeObject<Configuration>(configJson);

            LogConfigProperties();

            ETHLogging.Log(ETHLogging.LogLevel.Information, "Config loaded successfully");
            return _config;
        }

        private static Configuration CreateDefaultConfig()
        {
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Creating default config");

            _config = new Configuration();
            var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);

            ETHLogging.Log(ETHLogging.LogLevel.Information, "Writing default config to file");
            File.WriteAllText(ETHPath.Data.ConfigFile, configJson);

            LogConfigProperties();

            ETHLogging.Log(ETHLogging.LogLevel.Information, "Config file created with default values");
            return _config;
        }

        private static void LogConfigProperties()
        {
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Logging config properties");

            foreach (var prop in typeof(Configuration).GetProperties())
            {
                ETHLogging.Log(ETHLogging.LogLevel.Information, $"{prop.Name}: {prop.GetValue(_config)}");
            }
        }
    }
}
