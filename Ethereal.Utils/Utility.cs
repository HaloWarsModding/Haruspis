using System.Windows.Documents;

namespace Ethereal.Utils
{
    public sealed class Utility : Singleton<Utility>
    {
        private readonly DiscordRichPresence DiscordRichPresence = new();

        public static FlowDocument MarkdownToFlowDocument(string text)
        {
            return Markdown.ToFlowDocument(text);
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
