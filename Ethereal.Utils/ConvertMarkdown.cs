using System.Windows.Documents;

namespace Ethereal.Utils
{
    public class Markdown
    {
        public static FlowDocument ToFlowDocument(string markdownText)
        {
            if (!string.IsNullOrEmpty(markdownText))
            {

                global::Markdown.Xaml.Markdown markdown = new();
                FlowDocument flowDocument = markdown.Transform(markdownText);

                return flowDocument;

            }

            throw new ArgumentException("Markdown text is empty", nameof(markdownText));
        }
    }
}
