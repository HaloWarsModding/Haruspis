using System;
using System.IO;
using System.Xml.Serialization;

using Ethereal.Logging;
using Ethereal.Utils;

namespace Ethereal.HaloWars
{
    public sealed class Mod : Singleton<Mod>
    {
        #region Properties
        public string? Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;
        public string ChangelogPath { get; set; } = string.Empty;
        public string ModPath { get; set; } = string.Empty;
        public string DataPath { get; set; } = string.Empty;
        public string HWModPath { get; set; } = string.Empty;
        public string CustomVideoPath { get; set; } = string.Empty;
        public string CustomSavePath { get; set; } = string.Empty;
        public bool IsMod { get; set; } = true;
        #endregion

        #region Medstar's .hwmod
        public class HWMod
        {
            [XmlAttribute("ManifestVersion")]
            public int ManifestVersion { get; set; }

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

        public void FromPath(string path)
        {
            ModPath = path;
            Logger.Log(LogLevel.Information, $"Loading mod from path: {ModPath}");

            if (!Directory.Exists(ModPath))
            {
                Logger.Log(LogLevel.Error, "Specified mod path does not exist.");
                return;
            }

            if (!IsValid())
            {
                IsMod = false;
                Logger.Log(LogLevel.Warning, "The mod is not valid.");
                return;
            }

            Logger.Log(LogLevel.Information, "Mod is valid.");
            IsMod = true;

            HandleModData();
            CompatibilityPatch();
            HandleEmpty();

            DataPath = Path.Combine(ModPath, "data.eth");
            ChangelogPath = Path.Combine(ModPath, "changelog.txt");
        }

        private bool IsValid()
        {
            DirectoryInfo info = new(ModPath);
            bool isValid = !info.Name.StartsWith('.');

            if (!isValid)
            {
                Logger.Log(LogLevel.Warning, "Mod directory name starts with a dot, which may cause issues.");
            }

            return isValid;
        }

        private void HandleEmpty()
        {
            if (string.IsNullOrEmpty(Name))
            {
                DirectoryInfo info = new(ModPath);

                if (info.Name == "ModData")
                {
                    Name = Directory.GetParent(info.FullName)!.Name;
                    Logger.Log(LogLevel.Information, "Mod name set from parent directory.");
                    return;
                }

                Name = info.Name;
                Logger.Log(LogLevel.Information, "Mod name set from the directory name.");
            }
        }

        private void HandleModData()
        {
            string modDataPath = Path.Combine(ModPath, "ModData");

            if (!Directory.Exists(modDataPath))
            {
                Logger.Log(LogLevel.Warning, "ModData directory does not exist.");
                return;
            }

            ModPath = modDataPath;
            Logger.Log(LogLevel.Information, "Mod data directory found and set as the mod path.");
        }

        private void CompatibilityPatch()
        {
            if (Path.GetFileName(ModPath) != "ModData")
            {
                Logger.Log(LogLevel.Information, "Mod path is not ModData. No compatibility patch needed.");
                return;
            }

            string parentDirectory = Directory.GetParent(ModPath)!.FullName;
            Logger.Log(LogLevel.Information, "Navigated back one level in the directory tree.");

            string[] hwModFiles = Directory.GetFiles(parentDirectory, "*.hwmod");
            if (hwModFiles.Length == 0)
            {
                Logger.Log(LogLevel.Information, "No .hwmod file found in the directory.");
                return;
            }

            string hwModFilePath = hwModFiles[0];

            try
            {
                Logger.Log(LogLevel.Information, $"Loading HWMod from file: {hwModFilePath}");

                XmlSerializer serializer = new(typeof(HWMod));
                using FileStream fileStream = new(hwModFilePath, FileMode.Open);
                HWMod mod = (HWMod)serializer.Deserialize(fileStream)!;

                if (mod == null || mod.RequiredData == null)
                {
                    Logger.Log(LogLevel.Warning, "HWMod file is missing required data.");
                    return;
                }

                Name = mod.RequiredData.Title ?? string.Empty;
                Author = mod.RequiredData.Author ?? string.Empty;
                Version = mod.RequiredData.Version ?? string.Empty;

                Description = mod.OptionalData?.Description ?? string.Empty;
                Icon = mod.OptionalData?.Icon ?? string.Empty;
                Banner = mod.OptionalData?.BannerArt ?? string.Empty;

                Logger.Log(LogLevel.Information, "HWMod loaded successfully.");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error parsing .hwmod file: {ex.Message}");
            }
        }
    }
}
