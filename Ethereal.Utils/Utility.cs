using System.Diagnostics;
using System.Net.Http;
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
            text = Markdown.ConvertModDbToMarkdown(text);

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
                // Basic system and environment details
                string osVersion = Environment.OSVersion.VersionString;
                string processorCount = Environment.ProcessorCount.ToString();
                string machineName = Environment.MachineName;
                string userName = Environment.UserName;

                // Screen details
                string screenWidth = SystemParameters.PrimaryScreenWidth.ToString();
                string screenHeight = SystemParameters.PrimaryScreenHeight.ToString();
                string screenResolution = $"{screenWidth}x{screenHeight}";

                // Process details
                string processName = Process.GetCurrentProcess().ProcessName;
                string processId = Environment.ProcessId.ToString();
                string currentDirectory = Environment.CurrentDirectory;
                string processStartTime = Process.GetCurrentProcess().StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                string processMemoryUsage = (Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024).ToString() + " MB";

                // Additional system information
                string systemDirectory = Environment.SystemDirectory;
                string userDomainName = Environment.UserDomainName;
                string currentCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
                string dotNetVersion = Environment.Version.ToString();

                // Stack trace
                StackTrace stackTrace = new(true);
                string stackTraceStr = stackTrace.ToString();

                // Report timestamp
                string reportTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Construct the full message
                string fullMessage = $"**{message}**\n\n" +
                                     $"**Report Timestamp:** {reportTimestamp}\n\n" +
                                     $"**Computer Information:**\n" +
                                     $"- OS Version: {osVersion}\n" +
                                     $"- Processor Count: {processorCount}\n" +
                                     $"- Machine Name: {machineName}\n" +
                                     $"- User Name: {userName}\n" +
                                     $"- User Domain Name: {userDomainName}\n" +
                                     $"- System Directory: {systemDirectory}\n" +
                                     $"- .NET Version: {dotNetVersion}\n" +
                                     $"- Current Culture: {currentCulture}\n\n" +
                                     $"**Screen Information:**\n" +
                                     $"- Screen Resolution: {screenResolution}\n\n" +
                                     $"**Process Information:**\n" +
                                     $"- Process Name: {processName}\n" +
                                     $"- Process ID: {processId}\n" +
                                     $"- Process Start Time: {processStartTime}\n" +
                                     $"- Memory Usage: {processMemoryUsage}\n" +
                                     $"- Current Directory: {currentDirectory}\n\n" +
                                     $"**Stack Trace:**\n{stackTraceStr}\n\n" +
                                     $"**Program Version:** {version}";

                // Send the report
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
        public static bool IsInternetAvailable()
        {
            try
            {
                using HttpClient client = new();
                client.Timeout = TimeSpan.FromSeconds(5);
                HttpResponseMessage response = client.GetAsync("http://www.google.com").Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
