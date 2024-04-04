using EtherealModManagerGUI.Data;
using System.Windows;
using System.Windows.Input;

namespace EtherealModManagerGUI.UI
{
    public partial class ModsPage : Window
    {
        private List<Mod> mods;

        public ModsPage()
        {
            InitializeComponent();

            mods = ETHManager.Mods.GetMods();
            foreach (var mod in mods)
            {
                ListBoxMods.Items.Add(mod.Name);
            }

            Mod vanilla = new Mod { Name = "Vanilla" };
            ListBoxMods.Items.Add(vanilla.Name);

            ListBoxMods.MouseDoubleClick += ListBoxMods_MouseDoubleClick;
        }

        private void ListBoxMods_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            string selectedModTitle = (string)ListBoxMods.SelectedItem;

            Mod selectedMod = mods.FirstOrDefault(mod => mod.Name == selectedModTitle);

            if (selectedMod != null)
            {
                ETHManager.SetCurrentMod(selectedMod);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
