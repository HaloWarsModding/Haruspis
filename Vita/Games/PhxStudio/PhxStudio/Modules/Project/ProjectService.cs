using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace PhxStudio.Modules.Project
{
	[Export(typeof(IProjectService))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class ProjectService
		: PropertyChangedBase
		, IProjectService
	{
#pragma warning disable 649
		[Import] IEventAggregator mEventAggregator;
#pragma warning restore 649

		public PhxStudioProjectViewModel CurrentProject => App.CurrentProjectViewModel;

		public KSoft.Phoenix.Engine.PhxEngine Engine => CurrentProject.Model.Engine;

		string mCurrentProjectFilePath;
		public string CurrentProjectFilePath
		{
			get { return mCurrentProjectFilePath; }
			set
			{
				if (this.SetFieldObj(ref mCurrentProjectFilePath, value))
				{
					if (mCurrentProjectFilePath != null && CurrentProject != null)
					{
						CurrentProject.Model.ProjectFilePath = mCurrentProjectFilePath;
					}
				}
			}
		}

		public Exception CreateNew()
		{
			var operation_exception = CurrentProject.CreateNewInternal();
			if (operation_exception != null)
				return operation_exception;

			CurrentProjectFilePath = CurrentProject.Model.ProjectFilePath;

			return operation_exception;
		}

		public Exception Open(string path)
		{
			var operation_exception = CurrentProject.OpenInternal(path);
			if (operation_exception != null)
				return operation_exception;

			CurrentProjectFilePath = CurrentProject.Model.ProjectFilePath;

			return operation_exception;
		}

		public Exception Save(string path)
		{
			var operation_exception = CurrentProject.SaveInternal(path);
			if (operation_exception != null)
				return operation_exception;

			CurrentProjectFilePath = CurrentProject.Model.ProjectFilePath;

			return operation_exception;
		}

		public Exception PreloadEngine()
		{
			var operation_exception = CurrentProject.PreloadEngineInternal();

			if (operation_exception == null)
			{
				mEventAggregator.PublishOnUIThread(new ProjectEnginePreloadedEventArgs(this.Engine));
			}

			return operation_exception;
		}

		public Exception LoadEngine()
		{
			var operation_exception = CurrentProject.LoadEngineInternal();

			if (operation_exception == null)
			{
				mEventAggregator.PublishOnUIThread(new ProjectEngineLoadedEventArgs(this.Engine));
			}

			return operation_exception;
		}
	};
}
