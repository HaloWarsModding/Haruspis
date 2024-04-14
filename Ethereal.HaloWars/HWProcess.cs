using System.Diagnostics;
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

                        OnGameStarted();
                        OnFoundProcessExecutable(HwProcess.MainModule!.FileName);
                    }
                    else if (processes.Length == 0 && HwProcess != null)
                    {
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
            GameStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameExited()
        {
            GameExited?.Invoke(this, EventArgs.Empty);
            StopMonitoring();
        }

        private void OnFoundProcessExecutable(string executablePath)
        {
            FoundProcessExecutable?.Invoke(this, executablePath);

            if (IsSilent)
            {
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
