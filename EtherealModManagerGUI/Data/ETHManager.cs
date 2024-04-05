using EtherealModManager.Data;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Xml.Linq;

namespace EtherealModManager
{
    internal class ETHManager
    {
        private readonly string exePath;
        public static Mod currentMod;
        private readonly string ExeDirectory;
        private readonly string currentVideoPath;

        public ETHManager(string exePath)
        {
            this.exePath = exePath;
            ExeDirectory = Path.GetDirectoryName(exePath);
            currentVideoPath = Path.Combine(ExeDirectory, "video");
        }

        public static List<Mod> GetMods(string modsPath)
        {
            var modsDirectory = new DirectoryInfo(modsPath);
            var mods = modsDirectory.GetDirectories()
                .Select(dir =>
                {
                    var mod = new Mod { Name = dir.Name, Path = dir.FullName };

                    var hwModFile = new FileInfo(Path.Combine(dir.FullName, ".hwmod"));
                    if (hwModFile.Exists)
                    {
                        mod.HWModPath = hwModFile.FullName;

                        var hwModXml = XDocument.Load(hwModFile.FullName);
                        var requiredData = hwModXml.Root.Element("RequiredData");
                        if (requiredData != null)
                        {
                            var title = requiredData.Attribute("Title");
                            if (title != null)
                            {
                                mod.Title = title.Value;
                            }
                        }
                    }

                    var modDataDir = new DirectoryInfo(Path.Combine(dir.FullName, "ModData"));
                    if (modDataDir.Exists)
                    {
                        mod.Path = modDataDir.FullName;
                    }

                    return mod;
                })
                .ToList();

            return mods;
        }


        public void SetCurrentMod(Mod mod)
        {
            if (mod.Name == "Vanilla")
            {
                ///ETHManifest.Clear().GetAwaiter();
                return;
            }
            ///ETHManifest.Write(mod.Path).GetAwaiter();

            if (mod.CustomVideoPath != null)
            {
                TryCacheVideo();

                FileSystem.CopyDirectory(mod.CustomVideoPath, currentVideoPath, true);
            }

            currentMod = mod;
        }

        public void TryCacheVideo()
        {
            var cacheVideoPath = Path.Combine("ETHPath.Data.Cache", "video");
            var currentDirectory = new DirectoryInfo(currentVideoPath);
            var cacheDirectory = new DirectoryInfo(cacheVideoPath);

            if (!cacheDirectory.Exists && currentDirectory.Exists)
            {
                FileSystem.CopyDirectory(currentVideoPath, cacheVideoPath);
            }

        }
    }
}