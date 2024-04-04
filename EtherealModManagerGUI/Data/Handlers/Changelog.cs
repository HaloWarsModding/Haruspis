using System.IO;
using System.Windows.Documents;

namespace EtherealModManagerGUI.Handlers
{
    internal class Changelog
    {
        public static FlowDocument ToDocument()
        {
            ETHLogging.Log(ETHLogging.LogLevel.Information, "Starting ToDocument method in ChangelogHandler");

            FileInfo fileInfo = new(ETHPath.ChangelogFile);
            if (!fileInfo.Exists || (fileInfo.Attributes & FileAttributes.Directory) != 0)
            {
                ETHLogging.Log(ETHLogging.LogLevel.Error, $"Markdown file not found: {ETHPath.ChangelogFile}");
                throw new FileNotFoundException("Markdown file not found", ETHPath.ChangelogFile);
            }

            ETHLogging.Log(ETHLogging.LogLevel.Information, "Markdown file found, starting to read the file");

            using var streamReader = new StreamReader(ETHPath.ChangelogFile);
            string markdownText = streamReader.ReadToEnd();

            ETHLogging.Log(ETHLogging.LogLevel.Information, "Finished reading the file");

            if (!string.IsNullOrEmpty(markdownText))
            {
                ETHLogging.Log(ETHLogging.LogLevel.Information, "Markdown text is not empty, starting to transform the text");

                Markdown.Xaml.Markdown markdown = new();
                var flowDocument = markdown.Transform(markdownText);

                ETHLogging.Log(ETHLogging.LogLevel.Information, "Markdown text transformed successfully");
                return flowDocument;
            }

            ETHLogging.Log(ETHLogging.LogLevel.Error, "Markdown text is empty");
            throw new Exception("Markdown text is empty");
        }
    }
}
