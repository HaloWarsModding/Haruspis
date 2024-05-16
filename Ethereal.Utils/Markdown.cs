using System.Text;
using System.Windows.Documents;

namespace Ethereal.Utils
{
    public class Markdown
    {
        public static string ConvertModDbToMarkdown(string moddbText)
        {
            if (string.IsNullOrEmpty(moddbText))
            {
                throw new ArgumentException("Input text cannot be null or empty", nameof(moddbText));
            }

            StringBuilder markdownText = new();
            string[] lines = moddbText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            bool insideChangesSection = false;

            Dictionary<Func<string, bool>, Action<StringBuilder, string>> actionMap = new()
            {
            { line => line.StartsWith("----------------------------"), (sb, line) => insideChangesSection = !insideChangesSection },
            { line => insideChangesSection, (sb, line) => AppendLineWithPrefix(sb, "## **", line, "**") },
            { line => line.StartsWith('[') && line.EndsWith(']'), (sb, line) => AppendLineWithPrefix(sb, "## ", line.Trim('[', ']')) },
            { line => line.StartsWith("==") && line.EndsWith("=="), (sb, line) => AppendLineWithPrefix(sb, "**", line.Trim('=', ' '), "**") },
            { line => line.StartsWith("- ") && line.EndsWith(" -"), (sb, line) => AppendLineWithPrefix(sb, "### ", line.Trim('-', ' ')) },
            { line => line.StartsWith("- "), (sb, line) => sb.AppendLine(line) },
            { line => line.StartsWith("> "), (sb, line) => AppendLineWithPrefix(sb, "* ", line.Trim('>', ' ')) },
            { line => line.StartsWith("+ "), (sb, line) => AppendLineWithPrefix(sb, "* ", line.Trim('+', ' ')) },
            { string.IsNullOrWhiteSpace, (sb, line) => sb.AppendLine() },
            { line => true, (sb, line) => sb.AppendLine(line) }
        };

            foreach (string line in lines)
            {
                foreach (KeyValuePair<Func<string, bool>, Action<StringBuilder, string>> condition in actionMap)
                {
                    if (condition.Key(line))
                    {
                        condition.Value(markdownText, line);
                        break;
                    }
                }
            }

            return markdownText.ToString();
        }

        private static void AppendLineWithPrefix(StringBuilder builder, string prefix, string content, string suffix = "")
        {
            _ = builder.AppendLine($"{prefix}{content}{suffix}");
        }

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
