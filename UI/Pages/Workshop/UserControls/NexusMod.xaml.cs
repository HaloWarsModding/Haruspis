using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Nexus;

namespace UI.Pages.Workshop.UserControls
{
    /// <summary>
    /// Interaction logic for NexusMod.xaml
    /// </summary>
    public partial class NexusMod : UserControl
    {
        private readonly Nexus.Mod CurrentMod;
        private readonly DownloadLink CurrentLink;

        public NexusMod()
        {
            InitializeComponent();
        }

        public NexusMod(Nexus.Mod mod, DownloadLink link)
        {
            InitializeComponent();

            CurrentMod = mod;
            CurrentLink = link;

            LblName.Content = CurrentMod.Name;
            LblSummary.Text = CurrentMod.Summary;

            // Set the image source from the URL
            SetImageSource(CurrentMod.PictureUrl);
        }

        private void SetImageSource(string imageUrl)
        {
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uri))
            {
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.EndInit();
                ImgPreview.Source = bitmap;
            }
            else
            {
                // Handle invalid URL
                _ = MessageBox.Show("Invalid image URL.");
            }
        }
        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            DownloadFileAsync(CurrentLink.URI);
        }

        private async void DownloadFileAsync(string url)
        {
            try
            {
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                _ = response.EnsureSuccessStatusCode();

                string fileName = GetFileNameFromUrl(url);
                string filePath = Path.Combine(Core.Main.Config.Mods.Path, fileName);

                using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                      fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    long totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    Progress<long> progress = new(bytesTransferred =>
                        ReportProgress(bytesTransferred, totalBytes));

                    await CopyToAsync(contentStream, fileStream, progress);
                }

                // Determine the file extension and extract if it's a zip or rar file
                string fileExtension = Path.GetExtension(filePath).ToLower();
                string extractedPath = null;

                if (fileExtension == ".zip")
                {
                    extractedPath = MainWindow.ExtractZipFile(filePath);
                }
                else if (fileExtension == ".rar")
                {
                    extractedPath = MainWindow.ExtractRarFile(filePath);
                }

                if (!string.IsNullOrEmpty(extractedPath))
                {
                    // Create progress for the folder move
                    Progress<double> moveProgress = new(value =>
                    {

                    });

                    // Move the extracted folder
                    await FolderMover.MoveFolderAsync(extractedPath, Core.Main.Config.Mods.Path, moveProgress);
                }

                Console.WriteLine($"File downloaded to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download file: {ex.Message}");
            }
        }

        private static async Task CopyToAsync(Stream source, Stream destination, IProgress<long> progress = null, int bufferSize = 81920)
        {
            byte[] buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }

        private void ReportProgress(long bytesTransferred, long totalBytes)
        {
            if (totalBytes != -1)
            {
                double progressPercentage = (double)bytesTransferred / totalBytes * 100;
                // Update your progress UI here, e.g., a ProgressBar or Label
                Console.WriteLine($"Progress: {progressPercentage:F2}%");
            }
            else
            {
                // Update your progress UI here for indeterminate progress
                Console.WriteLine($"Transferred {bytesTransferred} bytes");
            }
        }

        private string GetFileNameFromUrl(string url)
        {
            return Path.GetFileName(new Uri(url).LocalPath);
        }

    }
}

