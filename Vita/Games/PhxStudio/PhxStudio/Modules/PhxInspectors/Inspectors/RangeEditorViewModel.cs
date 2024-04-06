
namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	public sealed class RangeEditorViewModel<T>
		: SelectiveUndoEditorBase<T>
		, ILabelledInspector
	{
		public T Minimum { get; private set; }
		public T Maximum { get; private set; }

		public RangeEditorViewModel(T minimum, T maximum)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public void DragStarted()
		{
			OnBeginEdit();
		}

		public void DragCompleted()
		{
			OnEndEdit();
		}
	};
}
