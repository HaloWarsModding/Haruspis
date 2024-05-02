using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Color = System.Windows.Media.Color;

namespace Ethereal.GUI.Pages.Mods.UserControls
{
    public partial class ModControl : UserControl
    {
        public readonly Mod currentMod;
        private bool isSelected = false;

        public ModControl(Mod mod)
        {
            InitializeComponent();
            currentMod = mod;
            LblModName.Content = mod.Name;

            if (!string.IsNullOrEmpty(mod.Icon))
            {
                if (!Uri.TryCreate(mod.Icon, UriKind.RelativeOrAbsolute, out Uri iconUri))
                {
                    Logger.Log(LogLevel.Warning, "Invalid URI format for mod.Icon");
                    return;
                }

                if (!iconUri.IsAbsoluteUri && iconUri.OriginalString.StartsWith("ModData\\"))
                {
                    string imagePath = iconUri.OriginalString["ModData\\".Length..];
                    iconUri = new Uri(Path.Combine(mod.ModPath, imagePath), UriKind.Absolute);
                }

                if (iconUri.IsFile)
                {
                    string filePath = iconUri.LocalPath;
                    try
                    {
                        Icon icon = new(filePath);
                        BitmapSource bitmap = Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                        ImgMod.Source = bitmap;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Warning, $"Failed to load icon from file: {ex.Message}");
                    }
                }
            }
        }

        private void SelectMod(object sender, MouseButtonEventArgs e)
        {
            isSelected = !isSelected;

            if (isSelected)
            {
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x6C, 0x75, 0x7D));

                ModsPage.SetCurrentMod(currentMod);
            }
            else
            {
                ResetBackgroundColor();
            }
        }

        public void ResetBackgroundColor()
        {
            Background = System.Windows.Media.Brushes.Transparent;
        }
    }
}
