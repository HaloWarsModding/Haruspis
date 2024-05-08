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
            ModsPathBox.Text = Core.config.Mods.Path;
            ErrorReportCheck.IsChecked = Core.config.Settings.SendReportOnError;
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
            Core.config.Mods.Path = ModsPathBox.Text;
            Core.config.Settings.SendReportOnError = ErrorReportCheck.IsChecked ?? false;


            Core.config.ToFile(Core.configPath);
            page.InitializeModList();
            Close();
        }
    }
}
