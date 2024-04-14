//-----------------------------------------------------------------------------
// File: Utility.cs
// Description: Contains the Utility class responsible for providing various utility methods.
//    This class includes methods for converting Markdown to FlowDocument and managing Discord Rich Presence.
//-----------------------------------------------------------------------------

using System.Windows.Documents;

namespace Ethereal.Utils
{
    public sealed class Utility
    {
        private static Utility? _instance;

        private readonly Markdown markdown = new();
        private readonly DiscordRichPresence DiscordRichPresence = new();

        private Utility() { }

        public static Utility GetInstance()
        {
            if (_instance == null)
            {
                lock (typeof(Utility))
                {
                    _instance ??= new Utility();
                }
            }
            return _instance;
        }

        public FlowDocument MarkdownToFlowDocument(string text)
        {
            return markdown.ToFlowDocument(text);
        }

        public void InitializeDiscord()
        {
            _ = DiscordRichPresence.TryInitializeClient();
        }

        public void SetDiscordPresence(string details, string state, string largeImageKey, string largeImageText, string buttonLabel, string buttonUrl)
        {
            DiscordRichPresence.ClearPresence();
            DiscordRichPresence.UpdatePresence(details, state, largeImageKey, largeImageText, buttonLabel, buttonUrl);
        }

        public void ClearDiscordPresence()
        {
            DiscordRichPresence.ClearPresence();
        }

        public void DiscordDispose()
        {
            DiscordRichPresence.Dispose();
        }
    }
}
