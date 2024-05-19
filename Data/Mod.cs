using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

using Common;

namespace Data
{
    /// <summary>
    /// Represents a mod with its properties and methods for loading from a specified path.
    /// </summary>
    public sealed partial class Mod : Singleton<Mod>
    {
        /// <summary>
        /// Represents the properties of a mod.
        /// </summary>
        public class ModProperties
        {
            [XmlElement("ModName")]
            public string Name { get; set; } = string.Empty;

            [XmlElement("ModDescription")]
            public string Description { get; set; } = string.Empty;

            [XmlElement("ModAuthor")]
            public string Author { get; set; } = string.Empty;

            [XmlElement("ModVersion")]
            public string Version { get; set; } = string.Empty;

            [XmlElement("ModIcon")]
            public string Icon { get; set; } = string.Empty;

            [XmlElement("ModBanner")]
            public string Banner { get; set; } = string.Empty;

            [XmlElement("ModChangelogPath")]
            public string ChangelogPath { get; set; } = string.Empty;

            [XmlElement("ModPath")]
            public string ModPath { get; set; } = string.Empty;

            [XmlElement("EncryptedDataFilePath")]
            public string DataPath { get; set; } = string.Empty;

            [XmlElement("HWModFilePath")]
            public string HWModPath { get; set; } = string.Empty;

            [XmlElement("ModCustomSavePath")]
            public string CustomSavePath { get; set; } = string.Empty;

            [XmlElement("IsModValidForDisplay")]
            public bool IsValid { get; set; } = true;
        }

        /// <summary>
        /// Gets or sets the properties of the mod.
        /// </summary>
        public ModProperties Properties { get; set; } = new ModProperties();

        /// <summary>
        /// Loads the mod properties from the specified path.
        /// </summary>
        /// <param name="path">The path to load the mod from.</param>
        public void FromPath(string path)
        {
            Properties.ModPath = path;
            Console.WriteLine($"Loading mod from path: {Properties.ModPath}");

            if (!Directory.Exists(Properties.ModPath))
            {
                Console.WriteLine("Specified mod path does not exist.");
                return;
            }

            if (!IsModValid())
            {
                Console.WriteLine("The mod is not valid.");
                Properties.IsValid = false;
                return;
            }

            Console.WriteLine("Mod is valid.");

            HandleModData();
            CompatibilityPatch();
            HandleEmpty();

            Properties.DataPath = Path.Combine(Properties.ModPath, "data.hrs");
            Properties.ChangelogPath = Path.Combine(Properties.ModPath, "changelog.txt");
        }

        /// <summary>
        /// Validates the mod path.
        /// </summary>
        /// <returns>True if the mod path is valid; otherwise, false.</returns>
        private bool IsModValid()
        {
            DirectoryInfo info = new(Properties.ModPath);
            bool isValid = !info.Name.StartsWith('.');

            if (!isValid)
            {
                Console.WriteLine("Mod directory name starts with a dot, which may cause issues.");
            }

            return isValid;
        }

        /// <summary>
        /// Handles the case where the mod name is empty by setting it from the directory name.
        /// </summary>
        private void HandleEmpty()
        {
            if (string.IsNullOrEmpty(Properties.Name))
            {
                DirectoryInfo info = new(Properties.ModPath);

                if (info.Name == "ModData")
                {
                    Properties.Name = Directory.GetParent(info.FullName)!.Name;
                    Console.WriteLine("Mod name set from parent directory.");
                    return;
                }

                Properties.Name = info.Name;
                Console.WriteLine("Mod name set from the directory name.");
            }
        }

        /// <summary>
        /// Handles the mod data by setting the mod path to the ModData directory.
        /// </summary>
        private void HandleModData()
        {
            string modDataPath = System.IO.Path.Combine(Properties.ModPath, "ModData");

            if (!Directory.Exists(modDataPath))
            {
                Console.WriteLine("ModData directory does not exist.");
                return;
            }

            Properties.ModPath = modDataPath;
            Console.WriteLine("Mod data directory found and set as the mod path.");
        }

