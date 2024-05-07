using System.Diagnostics;
using System.Windows;
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
            try
            {
                string osVersion = Environment.OSVersion.VersionString;
                string processor = Environment.ProcessorCount.ToString();
                string machineName = Environment.MachineName;
                string userName = Environment.UserName;

                string screenWidth = SystemParameters.PrimaryScreenWidth.ToString();
                string screenHeight = SystemParameters.PrimaryScreenHeight.ToString();
                string screenResolution = $"{screenWidth}x{screenHeight}";

                string processName = Process.GetCurrentProcess().ProcessName;
                string processId = Environment.ProcessId.ToString();
                string currentDirectory = Environment.CurrentDirectory;

                StackTrace stackTrace = new(true);
                string stackTraceStr = stackTrace.ToString();

                string fullMessage = $"**{message}**\n\n" +
                                     $"**Computer Information:**\n" +
                                     $"- OS Version: {osVersion}\n" +
                                     $"- Processor Count: {processor}\n" +
                                     $"- Machine Name: {machineName}\n" +
                                     $"- User Name: {userName}\n" +
                                     $"- Screen Resolution: {screenResolution}\n" +
                                     $"- Process Name: {processName}\n" +
                                     $"- Process ID: {processId}\n" +
                                     $"- Current Directory: {currentDirectory}\n" +
                                     $"**Stack Trace:**\n{stackTraceStr}\n" +
                                     $"**Program Version:** {version}";

                _ = discordWebhook.SendMessage(fullMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending crash report: {ex.Message}");
            }
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
