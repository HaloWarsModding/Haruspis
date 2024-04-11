//-----------------------------------------------------------------------------
// File: GameProcess.cs
// Description: Contains the GameProcess class responsible for managing the game process in Halo Wars.
//    This class provides functionality to start the game process and handle game events.
//-----------------------------------------------------------------------------

using Ethereal.Core.Logging;
using System.Diagnostics;

namespace Ethereal.Core.HaloWars
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
        public string? GameExecutablePath;
        public Process? HaloWarsProcess;
        private bool silent = false;
        public readonly LogWriter logWriter = logWriter;

        public event Action GameExited = () => { };
        public event Action GameStarted = () => { };

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task StartGame(bool silent, string distribution)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            this.silent = silent;
            logWriter.Log(LogLevel.Information, "Starting the game...");
            if (HaloWarsProcess != null && !HaloWarsProcess.HasExited)
            {
                logWriter.Log(LogLevel.Warning, "Game is already running.");
                return;
            }

            logWriter.Log(LogLevel.Information, "Starting the game process...");

            HaloWarsProcess = new Process();
            HaloWarsProcess.StartInfo.CreateNoWindow = true;

            if (!string.IsNullOrEmpty(GameExecutablePath))
            {
                HaloWarsProcess.StartInfo.FileName = GameExecutablePath;
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

            _ = HaloWarsProcess.Start();
            logWriter.Log(LogLevel.Information, "Game process started.");

            logWriter.Log(LogLevel.Information, "Scanning for game process...");

            Process? gameProcess = null;
            for (int attempts = 0; attempts < 70; attempts++)
            {
                Thread.Sleep(100);
                Process[] processes = Process.GetProcessesByName("xgameFinal");
                if (processes.Length > 0)
                {
                    gameProcess = processes[0];
                    break;
                }
            }

            if (gameProcess != null)
            {
                logWriter.Log(LogLevel.Information, "Game process found.");
                if (!this.silent)
                {
                    GameStarted?.Invoke();
                    gameProcess.EnableRaisingEvents = true;
                    gameProcess.Exited += (sender, e) =>
                    {
                        GameExited?.Invoke();
                    };
                }

                if (this.silent)
                {
                    string exePath = gameProcess.MainModule!.FileName;
                    GameExecutablePath = exePath;
                    logWriter.Log(LogLevel.Information, "Game process found and killed silently.");
                    gameProcess.Kill();
                }
                else
                {
                    logWriter.Log(LogLevel.Information, "Game process found and running.");
                }
            }
        }
    }
}