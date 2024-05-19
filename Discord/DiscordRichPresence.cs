using System.Text;

using DiscordRPC;

using Newtonsoft.Json;

namespace Discord
{
    public class DiscordRichPresence(string clientId = "1224459522278555711") : IDisposable
    {
        private readonly string _clientId = clientId;
        private DiscordRpcClient _client;

        /// <summary>
        /// Initializes the Discord RPC client.
        /// </summary>
        /// <returns>True if initialization succeeds, otherwise false.</returns>
        public bool TryInitializeClient()
        {
            try
            {
                _client = new DiscordRpcClient(_clientId);
                _ = _client.Initialize();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Discord RPC client: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Updates the Discord rich presence.
        /// </summary>
        /// <param name="data">The data to update the presence with.</param>
        public void UpdatePresence(RichPresenceData data)
        {
            if (_client == null)
            {
                Console.WriteLine("Discord RPC client is not initialized.");
                return;
            }

            RichPresence presence = new()
            {
                Details = data.Details,
                State = data.State,
                Assets = new Assets
                {
                    LargeImageKey = data.LargeImageKey,
                    LargeImageText = data.LargeImageText
                },
                Timestamps = new Timestamps
                {
                    Start = DateTime.UtcNow
                },
                Buttons = Array.ConvertAll(data.Buttons.ToArray(), b => new Button { Label = b.Label, Url = b.Url })
            };

            _client.SetPresence(presence);
        }

        /// <summary>
        /// Clears the Discord rich presence.
        /// </summary>
        public void ClearPresence()
        {
            if (_client == null)
            {
                Console.WriteLine("Discord RPC client is not initialized.");
                return;
            }

            _client.ClearPresence();
        }

        /// <summary>
        /// Disposes the Discord RPC client.
        /// </summary>
        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class DiscordWebhook(string webhookUrl = "https://discord.com/api/webhooks/1236087950534770769/pZZpYuVBglxKgVDNYV8JfcBQzINvQBG-MwQ2eGP7e7vKQe1QkrBA1LfEXteZQ3DJyHCK")
    {
        private readonly string _webhookUrl = webhookUrl;
        private readonly HttpClient _httpClient = new();

        /// <summary>
        /// Sends a message to the Discord webhook.
        /// </summary>
        /// <param name="content">Content of the message.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendMessageAsync(string content)
        {
            var payload = new { content };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            StringContent httpContent = new(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_webhookUrl, httpContent);
                _ = response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Failed to send message to Discord webhook: {ex.Message}");
            }
        }
    }
}
