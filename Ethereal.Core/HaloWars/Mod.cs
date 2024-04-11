//-----------------------------------------------------------------------------
// File: Mod.cs
// Description: Contains the Manager class responsible for managing mods in Halo Wars.
//    This class provides functionality to get mods, set the current mod, and handle mod installations and uninstallations.
//-----------------------------------------------------------------------------

using Ethereal.Core.Logging;
using System.IO;
using System.Xml.Linq;

namespace Ethereal.Core.HaloWars
{
    public class Mod
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string HWModPath { get; set; } = string.Empty;
        public string CustomVideoPath { get; set; } = string.Empty;
        public string CustomSavePath { get; set; } = string.Empty;
    }

    public class Manager
    {
        private readonly Manifest Manifest;
        private readonly LogWriter Writer;
        public Mod CurrentMod = new() { Name = "Vanilla" };
        private readonly string ExecutableDirectory;
        private readonly string CurrentVideoPath;
        private readonly string CurrentSavePath;

        public Manager(string exePath, Manifest manifest, LogWriter writer)
        {
            ExecutableDirectory = Path.GetDirectoryName(exePath)!;
            CurrentVideoPath = Path.Combine(ExecutableDirectory, "video");
            CurrentSavePath = Path.Combine(ExecutableDirectory, "savegame");
            Manifest = manifest;
            Writer = writer;
        }

        public List<Mod> GetMods(string modsPath)
        {
            Writer.Log(LogLevel.Information, $"Getting mods from directory: {modsPath}");

            DirectoryInfo modsDirectory = new(modsPath);
            List<Mod> mods = modsDirectory.GetDirectories()
       .Select(dir =>
       {
           Mod mod = new() { Name = dir.Name, Path = dir.FullName };

           FileInfo hwModFile = new(Path.Combine(dir.FullName, ".hwmod"));
           if (hwModFile.Exists)
           {
               mod.HWModPath = hwModFile.FullName;

               XDocument hwModXml = XDocument.Load(hwModFile.FullName);
               XElement? requiredData = hwModXml.Root?.Element("RequiredData");
               if (requiredData != null)
               {
                   XAttribute? title = requiredData.Attribute("Title");
                   if (title != null)
                   {
                       mod.Name = title.Value;
                   }
               }

               DirectoryInfo modDataDir = new(Path.Combine(dir.FullName, "ModData"));
               if (modDataDir.Exists)
               {
                   mod.Path = modDataDir.FullName;
               }

               return mod;
           }

           return mod;
       })
       .Where(mod => mod != null)
       .ToList();


            Writer.Log(LogLevel.Information, $"Retrieved {mods.Count} mods from the directory");

            return mods;
        }

        public void SetCurrentMod(Mod mod)
        {
            Writer.Log(LogLevel.Information, $"Setting current mod to: {mod.Path}");

            if (mod.Name == "Vanilla")
            {
                Writer.Log(LogLevel.Information, "Uninstalling current mod");
                Manifest.Uninstall();
                return;
            }

            if (mod.CustomVideoPath != null)
            {
                Writer.Log(LogLevel.Information, $"Custom video path specified for mod: {mod.Name}");
                // TO-DO: Add custom video support
            }

            if (mod.CustomSavePath != null)
            {
                Writer.Log(LogLevel.Information, $"Custom save path specified for mod: {mod.Name}");
                // TO-DO: Add custom save support
            }


            Writer.Log(LogLevel.Information, $"Installing mod: {mod.Name}");
            Manifest.Install(mod);

            CurrentMod = mod;
        }

        public Mod? FindMod(string path, string modsPath)
        {
            return GetMods(modsPath)
                .FirstOrDefault(mod => mod.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        }

    }
}