using System;
using System.Windows.Controls;
using System.Windows.Media;
using ColorEventArgs = Gemini.Modules.Inspector.Controls.ColorEventArgs;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	public partial class ColorEditorView : UserControl
	{
		private Color mOriginalColor;
		private ColorEditorViewModel ViewModel { get { return (ColorEditorViewModel)DataContext; } }

		public ColorEditorView()
		{
			InitializeComponent();
		}

		private void OnScreenColorPickerPickingStarted(object sender, EventArgs e)
		{
			mOriginalColor = ViewModel.Value;
		}

		private void OnScreenColorPickerPickingCancelled(object sender, EventArgs e)
		{
			ViewModel.Value = mOriginalColor;
		}

		private void OnScreenColorPickerColorHovered(object sender, ColorEventArgs e)
		{
			ViewModel.Value = e.Color;
		}

		private void OnScreenColorPickerColorPicked(object sender, ColorEventArgs e)
		{
			ViewModel.Value = e.Color;
		}
	}
}
