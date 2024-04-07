using System.Windows;

namespace Ethereal.ModManager.UI
{
    public partial class EtherealBox : Window
    {
        public enum EtherealBoxDialog
        {
            YesNo,
            Yes
        }

        public EtherealBox(EtherealBoxDialog dialog, string title, string description, string yesText, string noText = "")
        {
            InitializeComponent();

            TxtTitle.Content = title;
            TxtDescription.Text = description;
            BtnResultYes.Content = yesText;
            BtnResultNo.Content = noText;

            if (dialog == EtherealBoxDialog.Yes)
            {
                BtnResultNo.Visibility = Visibility.Collapsed;
                BtnResultYes.Width = 118;
            }
        }

        #region UI Event Handlers

        private void BtnResultYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnResultNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion
    }
}
