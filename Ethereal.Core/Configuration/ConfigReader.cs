using Newtonsoft.Json;
using System.IO;

namespace EtherealEngine
{
    internal interface IConfigReader
    {
        /// <summary>
        /// Gets the path of the configuration file.
        /// </summary>
        string ConfigFilePath { get; }

        /// <summary>
        /// Validates if the configuration file exists and is not empty.
        /// </summary>
        /// <returns>True if the configuration file is valid; otherwise, false.</returns>
        bool IsConfigFileValid();

        /// <summary>
        /// Reads and deserializes the configuration file into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize the configuration file into.</typeparam>
        /// <returns>The deserialized object of type T.</returns>
        T ReadConfigFile<T>() where T : new();
    }

    public class ConfigReader(string configFilePath, LogWriter logWriter) : IConfigReader
    {
        public static object? ConfigObject { get; private set; }
        public static bool ConfigLoaded { get; private set; }

        public string ConfigFilePath { get; private set; } = configFilePath;
        public LogWriter LogWriter { get; private set; } = logWriter;

        private string ReadFileContents()
        {
            if (!File.Exists(ConfigFilePath))
            {
                LogWriter.Log(LogLevel.Error, "Configuration file does not exist.");
            }

            try
            {
                using FileStream fileStream = new(ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader streamReader = new(fileStream);
                return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                LogWriter.Log(LogLevel.Error, $"Error reading configuration file: {ex.Message}");
                return string.Empty;
            }
        }

        public bool IsConfigFileValid()
        {
            string fileContents = ReadFileContents();
            if (string.IsNullOrEmpty(fileContents))
            {
                LogWriter.Log(LogLevel.Error, "Configuration file is empty.");
                return false;
            }
            return true;
        }

        public T ReadConfigFile<T>() where T : new()
        {
            T configObject = new();

            if (IsConfigFileValid())
            {
                string fileContents = ReadFileContents();
                try
                {
                    configObject = JsonConvert.DeserializeObject<T>(fileContents)!;
                    LogWriter.Log(LogLevel.Information, "Configuration file deserialized successfully.");
                    ConfigObject = configObject;
                    ConfigLoaded = true;
                }
                catch (Exception ex)
                {
                    LogWriter.Log(LogLevel.Error, $"Error deserializing configuration file: {ex.Message}");
                    throw new JsonSerializationException("Error deserializing configuration file.", ex);
                }
            }

            return configObject;
        }
    }
}