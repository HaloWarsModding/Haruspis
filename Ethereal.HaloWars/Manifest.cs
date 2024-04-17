using Ethereal.Logging;

namespace Ethereal.HaloWars
{
    public sealed class Manifest
    {
        private static Manifest? _instance;
        public string? Content { get; set; } = string.Empty;

        public Manifest() { }

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

        public void FromFile(string path)
        {
            try
            {
                using StreamReader streamReader = new(path);
                Content = streamReader.ReadToEnd();
                Logger.GetInstance().Log(LogLevel.Information, $"Manifest loaded successfully from file: {path}");
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(LogLevel.Error, $"Error loading manifest from file: {ex.Message}");
            }
        }

        public void ToFile(string path)
        {
            try
            {
                using StreamWriter streamWriter = new(path, false);
                streamWriter.Write(Content);
                Logger.GetInstance().Log(LogLevel.Information, $"Manifest saved successfully to file: {path}");
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(LogLevel.Error, $"Error saving manifest to file: {ex.Message}");
            }
        }
    }
}
