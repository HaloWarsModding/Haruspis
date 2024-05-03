using System.Net.Http;
using System.Text;

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

    public class DiscordWebhook(string webhookUrl = "https://discord.com/api/webhooks/1236087950534770769/pZZpYuVBglxKgVDNYV8JfcBQzINvQBG-MwQ2eGP7e7vKQe1QkrBA1LfEXteZQ3DJyHCK")
    {
        private readonly HttpClient httpClient = new();

        public async Task SendMessage(string content)
        {
            var payload = new
            {
                content
            };

            string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

            StringContent httpContent = new(jsonPayload, Encoding.UTF8, "application/json");

            _ = await httpClient.PostAsync(webhookUrl, httpContent);
        }
    }
}



