using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using Brushes = System.Windows.Media.Brushes;

namespace UI.Pages.Mods.UserControls
{
    public partial class ModControl : UserControl
    {
        public Mod CurrentMod { get; }

        public ModControl(Mod mod)
        {
            Console.WriteLine($"Initializing ModControl for mod: {mod.Properties.Name}");
            InitializeComponent();
            CurrentMod = mod;
            Console.WriteLine($"Setting mod name label: {mod.Properties.Name}");
            LblModName.Content = mod.Properties.Name;
            LoadModIcon(mod.Properties.Icon, mod.Properties.ModPath);
        }

        private void LoadModIcon(string iconPath, string modPath)
        {
            Console.WriteLine($"Loading mod icon. Path: {iconPath}");

            if (string.IsNullOrEmpty(iconPath))
            {
                Console.WriteLine("Icon path is null or empty. Skipping icon load.");
                return;
            }

            if (!Uri.TryCreate(iconPath, UriKind.RelativeOrAbsolute, out Uri iconUri))
            {
                Console.WriteLine("Invalid URI format for mod.Icon");
                return;
            }

            if (!iconUri.IsAbsoluteUri && iconUri.OriginalString.StartsWith("ModData\\"))
            {
                string relativePath = iconUri.OriginalString["ModData\\".Length..];
                iconUri = new Uri(Path.Combine(modPath, relativePath), UriKind.Absolute);
                Console.WriteLine($"Resolved relative URI to: {iconUri.AbsoluteUri}");
            }

            if (iconUri.IsFile)
            {
                try
                {
                    using Icon icon = new(iconUri.LocalPath);
                    Console.WriteLine($"Loaded icon from file: {iconUri.LocalPath}");
                    BitmapSource bitmap = Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    ImgMod.Source = bitmap;
                    Console.WriteLine("Icon successfully set to image source.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load icon from file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Icon URI is not a file URI. Skipping icon load.");
            }
        }

        public void ResetBackgroundColor()
        {
            Console.WriteLine("Resetting background color to transparent.");
            Background = Brushes.Transparent;
        }
    }
}
