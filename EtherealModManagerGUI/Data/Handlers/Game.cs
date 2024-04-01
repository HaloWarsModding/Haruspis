using EtherealModManagerGUI.Configuration;
using EtherealModManagerGUI.Logging;
using System.Diagnostics;

namespace EtherealModManagerGUI.Handlers
{
    internal class Game
    {
        private static Process p = null;

        public static event Action GameExited;
        public static event Action GameStarted;

        public static async void Start()
        {
            if (IsGameRunning())
            {
                return;
            }

            StartGameProcess();

            await AutomaticExeScanning();
        }

        public static async Task AutomaticExeScanning()
        {
            Process gameProcess = await GetGameProcess();
            if (gameProcess != null)
            {
                HandleGameProcessFound(gameProcess);
            }
            else
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Warning, "Game process not found after 70 attempts.");
            }
        }

        public static async void StartAndKill()
        {
            if (IsGameRunning())
            {
                return;
            }

            StartGameProcess();

            await CatchAndCloseProcess();
        }

        public static async Task CatchAndCloseProcess()
        {
            Process gameProcess = await GetGameProcess();
            if (gameProcess != null)
            {
                HandleGameProcessFound(gameProcess);
                gameProcess.Kill();
                EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Game process closed.");
            }
            else
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Warning, "Game process not found after 70 attempts.");
            }
        }

        private static bool IsGameRunning()
        {
            return p != null && !p.HasExited;
        }

        private static void StartGameProcess()
        {
            p = new Process();
            p.StartInfo.CreateNoWindow = true;

            if (!string.IsNullOrEmpty(EtherealConfig.Config.CachedExePath))
            {
                p.StartInfo.FileName = EtherealConfig.Config.CachedExePath;
            }
            else
            {
                p.StartInfo.FileName = "cmd.exe";

                if (EtherealConfig.Config.Distribution == "Steam")
                {
                    p.StartInfo.Arguments = "/C start steam://rungameid/459220";
                }
                else if (EtherealConfig.Config.Distribution == "MS")
                {
                    p.StartInfo.Arguments = "/C start shell:AppsFolder\\Microsoft.BulldogThreshold_8wekyb3d8bbwe!xgameFinal";
                }
            }

            p.Start();
            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Game process started.");
        }

        private static async Task<Process> GetGameProcess()
        {
            Process gameProcess = null;
            int attempts = 0;
            while (gameProcess == null && attempts < 70)
            {
                await Task.Delay(100);
                gameProcess = Process.GetProcessesByName("xgameFinal").FirstOrDefault();
                attempts++;
            }
            return gameProcess;
        }

        private static void HandleGameProcessFound(Process gameProcess)
        {
            EtherealConfig.Config.CachedExePath = gameProcess.MainModule.FileName;
            EtherealConfig.Config.Save();
            EtherealLogging.Log(EtherealLogging.LogLevel.Information, $"Game process found: {gameProcess.MainModule.FileName}");

            if (EtherealConfig.Config.DiscordPresence)
            {
                GameStarted?.Invoke();

                gameProcess.EnableRaisingEvents = true;
                gameProcess.Exited += (sender, e) =>
                {
                    EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Game process exited.");
                    GameExited?.Invoke();
                };
            }
        }
    }
}
