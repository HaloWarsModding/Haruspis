using System.IO;

namespace EtherealModManagerGUI
{
    internal class ETHManifest
    {
        internal static Task Clear()
        {
            File.Delete(ETHPath.ModManifest);
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Cleared mod manifest.");
            return Task.CompletedTask;
        }

        internal static async Task Write(string path)
        {
            using (var streamWriter = new StreamWriter(ETHPath.ModManifest, false))
            {
                await streamWriter.WriteAsync(path);
            }
            ETHLogging.Log(ETHLogging.LogLevel.Information, $"Wrote mod manifest: {path}");
        }
    }
}
