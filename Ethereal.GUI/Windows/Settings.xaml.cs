using System.Windows;
using System.Windows.Input;

using Ethereal.GUI.Pages;

namespace Ethereal.GUI.Windows
{
    public partial class Settings : Window
    {
        private readonly ModsPage page;
        public Settings(ModsPage modsPage)
        {
            InitializeComponent();
            page = modsPage;
            ModsPathBox.Text = Core.Config.Mods.Path;
            ErrorReportCheck.IsChecked = Core.Config.Settings.SendReportOnError;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            Core.Config.Mods.Path = ModsPathBox.Text;
            Core.Config.Settings.SendReportOnError = ErrorReportCheck.IsChecked ?? false;


            Core.Config.ToFile(Core.ConfigPath);
            page.PopulateModList();
            Close();
        }
    }
}
