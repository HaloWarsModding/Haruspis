using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ethereal.GUI.Pages.UserControls
{
    public partial class NotificationControl : UserControl
    {
        string DownloadLink;

        public NotificationControl(NotificationType type, string header, Version version, string downloadLink = "")
        {
            InitializeComponent();

            switch (type)
            {
                case NotificationType.Information:
                    NotifImage.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.NotifInformation.GetHbitmap(),
                                                                                  IntPtr.Zero,
                                                                                  Int32Rect.Empty,
                                                                                  BitmapSizeOptions.FromEmptyOptions());
                    break;
                case NotificationType.Error:
                    NotifImage.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.NotifError.GetHbitmap(),
                                                                                  IntPtr.Zero,
                                                                                  Int32Rect.Empty,
                                                                                  BitmapSizeOptions.FromEmptyOptions());
                    break;
            }

            LblHeader.Content = header;
            LblVersion.Content = $"v{version}";

            if(downloadLink == null)
            {
                BtnDownload.Visibility = Visibility.Hidden;
            }
        }

        public enum NotificationType
        {
            Information,
            Error
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if(DownloadLink != null)
            {
                Process.Start(DownloadLink);
            }
        }
    }
}
