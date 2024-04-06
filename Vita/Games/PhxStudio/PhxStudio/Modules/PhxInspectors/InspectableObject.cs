using System.Collections.Generic;

namespace PhxStudio.Modules.PhxInspectors
{
	public interface IInspectableObject
	{
		IEnumerable<Inspectors.IInspector> Inspectors { get; }
	};

	public class InspectableObject
		: IInspectableObject
	{
		public IEnumerable<Inspectors.IInspector> Inspectors { get; set; }

		public InspectableObject(IEnumerable<Inspectors.IInspector> inspectors)
		{
			Inspectors = inspectors;
		}
	};
}
