using DiscordRPC;

namespace EtherealModManagerGUI.Handlers
{
    internal static class Discord
    {
        private static readonly DiscordRpcClient client;

        static Discord()
        {
            client = new DiscordRpcClient("1224459522278555711");
            client.Initialize();
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Discord RPC client initialized.");
        }

        public static void UpdatePresence(string details, string state)
        {
            client.SetPresence(new RichPresence()
            {
                Details = details,
                State = state,
                Assets = new Assets()
                {
                    LargeImageKey = "ethereal",
                    LargeImageText = "Ethereal Mod Manager"
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow
                },
                Buttons =
                [
                        new Button() { Label = "W.I.P", Url = "https://github.com/HaloWarsModding/EtherealModManager" }
                ]
            });

            ETHLogging.Log(ETHLogging.LogLevel.Information, $"Updated Discord presence: Details = {details}, State = {state}");
        }

        public static void ClearPresence()
        {
            client.ClearPresence();
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Cleared Discord presence.");
        }
    }
}
