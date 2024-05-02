using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Ethereal.GUI.Pages.UserControls
{
    public partial class NotificationControl : UserControl
    {
        private readonly string DownloadLink;

        public NotificationControl(NotificationType type, string header, Version version, string downloadLink = "")
        {
            InitializeComponent();

            NotifImage.Source = type == NotificationType.Information ? Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.NotifInformation.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) : Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.NotifError.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            LblHeader.Content = header;
            LblVersion.Content = $"v{version}";
            DownloadLink = downloadLink;

            BtnDownload.Visibility = string.IsNullOrEmpty(downloadLink) ? Visibility.Hidden : Visibility.Visible;
        }

        public enum NotificationType
        {
            Information,
            Error
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DownloadLink))
                _ = Process.Start(new ProcessStartInfo
                {
                    FileName = DownloadLink,
                    UseShellExecute = true
                });
        }
    }
}
