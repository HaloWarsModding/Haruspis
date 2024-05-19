using System.Text;
using System.Windows.Documents;

namespace Common
{
    /// <summary>
    /// Provides methods for converting MODDB text to markdown and markdown to FlowDocument.
    /// </summary>
    public class Markdown
    {
        /// <summary>
        /// Converts MODDB formatted text to markdown.
        /// </summary>
        /// <param name="moddbText">The MODDB formatted text to convert.</param>
        /// <returns>A string containing the converted markdown.</returns>
        /// <exception cref="ArgumentException">Thrown when the input text is null or empty.</exception>
        public static string MODDBToMarkdown(string moddbText)
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

        /// <summary>
        /// Appends a line to the StringBuilder with a prefix and optional suffix.
        /// </summary>
        /// <param name="builder">The StringBuilder to append to.</param>
        /// <param name="prefix">The prefix to prepend to the line.</param>
        /// <param name="content">The content of the line.</param>
        /// <param name="suffix">The optional suffix to append to the line.</param>
        private static void AppendLineWithPrefix(StringBuilder builder, string prefix, string content, string suffix = "")
        {
            _ = builder.AppendLine($"{prefix}{content}{suffix}");
        }

        /// <summary>
        /// Converts markdown text to a FlowDocument.
        /// </summary>
        /// <param name="markdownText">The markdown text to convert.</param>
        /// <returns>A FlowDocument representing the markdown content.</returns>
        /// <exception cref="ArgumentException">Thrown when the markdown text is null or empty.</exception>
        public static FlowDocument ToFlowDocument(string markdownText)
        {
            if (string.IsNullOrEmpty(markdownText))
            {
                throw new ArgumentException("Markdown text cannot be null or empty", nameof(markdownText));
            }

            global::Markdown.Xaml.Markdown markdown = new();
            FlowDocument flowDocument = markdown.Transform(markdownText);

            return flowDocument;
        }
    }
}
