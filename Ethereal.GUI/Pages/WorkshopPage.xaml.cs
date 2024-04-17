using System.Windows;
using System.Windows.Input;

namespace Ethereal.GUI.Pages
{
    public partial class WorkshopPage : Window
    {
        public WorkshopPage()
        {
            InitializeComponent();
        }

        private void ExitPage(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DragPage(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
