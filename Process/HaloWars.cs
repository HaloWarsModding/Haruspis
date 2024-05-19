using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Common;
using static Data.Configuration;

namespace Process
{
    /// <summary>
    /// Manages the game process, including starting, monitoring, and handling events for game start and exit.
    /// </summary>
    public class HaloWars : Singleton<HaloWars>
    {
        private System.Diagnostics.Process HwProcess;
        private bool IsMonitoring { get; set; }
        private bool IsSilent { get; set; }

        /// <summary>
        /// Event triggered when the game starts.
        /// </summary>
        public event EventHandler GameStarted;

        /// <summary>
        /// Event triggered when the game exits.
        /// </summary>
        public event EventHandler GameExited;

        /// <summary>
        /// Event triggered when the game process executable is found.
        /// </summary>
        public event EventHandler<string> FoundProcessExecutable;

        /// <summary>
        /// Starts the game with the specified distribution and silent mode option.
        /// </summary>
        /// <param name="distribution">The game distribution type.</param>
        /// <param name="silent">If true, enables silent mode.</param>
        /// <exception cref="NotImplementedException">Thrown when an unsupported distribution type is specified.</exception>
        public void StartGame(DistributionType distribution, bool silent)
        {
            IsSilent = silent;
            ProcessStartInfo startInfo = new()
            {
                FileName = "cmd.exe",
                Arguments = distribution switch
                {
                    DistributionType.Steam => "/C start steam://rungameid/459220",
                    DistributionType.MicrosoftStore => "shell:AppsFolder\\Microsoft.BulldogThreshold_8wekyb3d8bbwe!xgameFinal",
                    _ => throw new NotImplementedException()
                },
                CreateNoWindow = true
            };

            _ = System.Diagnostics.Process.Start(startInfo);
        }

        /// <summary>
        /// Starts monitoring the game process.
        /// </summary>
        public void StartMonitoring()
        {
            if (IsMonitoring)
            {
                return;
            }

            IsMonitoring = true;

            _ = Task.Run(() =>
            {
                while (IsMonitoring)
                {
                    try
                    {
                        var processes = System.Diagnostics.Process.GetProcessesByName("xgameFinal");
                        if (processes.Length > 0 && HwProcess == null)
                        {
                            HwProcess = processes[0];
                            HwProcess.EnableRaisingEvents = true;
                            HwProcess.Exited += (sender, e) =>
                            {
                                Console.WriteLine("Game process exited. Monitoring stopped.");
                                OnGameExited();
                                HwProcess = null;
                            };

                            Console.WriteLine("Game process detected. Monitoring started.");
                            OnGameStarted();
                            OnFoundProcessExecutable(HwProcess.MainModule?.FileName ?? "Unknown");
                        }
                        else if (processes.Length == 0 && HwProcess != null)
                        {
                            Console.WriteLine("Game process exited unexpectedly. Monitoring stopped.");
                            OnGameExited();
                            HwProcess = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while monitoring the process: {ex.Message}");
                    }

                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// Stops monitoring the game process.
        /// </summary>
        public void StopMonitoring()
        {
            IsMonitoring = false;
        }

        /// <summary>
        /// Invokes the GameStarted event.
        /// </summary>
        private void OnGameStarted()
        {
            Console.WriteLine("Game started.");
            GameStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invokes the GameExited event.
        /// </summary>
        private void OnGameExited()
        {
            Console.WriteLine("Game exited.");
            GameExited?.Invoke(this, EventArgs.Empty);
            StopMonitoring();
        }

        /// <summary>
        /// Invokes the FoundProcessExecutable event with the specified executable path.
        /// </summary>
        /// <param name="executablePath">The path to the executable.</param>
        private void OnFoundProcessExecutable(string executablePath)
        {
            Console.WriteLine($"Executable found: {executablePath}");
            FoundProcessExecutable?.Invoke(this, executablePath);

            if (IsSilent)
            {
                Console.WriteLine("Silent mode enabled. Stopping monitoring and terminating process.");
                StopMonitoring();
                HwProcess?.Kill();
                HwProcess = null;
            }
        }
    }
}
