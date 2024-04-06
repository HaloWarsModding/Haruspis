using System;
using System.Collections.Generic;
using Gemini.Framework;

namespace PhxStudio.Modules.PhxInspectors
{
	public class PhxInspectorViewModel
		: Document
	{
		IInspectableObject mInspectableModel;
		public IInspectableObject InspectableModel
		{
			get { return mInspectableModel; }
			set { this.SetField(ref mInspectableModel, value); }
		}

		public void HandleViewLoaded()
		{
			if (InspectableModel == null)
				return;

			RecurseEditors(InspectableModel.Inspectors, HandleViewLoadedAction);
		}

		private static void HandleViewLoadedAction(Inspectors.IEditor editor)
		{
			if (editor != null)
				editor.HandleViewLoaded();
		}

		public void ResetAll()
		{
			if (InspectableModel == null)
				return;

			RecurseEditors(InspectableModel.Inspectors, ResetEditorAction);
		}

		private static void ResetEditorAction(Inspectors.IEditor editor)
		{
			if (editor != null && editor.CanReset)
				editor.Reset();
		}

		public void RecurseEditors(IEnumerable<Inspectors.IInspector> inspectors, Action<Inspectors.IEditor> action)
		{
			foreach (var inspector in inspectors)
			{
				var group = inspector as Inspectors.CollapsibleGroupViewModel;
				if (group != null)
				{
					RecurseEditors(group.Children, action);
				}
				else
				{
					action(inspector as Inspectors.IEditor);
				}
			}
		}
	};
}
