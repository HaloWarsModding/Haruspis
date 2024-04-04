using EtherealModManagerGUI.Data;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Xml.Linq;

namespace EtherealModManagerGUI
{
    internal class ETHManager
    {
        public static Mod currentMod;
        private static readonly string ExeDirectory = Path.GetDirectoryName(ETHConfig.CurrentConfiguration.Game.GameExecutablePath);
        private static readonly string currentVideoPath = Path.Combine(ExeDirectory, "video");

        public class Mods
        {
            public static List<Mod> GetMods()
            {
                ETHLogging.Log(ETHLogging.LogLevel.Information, "Fetching mods...");

                var modsDirectory = new DirectoryInfo(ETHConfig.CurrentConfiguration.Game.ModsDirectory);
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

                        ETHLogging.Log(ETHLogging.LogLevel.Debug, $"Mod fetched: {mod.Name}");

                        return mod;
                    })
                    .ToList();

                ETHLogging.Log(ETHLogging.LogLevel.Information, "Finished fetching mods.");

                return mods;
            }
        }

        public static void SetCurrentMod(Mod mod)
        {
            ETHLogging.Log(ETHLogging.LogLevel.Information, $"Setting current mod to {mod.Name}");
            if(mod.Name == "Vanilla")
            {
                ETHManifest.Clear().GetAwaiter();
                return;
            }
            ETHManifest.Write(mod.Path).GetAwaiter();

            if (mod.CustomVideoPath != null)
            {
                TryCacheVideo();

                ETHLogging.Log(ETHLogging.LogLevel.Information, $"Copying custom video directory to current video directory: {mod.CustomVideoPath} -> {currentVideoPath}");
                FileSystem.CopyDirectory(mod.CustomVideoPath, currentVideoPath, true);
                ETHLogging.Log(ETHLogging.LogLevel.Information, "Copy to current video directory completed.");
            }

            currentMod = mod;
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Current mod set.");
        }

        public static void TryCacheVideo()
        {
            var cacheVideoPath = Path.Combine(ETHPath.Data.Cache, "video");
            var currentDirectory = new DirectoryInfo(currentVideoPath);
            var cacheDirectory = new DirectoryInfo(cacheVideoPath);

            if (!cacheDirectory.Exists && currentDirectory.Exists)
            {
                ETHLogging.Log(ETHLogging.LogLevel.Information, $"Copying current video directory to cache: {currentVideoPath} -> {cacheVideoPath}");
                FileSystem.CopyDirectory(currentVideoPath, cacheVideoPath);
                ETHLogging.Log(ETHLogging.LogLevel.Information, "Copy to cache completed.");
            }
        }
    }
}