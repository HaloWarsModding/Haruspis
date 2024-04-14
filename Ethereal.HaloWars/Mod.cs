using Ethereal.Logging;
using System.Xml.Serialization;

namespace Ethereal.HaloWars
{
    public sealed class Mod
    {
        #region Properties
        public string? Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string ModPath { get; set; } = string.Empty;
        public string HWModPath { get; set; } = string.Empty;
        public string CustomVideoPath { get; set; } = string.Empty;
        public string CustomSavePath { get; set; } = string.Empty;
        #endregion

        private static Mod? _instance;

        public Mod() { }

        public static Mod GetInstance()
        {
            if (_instance == null)
            {
                lock (typeof(Mod))
                {
                    _instance ??= new Mod();
                }
            }
            return _instance;
        }

        public void FromPath(string path)
        {
            ModPath = path;

            if (!Directory.Exists(ModPath))
            {
                Logger.GetInstance().Log(LogLevel.Error, "Specified path does not exist.");
                return;
            }

            HandleModData();
            HandleHWMOD();

            if(Name == string.Empty)
            {
                Name = Path.GetDirectoryName(ModPath);
            }
        }

        private void HandleModData()
        {
            string modDataPath = Path.Combine(ModPath, "ModData");
            if (Directory.Exists(modDataPath))
            {
                ModPath = modDataPath;
            }
        }

        private void HandleHWMOD()
        {
            string path = ModPath;

            if (Path.GetFileName(ModPath) == "ModData")
            {
                path = Directory.GetParent(ModPath)!.FullName;
                Logger.GetInstance().Log(LogLevel.Information, "Navigated back one level in the directory tree.");
            }

            string[] hwModFiles = Directory.GetFiles(path, "*.hwmod");
            if (hwModFiles.Length > 0)
            {
                string hwModFilePath = hwModFiles[0];

                try
                {
                    XmlSerializer serializer = new(typeof(HWMod));
                    using FileStream fileStream = new(hwModFilePath, FileMode.Open);
                    HWMod mod = (HWMod)serializer.Deserialize(fileStream)!;

                    Name = mod.RequiredData.Title;
                    Description = mod.OptionalData.Description;
                    Author = mod.RequiredData.Author;
                    Version = mod.RequiredData.Version;
                }
                catch (Exception ex)
                {
                    Logger.GetInstance().Log(LogLevel.Error, $"Error parsing .hwmod file: {ex.Message}");
                }
            }
            else
            {
                Logger.GetInstance().Log(LogLevel.Information, "No .hwmod file found in the directory.");
            }
        }
    }

    #region Medstar's .hwmod

    public class HWMod
    {
        [XmlAttribute("ManifestVersion")]
        public required int ManifestVersion { get; set; }

        [XmlAttribute("ModID")]
        public required string ModID { get; set; }

        public required RequiredData RequiredData { get; set; }
        public OptionalData? OptionalData { get; set; }
    }

    public class RequiredData
    {
        [XmlAttribute("Title")]
        public required string Title { get; set; }

        [XmlAttribute("Author")]
        public required string Author { get; set; }

        [XmlAttribute("Version")]
        public required string Version { get; set; }
    }

    public class OptionalData
    {
        public string? BannerArt { get; set; }
        public string? Icon { get; set; }
        public string? Description { get; set; }
    }

    #endregion
}
