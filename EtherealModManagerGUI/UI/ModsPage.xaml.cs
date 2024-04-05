using EtherealModManager.Data;
using System.Windows;
using System.Windows.Input;

namespace EtherealModManager.UI
{
    public partial class ModsPage : Window
    {
        private readonly List<Mod> mods;
        private readonly string yes = "Yes";
        public ModsPage()
        {
            InitializeComponent();

            mods = ETHManager.GetMods(yes);
            foreach (var mod in mods)
            {
                ListBoxMods.Items.Add(mod.Name);
            }

            Mod vanilla = new() { Name = "Vanilla" };
            ListBoxMods.Items.Add(vanilla.Name);

            ListBoxMods.MouseDoubleClick += ListBoxMods_MouseDoubleClick;
        }

        private void ListBoxMods_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            string selectedModTitle = (string)ListBoxMods.SelectedItem;

            Mod selectedMod = mods.FirstOrDefault(mod => mod.Name == selectedModTitle);

            if (selectedMod != null)
            {
                ETHManager eth = new(yes);
                eth.SetCurrentMod(selectedMod);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
