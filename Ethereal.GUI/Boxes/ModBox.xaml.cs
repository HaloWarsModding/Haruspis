using System.IO;
using System.Windows.Controls;

namespace Ethereal.GUI.Boxes
{

    public partial class ModBox : UserControl
    {
        public string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.xml");
        private readonly string ManifestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Halo Wars", "ModManifest.txt");
        private readonly Mod CurrentMod;

        public ModBox(Mod mod)
        {
            InitializeComponent();
            CurrentMod = mod;
            LblModTitle.Content = mod.Name;
            LblModSummary.Content = mod.Description;
            LblModVersion.Content = mod.Version;
            ImgMod.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(mod.Icon, UriKind.RelativeOrAbsolute));
        }

        private void BtnPlay_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (CurrentMod.Name == "Vanilla")
            {
                MainWindow.manifest.Content = string.Empty;
                MainWindow.config.Mods.LastPlayedMod = "Vanilla";
                MainWindow.config.ToFile(configPath);
                return;
            }

            MainWindow.manifest.Content = CurrentMod.ModPath;
            MainWindow.config.Mods.LastPlayedMod = CurrentMod.Name;
            MainWindow.config.ToFile(configPath);

            if (CurrentMod.Description != string.Empty)
            {
                MainWindow.SetChangelogContent(CurrentMod.Description, MainWindow.TimelineBox);
            }

        }

        private void BtnRemove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Mods Call
        }

        private void BtnUpdate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Workshop Call
        }
    }
}
