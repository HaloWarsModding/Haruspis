using System;
using System.Windows;
using System.Windows.Controls;

namespace UI.Pages.Mods.UserControls
{
    public partial class LastPlayedControl : UserControl
    {
        public LastPlayedControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LastPlayedDateProperty =
            DependencyProperty.Register("LastPlayedDate", typeof(string), typeof(LastPlayedControl), new PropertyMetadata("Today"));

        public string LastPlayedDate
        {
            get { return (string)GetValue(LastPlayedDateProperty); }
            set { SetValue(LastPlayedDateProperty, value); }
        }

        public void UpdateLastPlayedDate(DateTime lastPlayed)
        {
            string lastPlayedString = lastPlayed == DateTime.MinValue
                ? "Never"
                : lastPlayed.Date == DateTime.Today
                    ? "Today"
                    : lastPlayed.Date == DateTime.Today.AddDays(-1)
                        ? "Yesterday"
                        : lastPlayed.ToString("d MMMM");

            LastPlayedDate = lastPlayedString;
        }
    }
}
