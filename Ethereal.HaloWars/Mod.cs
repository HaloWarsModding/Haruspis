using Ethereal.Logging;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Ethereal.HaloWars
{
    public sealed class Mod
    {
        #region Properties
        public string? Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string ModPath { get; set; } = string.Empty;
        public string HWModPath { get; set; } = string.Empty;
        public string CustomVideoPath { get; set; } = string.Empty;
        public string CustomSavePath { get; set; } = string.Empty;
        public bool IsMod { get; set; } = true;
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

            Logger.GetInstance().Log(LogLevel.Information, $"Loading mod from path: {ModPath}");

            if (!Directory.Exists(ModPath))
            {
                Logger.GetInstance().Log(LogLevel.Error, "Specified path does not exist.");
                return;
            }

            if (!IsValid())
            {
                IsMod = false;
                Logger.GetInstance().Log(LogLevel.Warning, "Mod is not valid.");
                return;
            }

            Logger.GetInstance().Log(LogLevel.Information, "Mod is valid.");

            IsMod = true;

            HandleModData();
            HandleHWMOD();
            HandleEmpty();
        }

        private bool IsValid()
        {
            DirectoryInfo info = new(ModPath);

            bool isValid = !info.Name.StartsWith('.');

            if (!isValid)
            {
                Logger.GetInstance().Log(LogLevel.Warning, "Mod directory name starts with a dot.");
            }

            return isValid;
        }

        private void HandleEmpty()
        {
            if (Name == string.Empty)
            {
                DirectoryInfo info = new(ModPath);

                if (info.Name == "ModData")
                {
                    Name = Directory.GetParent(info.FullName)!.Name;
                    Logger.GetInstance().Log(LogLevel.Information, "Mod name set from parent directory.");
                    return;
                }

                Name = info.Name;
                Logger.GetInstance().Log(LogLevel.Information, "Mod name set from directory name.");
            }
        }

        private void HandleModData()
        {
            string modDataPath = Path.Combine(ModPath, "ModData");
            if (Directory.Exists(modDataPath))
            {
                ModPath = modDataPath;
                Logger.GetInstance().Log(LogLevel.Information, "Mod data directory found and set as mod path.");
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
                    Logger.GetInstance().Log(LogLevel.Information, $"Loading HWMod from file: {hwModFilePath}");

                    XmlSerializer serializer = new(typeof(HWMod));
                    using FileStream fileStream = new(hwModFilePath, FileMode.Open);
                    HWMod mod = (HWMod)serializer.Deserialize(fileStream)!;

                    Name = mod.RequiredData.Title;
                    Description = mod.OptionalData?.Description ?? string.Empty;
                    Author = mod.RequiredData.Author;
                    Version = mod.RequiredData.Version;

                    Logger.GetInstance().Log(LogLevel.Information, "HWMod loaded successfully.");
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
        public int ManifestVersion { get; set; }

        [XmlAttribute("ModID")]
        public string ModID { get; set; }

        public RequiredData RequiredData { get; set; }
        public OptionalData? OptionalData { get; set; }
    }

    public class RequiredData
    {
        [XmlAttribute("Title")]
        public string Title { get; set; }

        [XmlAttribute("Author")]
        public string Author { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }
    }

    public class OptionalData
    {
        public string? BannerArt { get; set; }
        public string? Icon { get; set; }
        public string? Description { get; set; }
    }

    #endregion
}
