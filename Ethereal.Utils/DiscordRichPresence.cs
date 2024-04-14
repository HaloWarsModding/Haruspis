//-----------------------------------------------------------------------------
// File: DiscordRichPresence.cs
// Description: Contains the DiscordRichPresence class responsible for managing Discord Rich Presence.
//    This class provides functionality to update and clear Discord Rich Presence details.
//-----------------------------------------------------------------------------

using DiscordRPC;
using Ethereal.Logging;

namespace Ethereal.Utils
{
    public class DiscordRichPresence(string clientId = "1224459522278555711")
    {
        private DiscordRpcClient client;

        public bool TryInitializeClient()
        {
            try
            {
                client = new DiscordRpcClient(clientId);
                _ = client.Initialize();
                return true;
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(LogLevel.Error, "Error initializing Discord RPC client: " + ex.Message);
                return false;
            }
        }

        public void UpdatePresence(string details, string state, string largeImageKey, string largeImageText, string buttonLabel, string buttonUrl)
        {
            if (client == null)
            {
                Logger.GetInstance().Log(LogLevel.Error, "Discord RPC client is not initialized.");
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

            try
            {
                client.SetPresence(presence);

                Logger.GetInstance().Log(LogLevel.Information, "Updating Discord Rich Presence with details: " + details);
                Logger.GetInstance().Log(LogLevel.Information, "State: " + state);
                Logger.GetInstance().Log(LogLevel.Information, "Large Image Key: " + largeImageKey);
                Logger.GetInstance().Log(LogLevel.Information, "Large Image Text: " + largeImageText);
                Logger.GetInstance().Log(LogLevel.Information, "Button Label: " + buttonLabel);
                Logger.GetInstance().Log(LogLevel.Information, "Button URL: " + buttonUrl);
                Logger.GetInstance().Log(LogLevel.Information, "Discord Rich Presence updated successfully");
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(LogLevel.Error, "Error updating Discord Rich Presence: " + ex.Message);
            }
        }

        public void ClearPresence()
        {
            if (client == null)
            {
                Logger.GetInstance().Log(LogLevel.Error, "Discord RPC client is not initialized.");
                return;
            }

            try
            {
                client.ClearPresence();

                Logger.GetInstance().Log(LogLevel.Information, "Clearing Discord Rich Presence");
                Logger.GetInstance().Log(LogLevel.Information, "Discord Rich Presence cleared successfully");
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(LogLevel.Error, "Error clearing Discord Rich Presence: " + ex.Message);
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}

