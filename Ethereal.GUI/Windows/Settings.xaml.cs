using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ethereal.GUI.Windows
{
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
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
            Close();
        }
    }
}
