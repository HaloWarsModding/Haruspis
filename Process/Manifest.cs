using Common;

namespace Process
{
    /// <summary>
    /// Represents a manifest with methods for loading from and saving to a file.
    /// </summary>
    public static class Manifest
    {
        /// <summary>
        /// Gets or sets the content of the manifest.
        /// </summary>
        public static string Content { get; set; } = string.Empty;

        /// <summary>
        /// Loads the manifest content from the specified file.
        /// </summary>
        /// <param name="path">The file path to load from.</param>
        public static void FromFile(string path)
        {
            try
            {
                using StreamReader streamReader = new(path);
                Content = streamReader.ReadToEnd();
                Console.WriteLine($"Manifest loaded successfully from file: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading manifest from file: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves the manifest content to the specified file.
        /// </summary>
        /// <param name="path">The file path to save to.</param>
        public static void ToFile(string path)
        {
            try
            {
                using StreamWriter streamWriter = new(path, false);
                streamWriter.Write(Content);
                Console.WriteLine($"Manifest saved successfully to file: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving manifest to file: {ex.Message}");
            }
        }
    }
}
