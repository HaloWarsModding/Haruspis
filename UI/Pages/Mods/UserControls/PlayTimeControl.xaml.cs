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

        public void UpdatePlayTime(long playTimeInSeconds)
        {
            long hours = playTimeInSeconds / 3600;
            long minutes = playTimeInSeconds % 3600 / 60;
            long seconds = playTimeInSeconds % 60;

            string playTimeString = hours >= 1
                ? $"{hours} Hour{(hours > 1 ? "s" : string.Empty)}"
                : minutes >= 1
                    ? $"{minutes} Minute{(minutes > 1 ? "s" : string.Empty)}"
                    : $"{seconds} Second{(seconds > 1 ? "s" : string.Empty)}";
            PlayTimeString = playTimeString;
        }


    }
}
