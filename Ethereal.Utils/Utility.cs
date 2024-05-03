using System.Windows.Documents;

namespace Ethereal.Utils
{
    public sealed class Utility : Singleton<Utility>
    {
        private readonly DiscordRichPresence DiscordRichPresence = new();
        private readonly DiscordWebhook discordWebhook = new();

        public static FlowDocument MarkdownToFlowDocument(string text)
        {
            return Markdown.ToFlowDocument(text);
        }
        public void InitializeDiscord()
        {
            _ = DiscordRichPresence.TryInitializeClient();
        }
        public void SendReport(string message, Version version)
        {
            string osVersion = Environment.OSVersion.VersionString;
            string processor = Environment.ProcessorCount.ToString();
            string machineName = Environment.MachineName;
            string userName = Environment.UserName;

            string screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth.ToString();
            string screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight.ToString();
            string screenResolution = $"{screenWidth}x{screenHeight}";

            string fullMessage = $"**{message}**\n\n**Computer Information:**\n- OS Version: {osVersion}\n- Processor Count: {processor}\n- Machine Name: {machineName}\n- User Name: {userName}\n- Screen Resolution: {screenResolution}\n**Program Version:** {version}";

            _ = discordWebhook.SendMessage(fullMessage);
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
