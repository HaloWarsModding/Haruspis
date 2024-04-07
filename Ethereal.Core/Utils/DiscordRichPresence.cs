using DiscordRPC;

namespace EtherealEngine
{
    public interface IDiscordRichPresence
    {
        /// <summary>
        /// Update the Discord Rich Presence with the specified details, state, large image key, large image text, button label, and button URL.
        /// </summary>
        /// <param name="details">The details to display in the Rich Presence.</param>
        /// <param name="state">The state to display in the Rich Presence.</param>
        /// <param name="largeImageKey">The key of the large image asset to display.</param>
        /// <param name="largeImageText">The tooltip text for the large image asset.</param>
        /// <param name="buttonLabel">The label for the button in the Rich Presence.</param>
        /// <param name="buttonUrl">The URL to open when the button in the Rich Presence is clicked.</param>
        void UpdatePresence(string details, string state, string largeImageKey, string largeImageText, string buttonLabel, string buttonUrl);

        /// <summary>
        /// Clear the Discord Rich Presence.
        /// </summary>
        void ClearPresence();
    }

    public class DiscordRichPresence : IDiscordRichPresence
    {
        private readonly DiscordRpcClient client;
        private readonly LogWriter logWriter;

        public DiscordRichPresence(string clientId, LogWriter logWriter)
        {
            client = new DiscordRpcClient(clientId);
            this.logWriter = logWriter;
            try
            {
                client.Initialize();
            }
            catch (Exception ex)
            {
                logWriter.Log(LogLevel.Error, "Error initializing Discord RPC client: " + ex.Message);
            }
        }

        public void UpdatePresence(string details, string state, string largeImageKey, string largeImageText, string buttonLabel, string buttonUrl)
        {
            try
            {
                client.SetPresence(new RichPresence
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
                    Buttons =
                  [
            new Button { Label = buttonLabel, Url = buttonUrl }
                  ]
                });

                logWriter.Log(LogLevel.Information, "Updating Discord Rich Presence with details: " + details);
                logWriter.Log(LogLevel.Information, "State: " + state);
                logWriter.Log(LogLevel.Information, "Large Image Key: " + largeImageKey);
                logWriter.Log(LogLevel.Information, "Large Image Text: " + largeImageText);
                logWriter.Log(LogLevel.Information, "Button Label: " + buttonLabel);
                logWriter.Log(LogLevel.Information, "Button URL: " + buttonUrl);
                logWriter.Log(LogLevel.Information, "Discord Rich Presence updated successfully");
            }
            catch (Exception ex)
            {
                logWriter.Log(LogLevel.Error, "Error updating Discord Rich Presence: " + ex.Message);
            }
        }

        public void ClearPresence()
        {
            try
            {
                client.ClearPresence();

                logWriter.Log(LogLevel.Information, "Clearing Discord Rich Presence");
                logWriter.Log(LogLevel.Information, "Discord Rich Presence cleared successfully");
            }
            catch (Exception ex)
            {
                logWriter.Log(LogLevel.Error, "Error clearing Discord Rich Presence: " + ex.Message);
            }
        }
    }
}