using Ethereal.Logging;
using Ethereal.Utils;

namespace Ethereal.HaloWars
{
    public sealed class Manifest : Singleton<Manifest>
    {
        public string? Content { get; set; } = string.Empty;

        public void FromFile(string path)
        {
            try
            {
                using StreamReader streamReader = new(path);
                Content = streamReader.ReadToEnd();
                Logger.Log(LogLevel.Information, $"Manifest loaded successfully from file: {path}");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error loading manifest from file: {ex.Message}");
            }
        }
        public void ToFile(string path)
        {
            try
            {
                using StreamWriter streamWriter = new(path, false);
                streamWriter.Write(Content);
                Logger.Log(LogLevel.Information, $"Manifest saved successfully to file: {path}");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error saving manifest to file: {ex.Message}");
            }
        }
    }
}
