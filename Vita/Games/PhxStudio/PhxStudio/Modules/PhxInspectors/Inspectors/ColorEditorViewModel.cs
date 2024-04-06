using System.Windows.Media;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	public sealed class ColorEditorViewModel
		: SelectiveUndoEditorBase<Color>
		, ILabelledInspector
	{
		private bool mUsingAlphaChannel = true;

		public bool UsingAlphaChannel
		{
			get { return mUsingAlphaChannel; }
			set { this.SetFieldVal(ref mUsingAlphaChannel, value); }
		}

		public void Opened()
		{
			OnBeginEdit();
		}

		public void Closed()
		{
			OnEndEdit();
		}
	}
}
