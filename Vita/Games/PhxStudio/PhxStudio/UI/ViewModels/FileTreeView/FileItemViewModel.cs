using System.Collections;
using System.Windows.Media;
using Gemini.Framework.Services;

namespace PhxStudio.UI.ViewModels.FileTreeView
{
	using IEditorProviderSelector = Modules.Main.IEditorProviderSelector;

	// #TODO_PHXSTUDIO need a service that allows configuring how file info is gathered by items
	// * How to look for file icons

	public sealed class FileItemViewModel
		: TreeViewItemBase
	{
		ImageSource mIcon;
		public ImageSource Icon
		{
			get { return mIcon; }
			set { this.SetFieldRef(ref mIcon, value); }
		}

		public IEditorProvider EditorProvider { get; private set; }
		public bool IsEditorAvailable => EditorProvider != null;

		// #HACK_PHXSTUDIO working around an issue where we get errors like:
		//BindingExpression path error: 'Children' property not found on 'object' ''FileItemViewModel' (HashCode=4583446)'. BindingExpression:Path=Children; DataItem='FileItemViewModel' (HashCode=4583446); target element is 'TreeViewItem' (Name=''); target property is 'ItemsSource' (type 'IEnumerable')
		// due to where we specify ItemsSource in the Folder's HierarchicalDataTemplate in ProjectExplorerView. Also possibly due to virturalization
		public IEnumerable Children { get { return null; } }

		public FileItemViewModel(IEditorProviderSelector editorProviderSelector, string filePath)
		{
			SetPathToFile(filePath);

			if (editorProviderSelector != null)
				EditorProvider = editorProviderSelector.GetEditor(FilePath, FileName, FileExtension);
		}

		public void LookUpIconFromOperatingSystem()
		{
			if (Icon != null)
				return;

			Icon = Utils.FileIconsLoader.GetSmallIcon(FilePath);
		}
	};
}
