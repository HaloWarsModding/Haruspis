//-----------------------------------------------------------------------------
// File: ConfigWriter.cs
// Description: Contains the ConfigWriter class responsible for writing configuration files.
//              This class provides functionality to write a configuration file with the specified configuration object.
//-----------------------------------------------------------------------------

using Ethereal.Core.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Ethereal.Core.Configuration
{
    internal interface IConfigWriter
    {
        /// <summary>
        /// Writes the configuration file with the specified configuration object.
        /// </summary>
        /// <param name="configObject">The configuration object to be serialized and written to the file.</param>
        void WriteConfigFile(object configObject);
    }

    public class ConfigWriter(string configFilePath, LogWriter logWriter) : IConfigWriter
    {
        private readonly string configFilePath = configFilePath;
        private readonly LogWriter logWriter = logWriter;
        private FileStream? fileStream;

        public void WriteConfigFile(object configObject)
        {
            try
            {
                Type objectType = configObject.GetType();
                PropertyInfo[] properties = objectType.GetProperties();

                if (properties.Length == 0)
                {
                    logWriter.Log(LogLevel.Warning, "No properties found in the configuration object.");
                }
                else
                {
                    foreach (PropertyInfo property in properties)
                    {
                        object value = property.GetValue(configObject)!;
                        if (value != null)
                        {
                            logWriter.Log(LogLevel.Information, $"Writing property {property.Name} with value: {value}");
                        }
                        else
                        {
                            logWriter.Log(LogLevel.Warning, $"Property {property.Name} has a null value");
                        }
                    }
                }

                string json = JsonConvert.SerializeObject(configObject, Formatting.Indented);

                fileStream = new FileStream(configFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                using (StreamWriter streamWriter = new(fileStream))
                {
                    streamWriter.Write(json);
                }

                logWriter.Log(LogLevel.Information, "Config file successfully written to: " + configFilePath);
            }
            catch (Exception ex)
            {
                logWriter.Log(LogLevel.Error, "Error writing config file: " + ex.Message);
                throw;
            }
            finally
            {
                fileStream?.Close();
                logWriter.Log(LogLevel.Debug, "WriteConfigFile method finished executing");
            }
        }
    }
}