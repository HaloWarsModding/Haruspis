using Ethereal.Logging;
using System.Xml.Serialization;

namespace Ethereal.HaloWars
{
    public sealed class Manifest
    {
        private static Manifest? _instance;
        private string? Content { get; set; }

        private Manifest() { }

        public static Manifest GetInstance()
        {
            if (_instance == null)
            {
                lock (typeof(Manifest))
                {
                    _instance ??= new Manifest();
                }
            }
            return _instance;
        }

        private static readonly XmlSerializer serializer = new(typeof(string));

        public void FromFile(string path)
        {
            try
            {
                using StreamReader streamReader = new(path);
                Content = serializer.Deserialize(streamReader) as string;
                Logger.GetInstance().Log(LogLevel.Information, "Manifest loaded successfully from file: " + path);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(LogLevel.Error, "Error loading manifest from file: " + ex.Message);
            }
        }

        public void ToFile(string path)
        {
            try
            {
                if (Content == null)
                {
                    Logger.GetInstance().Log(LogLevel.Error, "Manifest content is null. Load content before writing to file.");
                    return;
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                    Logger.GetInstance().Log(LogLevel.Information, "Deleted existing manifest file: " + path);
                }

                using StreamWriter streamWriter = new(path);
                serializer.Serialize(streamWriter, Content);
                Logger.GetInstance().Log(LogLevel.Information, "Manifest saved successfully to file: " + path);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(LogLevel.Error, "Error saving manifest to file: " + ex.Message);
            }
        }
    }
}
