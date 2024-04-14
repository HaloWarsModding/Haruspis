//-----------------------------------------------------------------------------
// File: Markdown.cs
// Description: Provides utility methods for converting Markdown text.
//-----------------------------------------------------------------------------

using Ethereal.Logging;
using System.Windows.Documents;

namespace Ethereal.Utils
{
    public class Markdown
    {
        public FlowDocument ToFlowDocument(string markdownText)
        {
            if (!string.IsNullOrEmpty(markdownText))
            {
                try
                {
                    global::Markdown.Xaml.Markdown markdown = new();
                    FlowDocument flowDocument = markdown.Transform(markdownText);

                    return flowDocument;
                }
                catch (Exception ex)
                {
                    Logger.GetInstance().Log(LogLevel.Error, "Error converting Markdown to FlowDocument: " + ex.Message);
                    throw;
                }
            }

            Logger.GetInstance().Log(LogLevel.Error, "Markdown text is empty");
            throw new ArgumentException("Markdown text is empty", nameof(markdownText));
        }
    }
}
