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

            if(ETHManager.currentMod == null)
            {
                await ETHManifest.Clear();
            }

            ETHManager.TryCacheVideo();
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
                ETHLogging.Log(ETHLogging.LogLevel.Warning, "Game process not found after 70 attempts.");
            }
        }

        public static async void SilentStart()
        {
            if (IsGameRunning())
            {
                return;
            }

            StartGameProcess();

            await CatchAndCloseProcess();

            await ETHManifest.Clear();
            ETHManager.TryCacheVideo();
        }

        public static async Task CatchAndCloseProcess()
        {
            Process gameProcess = await GetGameProcess();
            if (gameProcess != null)
            {
                HandleGameProcessFound(gameProcess);
                gameProcess.Kill();
                ETHLogging.Log(ETHLogging.LogLevel.Information, "Game process closed.");
            }
            else
            {
                ETHLogging.Log(ETHLogging.LogLevel.Warning, "Game process not found after 70 attempts.");
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

            if (!string.IsNullOrEmpty(ETHConfig.CurrentConfiguration.Game.GameExecutablePath))
            {
                p.StartInfo.FileName = ETHConfig.CurrentConfiguration.Game.GameExecutablePath;
            }
            else
            {
                p.StartInfo.FileName = "cmd.exe";

                if (ETHConfig.CurrentConfiguration.Game.CurrentDistribution == "Steam")
                {
                    p.StartInfo.Arguments = "/C start steam://rungameid/459220";
                }
                else if (ETHConfig.CurrentConfiguration.Game.CurrentDistribution == "MS")
                {
                    p.StartInfo.Arguments = "/C start shell:AppsFolder\\Microsoft.BulldogThreshold_8wekyb3d8bbwe!xgameFinal";
                }
            }

            p.Start();
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Game process started.");
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
            ETHConfig.CurrentConfiguration.Game.GameExecutablePath = gameProcess.MainModule.FileName;
            ETHConfig.CurrentConfiguration.Save();
            ETHLogging.Log(ETHLogging.LogLevel.Information, $"Game process found: {gameProcess.MainModule.FileName}");

            if (ETHConfig.CurrentConfiguration.Settings.DiscordRichPresence)
            {
                GameStarted?.Invoke();

                gameProcess.EnableRaisingEvents = true;
                gameProcess.Exited += (sender, e) =>
                {
                    ETHLogging.Log(ETHLogging.LogLevel.Information, "Game process exited.");
                    GameExited?.Invoke();
                };
            }
        }
    }
}
