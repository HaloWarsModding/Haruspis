using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using Brushes = System.Windows.Media.Brushes;

namespace Ethereal.GUI.Pages.Mods.UserControls
{
    public partial class ModControl : UserControl
    {
        public Mod CurrentMod { get; }

        public ModControl(Mod mod)
        {
            Logger.Log(LogLevel.Information, $"Initializing ModControl for mod: {mod.Name}");
            InitializeComponent();
            CurrentMod = mod;
            Logger.Log(LogLevel.Information, $"Setting mod name label: {mod.Name}");
            LblModName.Content = mod.Name;
            LoadModIcon(mod.Icon, mod.ModPath);
        }

        private void LoadModIcon(string iconPath, string modPath)
        {
            Logger.Log(LogLevel.Information, $"Loading mod icon. Path: {iconPath}");

            if (string.IsNullOrEmpty(iconPath))
            {
                Logger.Log(LogLevel.Warning, "Icon path is null or empty. Skipping icon load.");
                return;
            }

            if (!Uri.TryCreate(iconPath, UriKind.RelativeOrAbsolute, out Uri iconUri))
            {
                Logger.Log(LogLevel.Warning, "Invalid URI format for mod.Icon");
                return;
            }

            if (!iconUri.IsAbsoluteUri && iconUri.OriginalString.StartsWith("ModData\\"))
            {
                string relativePath = iconUri.OriginalString["ModData\\".Length..];
                iconUri = new Uri(Path.Combine(modPath, relativePath), UriKind.Absolute);
                Logger.Log(LogLevel.Information, $"Resolved relative URI to: {iconUri.AbsoluteUri}");
            }

            if (iconUri.IsFile)
            {
                try
                {
                    using Icon icon = new(iconUri.LocalPath);
                    Logger.Log(LogLevel.Information, $"Loaded icon from file: {iconUri.LocalPath}");
                    BitmapSource bitmap = Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    ImgMod.Source = bitmap;
                    Logger.Log(LogLevel.Information, "Icon successfully set to image source.");
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Warning, $"Failed to load icon from file: {ex.Message}");
                }
            }
            else
            {
                Logger.Log(LogLevel.Warning, "Icon URI is not a file URI. Skipping icon load.");
            }
        }

        public void ResetBackgroundColor()
        {
            Logger.Log(LogLevel.Information, "Resetting background color to transparent.");
            Background = Brushes.Transparent;
        }
    }
}