        /// <summary>
        /// Applies compatibility patch if needed.
        /// </summary>
        private void CompatibilityPatch()
        {
            if (System.IO.Path.GetFileName(Properties.ModPath) != "ModData")
            {
                Console.WriteLine("Mod path is not ModData. No compatibility patch needed.");
                return;
            }

            string parentDirectory = Directory.GetParent(Properties.ModPath)!.FullName;
            Console.WriteLine("Navigated back one level in the directory tree.");

            string[] hwModFiles = Directory.GetFiles(parentDirectory, "*.hwmod");
            if (hwModFiles.Length == 0)
            {
                Console.WriteLine("No .hwmod file found in the directory.");
                return;
            }

            string hwModFilePath = hwModFiles[0];

            try
            {
                Console.WriteLine($"Loading HWMod from file: {hwModFilePath}");

                XmlSerializer serializer = new(typeof(HWMod));
                using FileStream fileStream = new(hwModFilePath, FileMode.Open);
                HWMod mod = (HWMod)serializer.Deserialize(fileStream)!;

                if (mod == null || mod.RequiredData == null)
                {
                    Console.WriteLine("HWMod file is missing required data.");
                    return;
                }

                SetModProperties(mod);

                Console.WriteLine("HWMod loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing .hwmod file: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the mod properties from the given HWMod object.
        /// </summary>
        /// <param name="mod">The HWMod object to set properties from.</param>
        private void SetModProperties(HWMod mod)
        {
            Properties.Name = mod.RequiredData.Title ?? string.Empty;
            Properties.Author = mod.RequiredData.Author ?? string.Empty;
            Properties.Version = mod.RequiredData.Version ?? string.Empty;

            Properties.Description = mod.OptionalData?.Description ?? string.Empty;
            Properties.Icon = mod.OptionalData?.Icon ?? string.Empty;
            Properties.Banner = mod.OptionalData?.BannerArt ?? string.Empty;
        }

        /// <summary>
        /// Saves the mod properties to an XML file.
        /// </summary>
        /// <param name="filePath">The path to save the XML file to.</param>
        public void SaveToXml(string filePath)
        {
            try
            {
                XmlSerializer serializer = new(typeof(ModProperties));
                using FileStream fileStream = new(filePath, FileMode.Create);
                serializer.Serialize(fileStream, Properties);
                Console.WriteLine("Mod properties saved to XML successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving mod properties to XML: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the mod properties from an XML file.
        /// </summary>
        /// <param name="filePath">The path to load the XML file from.</param>
        public void LoadFromXml(string filePath)
        {
            try
            {
                XmlSerializer serializer = new(typeof(ModProperties));
                using FileStream fileStream = new(filePath, FileMode.Open);
                Properties = (ModProperties)serializer.Deserialize(fileStream)!;
                Console.WriteLine("Mod properties loaded from XML successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading mod properties from XML: {ex.Message}");
            }
        }

        public static class ImageProcessing
        {
            public static BitmapImage LoadImageFromFile(string path)
            {
                BitmapImage bitmap = new();
                if (!string.IsNullOrEmpty(path))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.EndInit();
                }
                return bitmap;
            }

            public static BitmapImage LoadDefaultImage(Bitmap m)
            {
                BitmapImage bitmap = new();
                using (MemoryStream stream = new())
                {
                    m.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                return bitmap;
            }

            public static BitmapImage LoadBannerImage(Mod mod, Bitmap def)
            {
                BitmapImage bitmap = new();
                string bannerPath = mod.Properties.Banner;
                string path;

                switch (bannerPath)
                {
                    case "":
                        bitmap = LoadDefaultImage(def);
                        break;
                    case string bPath when bPath.StartsWith("ModData\\"):
                        path = Path.Combine(mod.Properties.ModPath, bPath["ModData\\".Length..]);
                        bitmap = LoadImageFromFile(path);
                        break;
                    case string bPath when File.Exists(bPath):
                        path = bPath;
                        bitmap = LoadImageFromFile(path);
                        break;
                    default:
                        break;
                }

                return bitmap;
            }
        }
    }
}
