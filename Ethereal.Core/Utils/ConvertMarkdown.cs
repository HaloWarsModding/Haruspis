//-----------------------------------------------------------------------------
// File: ConvertMarkdown.cs
// Description: Contains the ConvertMarkdown class responsible for converting Markdown text to a FlowDocument.
//    This class provides functionality to convert Markdown text to a FlowDocument object.
//-----------------------------------------------------------------------------

using System.Windows.Documents;

namespace Ethereal.Core.Utils
{
    public interface IMarkdownConverter
    {
        /// <summary>
        /// Converts Markdown text to a FlowDocument.
        /// </summary>
        /// <param name="markdownText">The Markdown text to convert.</param>
        /// <returns>The converted FlowDocument.</returns>
        FlowDocument ToFlowDocument(string markdownText);
    }

    public class ConvertMarkdown : IMarkdownConverter
    {
        public FlowDocument ToFlowDocument(string markdownText)
        {
            if (!string.IsNullOrEmpty(markdownText))
            {
                Markdown.Xaml.Markdown markdown = new();
                FlowDocument flowDocument = markdown.Transform(markdownText);
                return flowDocument;
            }

            throw new Exception("Markdown text is empty");
        }
    }
}