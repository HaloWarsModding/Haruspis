using System;
using System.Collections.ObjectModel;
using System.IO;

namespace PhxStudio.UI.ViewModels.FileTreeView
{
	using IEditorProviderSelector = Modules.Main.IEditorProviderSelector;

	// #TODO_PHXSTUDIO need a service that allows configuring how children are gathered for a given path
	// * Which file types to search for

	public sealed class FolderItemViewModel
		: TreeViewItemBase
	{
		IEditorProviderSelector mEditorProviderSelector;

		ObservableCollection<ITreeViewItem> mChildren;
		public ObservableCollection<ITreeViewItem> Children
		{
			get
			{
				if (mChildren == null)
				{
					// trigger INPC
					RefreshChildren();
				}

				return mChildren;
			}
			private set
			{
				this.SetField(ref mChildren, value, overrideChecks: true);
			}
		}

		public FolderItemViewModel(IEditorProviderSelector editorProviderSelector, string directoryPath)
		{
			mEditorProviderSelector = editorProviderSelector;

			base.SetPathToDirectory(directoryPath);
		}

		private ObservableCollection<ITreeViewItem> GatherChildren()
		{
			var childs = new ObservableCollection<ITreeViewItem>();
			try
			{
				foreach (var directory_name in Directory.EnumerateDirectories(FilePath))
				{
					childs.Add(new FolderItemViewModel(mEditorProviderSelector, directory_name));
				}
				foreach (var file_name in Directory.EnumerateFiles(FilePath))
				{
					childs.Add(new FileItemViewModel(mEditorProviderSelector, file_name));
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				Debug.Trace.PhxStudio.TraceData(System.Diagnostics.TraceEventType.Error, 0,
					"Encountered unauthorized access to a path",
					FilePath, ex);
			}

			return childs;
		}

		public void RefreshChildren()
		{
			this.Children = GatherChildren();
		}
	};
}
