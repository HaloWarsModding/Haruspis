using System.Windows;
using System.Windows.Input;

namespace Ethereal.GUI.Box
{
    public partial class ModsPage : Window
    {
        public ModsPage()
        {
            InitializeComponent();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void PnlDragWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRemoveMod_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddMod_Click(object sender, RoutedEventArgs e)
        {

        }


        private void ListBoxMods_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            object selectedMod = ListBoxMods.SelectedItem;
            BtnRemoveMod.IsEnabled = BtnPlayThis.IsEnabled = selectedMod != null && selectedMod.ToString() != "Vanilla";
        }

        private void BtnModPagePlayThis_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
