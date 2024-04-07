using System.Diagnostics;

namespace EtherealEngine.HaloWars
{
    internal interface IGameProcess
    {
        /// <summary>
        /// Starts the game process.
        /// </summary>
        /// <param name="silent">Whether to start the game and close it instantly.</param>
        /// <param name="distribution">The distribution platform of the game.</param>
        Task StartGame(bool silent, string distribution);
    }

    public class GameProcess(LogWriter logWriter) : IGameProcess
    {
        public static string? GameExecutablePath;
        private static Process? HaloWarsProcess;
        private bool silent = false;
        public readonly LogWriter logWriter = logWriter;
        public static event Action GameExited = () => { };
        public static event Action GameStarted = () => { };

        public async Task StartGame(bool silent, string distribution)
        {
            this.silent = silent;
            logWriter.Log(LogLevel.Information, "Starting the game...");
            if (HaloWarsProcess != null && !HaloWarsProcess.HasExited)
            {
                logWriter.Log(LogLevel.Warning, "Game is already running.");
                return;
            }

            await StartGameProcess(distribution);
        }

        private async Task StartGameProcess(string distribution, string gameExecutablePath = "")
        {
            HaloWarsProcess = new Process();
            logWriter.Log(LogLevel.Information, "Starting the game process...");
            HaloWarsProcess.StartInfo.CreateNoWindow = true;

            if (!string.IsNullOrEmpty(gameExecutablePath))
            {
                HaloWarsProcess.StartInfo.FileName = gameExecutablePath;
            }
            else
            {
                HaloWarsProcess.StartInfo.FileName = "cmd.exe";

                HaloWarsProcess.StartInfo.Arguments = distribution switch
                {
                    "Steam" => "/C start steam://rungameid/459220",
                    "MS" => "shell:AppsFolder\\Microsoft.BulldogThreshold_8wekyb3d8bbwe!xgameFinal",
                    _ => throw new NotImplementedException()
                };
            }

            HaloWarsProcess.Start();
            logWriter.Log(LogLevel.Information, "Game process started.");

            await ScanForGameProcess();
        }

        private async Task<string> ScanForGameProcess()
        {
            logWriter.Log(LogLevel.Information, "Scanning for game process...");
            Process gameProcess = await GetGameProcess();
            HandleGameProcessFound(gameProcess);
            if (silent)
            {
                string exePath = gameProcess.MainModule!.FileName;
                GameExecutablePath = exePath;
                logWriter.Log(LogLevel.Information, "Game process found and killed silently.");
                gameProcess.Kill();

                return exePath;
            }

            logWriter.Log(LogLevel.Information, "Game process found and running.");
            return gameProcess.MainModule!.FileName;
        }

        private async Task<Process> GetGameProcess()
        {
            logWriter.Log(LogLevel.Information, "Getting game process...");
            Process gameProcess = null!;
            int attempts = 0;
            while (gameProcess == null && attempts < 70)
            {
                await Task.Delay(100);
                gameProcess = Process.GetProcessesByName("xgameFinal").FirstOrDefault()!;
                attempts++;
            }
            return gameProcess ?? throw new Exception("Game process not found.");
        }

        private void HandleGameProcessFound(Process gameProcess)
        {
            logWriter.Log(LogLevel.Information, "Game process found.");
            GameStarted?.Invoke();

            gameProcess.EnableRaisingEvents = true;
            gameProcess.Exited += (sender, e) =>
            {
                GameExited?.Invoke();
            };
        }
    }
}