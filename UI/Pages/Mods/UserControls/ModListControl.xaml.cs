using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UI.Pages.Mods.UserControls
{
    public partial class ModListControl : UserControl
    {
        public ModListControl()
        {
            InitializeComponent();
        }

        public void InitializeModList(List<Mod> mods)
        {
            ModPanel.Children.Clear();

            foreach (Mod mod in mods)
            {
                ModControl modControl = new(mod);
                modControl.MouseDown += OnModControlMouseDown;
                _ = ModPanel.Children.Add(modControl);
            }
        }

        public void SelectLastMod(int lastSelectedIndex)
        {
            if (lastSelectedIndex >= ModPanel.Children.Count)
            {
                lastSelectedIndex = 0;
            }

            if (ModPanel.Children.Count > 0)
            {
                ModsPage.SelectedModControl = (ModControl)ModPanel.Children[lastSelectedIndex];
                SetCurrentMod(ModsPage.SelectedModControl.CurrentMod);
            }
        }

        private void OnModControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ModControl clickedModControl && clickedModControl != SelectedModControl)
            {
                ModsPage.SelectedModControl?.ResetBackgroundColor();
                ModsPage.SelectedModControl = clickedModControl;
                HighlightSelectedMod(clickedModControl);
                SetCurrentMod(clickedModControl.CurrentMod);
            }
        }

        public ModControl SelectedModControl { get; private set; }

        private static void SetCurrentMod(Mod mod)
        {
            ModsPage.SetCurrentMod(mod);
        }

        private static void HighlightSelectedMod(ModControl modControl)
        {
            modControl.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x6C, 0x75, 0x7D));
        }
    }
}
