using Ethereal.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static Ethereal.Config.Configuration;

namespace Ethereal.HaloWars
{
    public class HWProcess
    {
        private static HWProcess? _instance;
        private Process? HwProcess;
        private bool IsMonitoring { get; set; }
        private bool IsSilent { get; set; }

        public event EventHandler? GameStarted;
        public event EventHandler? GameExited;
        public event EventHandler<string>? FoundProcessExecutable;

        private HWProcess() { }

        public static HWProcess GetInstance()
        {
            if (_instance == null)
            {
                lock (typeof(HWProcess))
                {
                    _instance ??= new HWProcess();
                }
            }
            return _instance;
        }

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
                    Process[] processes = Process.GetProcessesByName("xgameFinal");
                    if (processes.Length > 0 && HwProcess == null)
                    {
                        HwProcess = processes[0];
                        HwProcess.EnableRaisingEvents = true;

                        Logger.GetInstance().Log(LogLevel.Information, "Game process detected. Monitoring started.");
                        OnGameStarted();
                        OnFoundProcessExecutable(HwProcess.MainModule!.FileName);
                    }
                    else if (processes.Length == 0 && HwProcess != null)
                    {
                        Logger.GetInstance().Log(LogLevel.Information, "Game process exited. Monitoring stopped.");
                        OnGameExited();
                        HwProcess = null;
                    }

                    Thread.Sleep(100);
                }
            });
        }

        public void StopMonitoring()
        {
            IsMonitoring = false;
        }

        private void OnGameStarted()
        {
            Logger.GetInstance().Log(LogLevel.Information, "Game started.");
            GameStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameExited()
        {
            Logger.GetInstance().Log(LogLevel.Information, "Game exited.");
            GameExited?.Invoke(this, EventArgs.Empty);
            StopMonitoring();
        }

        private void OnFoundProcessExecutable(string executablePath)
        {
            Logger.GetInstance().Log(LogLevel.Information, $"Executable found: {executablePath}");
            FoundProcessExecutable?.Invoke(this, executablePath);

            if (IsSilent)
            {
                Logger.GetInstance().Log(LogLevel.Information, "Silent mode enabled. Stopping monitoring and terminating process.");
                StopMonitoring();
                HwProcess?.Kill();
                HwProcess = null;
            }
        }

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

            _ = Process.Start(startInfo);
        }
    }
}
