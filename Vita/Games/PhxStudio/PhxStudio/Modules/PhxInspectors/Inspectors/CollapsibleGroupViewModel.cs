using System.Collections.Generic;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	public sealed class CollapsibleGroupViewModel
		: InspectorBase
	{
		private static readonly Dictionary<string, bool> gPersistedExpandCollapseStates = new Dictionary<string, bool>();

		private readonly string mName;
		private readonly IEnumerable<IInspector> mChildren;

		public override string Name => mName;

		public override bool IsReadOnly => false;

		public IEnumerable<IInspector> Children => mChildren;

		private bool mIsExpanded;
		public bool IsExpanded
		{
			get { return mIsExpanded; }
			set
			{
				mIsExpanded = value;
				gPersistedExpandCollapseStates[mName] = value; // TODO: Key should be full path to this group, not just the name.
				NotifyOfPropertyChange();
			}
		}

		public CollapsibleGroupViewModel(string name, IEnumerable<IInspector> children)
		{
			mName = name;
			mChildren = children;

			if (!gPersistedExpandCollapseStates.TryGetValue(mName, out mIsExpanded))
				mIsExpanded = true;
		}
	};

	public sealed class CollapsibleGroupBuilder
		: InspectorBuilder<CollapsibleGroupBuilder>
	{
		internal CollapsibleGroupViewModel ToCollapsibleGroup(string name)
		{
			return new CollapsibleGroupViewModel(name, Inspectors);
		}
	};
}
