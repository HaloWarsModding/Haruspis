using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace PhxStudio.Modules.ProjectExplorer
{
	using Modules.Project;
	using UI.ViewModels.FileTreeView;

	[Export(typeof(ProjectExplorerViewModel))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class ProjectExplorerViewModel
		: Tool
		, IHandle<ProjectOpeningEventArgs>
		, IHandle<ProjectClosingEventArgs>
		, IHandle<ProjectWorkDirectoryChangedEventArgs>
	{
		#region Imports
#pragma warning disable 649

		[Import] IShell mShell;
		[Import] IEventAggregator mEventAggregator;
		[Import] Main.IEditorProviderSelector mEditorProviderSelector;

#pragma warning restore 649
		#endregion

		FolderItemViewModel mRoot;
		public FolderItemViewModel Root
		{
			get { return mRoot; }
			private set
			{
				mRoot = value;
				if (this.SetFieldRef(ref mRoot, value, overrideChecks: true))
				{
					// trigger INPC
					Items = Items;
				}
			}
		}

		public ObservableCollection<ITreeViewItem> Items
		{
			get { return mRoot?.Children; }
			set { this.SetPropertyChanged(); }
		}

		public bool ShowAllFiles { get; set; }

		[ImportingConstructor]
		public ProjectExplorerViewModel(IEventAggregator eventAggregator)
		{
			mEventAggregator = eventAggregator;
			mEventAggregator.Subscribe(this);

			DisplayName = "Project Explorer";

			//this.ToolBarDefinition = ProjectExplorer.ToolBarDefenitions.ProjectExplorerToolBar;
		}

		public override PaneLocation PreferredLocation => PaneLocation.Left;

#if false // Activate and Deactivate aren't called on Tool :|
		protected override void OnActivate()
		{
			base.OnActivate();

			mEventAggregator.Subscribe(this);
		}

		protected override void OnDeactivate(bool close)
		{
			base.OnDeactivate(close);

			mEventAggregator.Unsubscribe(this);
		}
#endif

		public void UpdateTree()
		{
			if (Root == null)
				return;

			Root.Refresh();
			// trigger INPC
			Root = Root;
		}

		public void Open(string directory)
		{
			if (string.IsNullOrEmpty(directory))
				return;

			if (!System.IO.Directory.Exists(directory))
				return;

			Root = new FolderItemViewModel(mEditorProviderSelector, directory);
			UpdateTree();
		}

		private void Close()
		{
			Root = null;
			UpdateTree();
		}

		public void OnMouseDown(object source, FileItemViewModel fileItem, MouseButtonEventArgs args)
		{
			if (args.LeftButton == MouseButtonState.Pressed && args.ClickCount == 2)
			{
				Open(fileItem);
			}
		}

		private async void Open(FileItemViewModel file)
		{
			// #TODO_PHXSTUDIO need to figure out how to support files that require an external viewer or such tool (eg, ddx until we support in-editor viewing)

			if (!file.IsEditorAvailable)
			{
				Debug.Trace.PhxStudio.TraceEvent(System.Diagnostics.TraceEventType.Warning, 0,
					"Can't find editor for file",
					file.FilePath);
			}
			else
			{
				var editor = file.EditorProvider;
				var vm = editor.Create();

				Debug.Trace.PhxStudio.TraceInformation("Opening file with {0}: {1}",
					editor, file.FilePath);

				await editor.Open(vm, file.FilePath);
				mShell.OpenDocument(vm);
			}
		}

		private void OpenCurrentProjectWorkDir()
		{
			string work_dir = App.CurrentProjectViewModel.Model.WorkDirectory;
			Open(work_dir);
		}

		void IHandle<ProjectOpeningEventArgs>.Handle(ProjectOpeningEventArgs message) => OpenCurrentProjectWorkDir();

		void IHandle<ProjectClosingEventArgs>.Handle(ProjectClosingEventArgs message) => Close();

		void IHandle<ProjectWorkDirectoryChangedEventArgs>.Handle(ProjectWorkDirectoryChangedEventArgs message) => OpenCurrentProjectWorkDir();
	};
}
