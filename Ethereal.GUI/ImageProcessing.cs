using System.IO;
using System.Windows.Media.Imaging;

namespace Ethereal.GUI
{
    public static class ImageProcessing
    {
        public static BitmapImage LoadImageFromFile(string path)
        {
            BitmapImage bitmap = new();
            if (!string.IsNullOrEmpty(path))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
            }
            return bitmap;
        }

        public static BitmapImage LoadDefaultImage()
        {
            BitmapImage bitmap = new();
            using (MemoryStream stream = new())
            {
                Properties.Resources.Background.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            return bitmap;
        }

        public static BitmapImage LoadBannerImage(Mod mod)
        {
            BitmapImage bitmap = new();
            string bannerPath = mod.Banner;
            string path;

            switch (bannerPath)
            {
                case "":
                    bitmap = LoadDefaultImage();
                    break;
                case string bPath when bPath.StartsWith("ModData\\"):
                    path = Path.Combine(mod.ModPath, bPath["ModData\\".Length..]);
                    bitmap = LoadImageFromFile(path);
                    break;
                case string bPath when File.Exists(bPath):
                    path = bPath;
                    bitmap = LoadImageFromFile(path);
                    break;
                default:
                    break;
            }

            return bitmap;
        }
    }

}
