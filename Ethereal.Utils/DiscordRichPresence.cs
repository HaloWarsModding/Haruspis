using DiscordRPC;

namespace Ethereal.Utils
{
    public class DiscordRichPresence(string clientId = "1224459522278555711")
    {
        private DiscordRpcClient? client;

        public bool TryInitializeClient()
        {

            client = new DiscordRpcClient(clientId);
            _ = client.Initialize();
            return true;

        }
        public void UpdatePresence(string details, string state, string largeImageKey, string largeImageText, string buttonLabel, string buttonUrl)
        {
            if (client == null)
            {
                return;
            }

            RichPresence presence = new()
            {
                Details = details,
                State = state,
                Assets = new Assets
                {
                    LargeImageKey = largeImageKey,
                    LargeImageText = largeImageText
                },
                Timestamps = new Timestamps
                {
                    Start = DateTime.UtcNow
                },
                Buttons = { }
            };


            client.SetPresence(presence);


        }
        public void ClearPresence()
        {
            if (client == null)
            {
                return;
            }

            client.ClearPresence();
        }
        public void Dispose()
        {
            client?.Dispose();
        }
    }
}

