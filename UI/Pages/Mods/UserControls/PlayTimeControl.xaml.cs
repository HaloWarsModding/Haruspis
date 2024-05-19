using System.Windows;
using System.Windows.Controls;

namespace UI.Pages.Mods.UserControls

{
    public partial class PlayTimeControl : UserControl
    {
        public PlayTimeControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PlayTimeStringProperty =
            DependencyProperty.Register("PlayTimeString", typeof(string), typeof(PlayTimeControl), new PropertyMetadata("0 Hours"));

        public string PlayTimeString
        {
            get => (string)GetValue(PlayTimeStringProperty);
            set => SetValue(PlayTimeStringProperty, value);
        }

        public void UpdatePlayTime(TimeSpan playTime)
        {
            string playTimeString = playTime.TotalHours >= 1
                ? $"{(int)playTime.TotalHours} Hours"
                : playTime.TotalMinutes >= 1
                    ? $"{(int)playTime.TotalMinutes} Minutes"
                    : "0 Hours";

            PlayTimeString = playTimeString;
        }
    }
}
