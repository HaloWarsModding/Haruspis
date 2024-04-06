using Caliburn.Micro;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	public interface ILabelledInspector
	{
	};

	public interface IInspector
	{
		string Name { get; }
		bool IsReadOnly { get; }
	};

	public abstract class InspectorBase
		: PropertyChangedBase
		, IInspector
	{
		public abstract string Name { get; }
		public abstract bool IsReadOnly { get; }
	};
}
