using System.Xml.Serialization;

namespace Data
{
    public sealed class Configuration
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public AppConfig Settings { get; set; } = new AppConfig();
        public GameConfig HaloWars { get; set; } = new GameConfig();
        public ModsConfig Mods { get; set; } = new ModsConfig(BaseDirectory);

        private readonly XmlSerializer serializer = new(typeof(Configuration));

        public void ToFile(string path)
        {
            try
            {
                using StreamWriter streamWriter = new(path);
                serializer.Serialize(streamWriter, this);
                Console.WriteLine($"Config file successfully written to: {path}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied while writing config file: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory not found while writing config file: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error while writing config file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error writing config file: {ex.Message}");
            }
        }

        public void FromFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"Configuration file does not exist: {path}");
                return;
            }

            try
            {
                using StreamReader streamReader = new(path);
                if (serializer.Deserialize(streamReader) is Configuration configObject)
                {
                    Settings = configObject.Settings;
                    HaloWars = configObject.HaloWars;
                    Mods = configObject.Mods;
                    Console.WriteLine($"Config file successfully read from: {path}");
                }
                else
                {
                    Console.WriteLine($"Failed to deserialize configuration object from file: {path}");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied while reading config file: {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found while reading config file: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error while reading config file: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation while reading config file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error reading config file: {ex.Message}");
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

        public class ModsConfig
        {
            public ModsConfig()
            {
                // Parameterless constructor for serialization
            }

            public ModsConfig(string baseDirectory)
            {
                Path = System.IO.Path.Combine(baseDirectory, "mods");
            }

            public string Path { get; set; }
            public bool CustomManifest { get; set; } = false;
            public string LastPlayedMod { get; set; } = "Vanilla";
        }

        public enum DistributionType
        {
            Steam,
            MicrosoftStore
        }
    }
}
