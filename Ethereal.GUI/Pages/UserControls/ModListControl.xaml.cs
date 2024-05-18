using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Ethereal.GUI.Pages.Mods.UserControls
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
                SelectedModControl = (ModControl)ModPanel.Children[lastSelectedIndex];
                HighlightSelectedMod(SelectedModControl);
                SetCurrentMod(SelectedModControl.CurrentMod);
            }
        }

        private void OnModControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ModControl clickedModControl && clickedModControl != SelectedModControl)
            {
                ResetSelectedMod();
                SelectedModControl = clickedModControl;
                HighlightSelectedMod(clickedModControl);
                SetCurrentMod(clickedModControl.CurrentMod);
            }
        }

        public ModControl SelectedModControl { get; private set; }

        private void SetCurrentMod(Mod mod)
        {
            ModsPage.SetCurrentMod(mod);
        }

        private void HighlightSelectedMod(ModControl modControl)
        {
            modControl.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x6C, 0x75, 0x7D));
        }

        private void ResetSelectedMod()
        {
            SelectedModControl?.ResetBackgroundColor();
        }
    }
}
