using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace UI.Pages.Mods.UserControls
{
    /// <summary>
    /// Interaction logic for ModSettingControl.xaml
    /// </summary>
    public partial class ModSettingControl : UserControl
    {
        public Mod CurrentMod { get; set; }
        public ModsPage Page { get; set; }

        public ModSettingControl()
        {
            InitializeComponent();
        }

        public ModSettingControl(Mod mod, ModsPage modspage)
        {
            InitializeComponent();
            CurrentMod = mod;
            Page = modspage;
            BtnVersion.Content = $"V{mod.Properties.Version}";
        }

        private void BtnDeleteMod_Click(object sender, RoutedEventArgs e)
        {
            Directory.Delete(CurrentMod.Properties.ModPath, true);
            Page.PopulateModList();
        }

        private void BtnModifiers_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
