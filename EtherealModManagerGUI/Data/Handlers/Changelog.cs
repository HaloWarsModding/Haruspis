using EtherealModManagerGUI.Logging;
using EtherealModManagerGUI.Paths;
using System.IO;
using System.Windows.Documents;

namespace EtherealModManagerGUI.Handlers
{
    internal class Changelog
    {
        public static FlowDocument ToDocument()
        {
            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Starting ToDocument method in ChangelogHandler");

            FileInfo fileInfo = new(EtherealPaths.ChangelogFile);
            if (!fileInfo.Exists || (fileInfo.Attributes & FileAttributes.Directory) != 0)
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Error, $"Markdown file not found: {EtherealPaths.ChangelogFile}");
                throw new FileNotFoundException("Markdown file not found", EtherealPaths.ChangelogFile);
            }

            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Markdown file found, starting to read the file");

            using var streamReader = new StreamReader(EtherealPaths.ChangelogFile);
            string markdownText = streamReader.ReadToEnd();

            EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Finished reading the file");

            if (!string.IsNullOrEmpty(markdownText))
            {
                EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Markdown text is not empty, starting to transform the text");

                Markdown.Xaml.Markdown markdown = new();
                var flowDocument = markdown.Transform(markdownText);

                EtherealLogging.Log(EtherealLogging.LogLevel.Information, "Markdown text transformed successfully");
                return flowDocument;
            }

            EtherealLogging.Log(EtherealLogging.LogLevel.Error, "Markdown text is empty");
            throw new Exception("Markdown text is empty");
        }
    }
}
