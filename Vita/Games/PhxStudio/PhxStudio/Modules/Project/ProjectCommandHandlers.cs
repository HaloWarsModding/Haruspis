using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;
using Microsoft.Win32;
using KSoft;

namespace PhxStudio.Modules.Project.Commands
{
	[CommandHandler]
	class FileNewProjectCommandHandler
		: CommandHandlerBase<FileNewProjectCommandDefinition>
	{
#pragma warning disable 649
		[Import] IEventAggregator mEventAggregator;
		[Import] IProjectService mProjectService;
#pragma warning restore 649

		[Export]
		public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<FileNewProjectCommandDefinition>(
			new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift));

		public override async Task Run(Command command)
		{
			if (mProjectService.Engine != null)
			{
				mEventAggregator.PublishOnUIThread(new ProjectEngineUnloadedEventArgs());
			}

			mEventAggregator.PublishOnUIThread(new ProjectClosingEventArgs());

			var project_task = Task.Factory.StartNew(mProjectService.CreateNew,
				CancellationToken.None,
				TaskCreationOptions.None,
				TaskScheduler.Default);
			var project_task_result = await project_task;
			if (project_task_result != null)
				return;

			mEventAggregator.PublishOnUIThread(new ProjectOpeningEventArgs());
		}
	};

	[CommandHandler]
	class FileOpenProjectCommandHandler
		: CommandHandlerBase<FileOpenProjectCommandDefinition>
	{
#pragma warning disable 649
		[Import] IEventAggregator mEventAggregator;
		[Import] IProjectService mProjectService;
#pragma warning restore 649

		[Export]
		public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<FileOpenProjectCommandDefinition>(
			new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift));

		public override async Task Run(Command command)
		{
			var dialog = new OpenFileDialog();
			dialog.SetupViaEditorFileType(PhxStudioProject.FileType);
			if (dialog.ShowDialog() != true)
				return;

			if (mProjectService.Engine != null)
			{
				mEventAggregator.PublishOnUIThread(new ProjectEngineUnloadedEventArgs());
			}

			mEventAggregator.PublishOnUIThread(new ProjectClosingEventArgs());

			var project_task = Task.Factory.StartNew(OpenProjectCallback, dialog.FileName,
				CancellationToken.None,
				TaskCreationOptions.None,
				TaskScheduler.Default);
			var project_task_result = await project_task;
			if (project_task_result != null)
				return;

			mEventAggregator.PublishOnUIThread(new ProjectOpeningEventArgs());
		}

		private Exception OpenProjectCallback(object state)
		{
			var file_name = (string)state;
			return mProjectService.Open(file_name);
		}
	};

	[CommandHandler]
	class FileSaveProjectCommandHandler
		: CommandHandlerBase<FileSaveProjectCommandDefinition>
	{
#pragma warning disable 649
		[Import] IEventAggregator mEventAggregator;
		[Import] IProjectService mProjectService;
#pragma warning restore 649

		[ImportingConstructor]
		public FileSaveProjectCommandHandler(IEventAggregator eventAggregator
			, IProjectService service)
		{
			mEventAggregator = eventAggregator;
			mProjectService = service;
		}

		public override async Task Run(Command command)
		{
			var project_task = Task.Factory.StartNew(SaveProjectCallback, mProjectService,
				CancellationToken.None,
				TaskCreationOptions.None,
				TaskScheduler.Default);
			var project_task_result = await project_task;
			if (project_task_result != null)
				return;
		}

		private static Exception SaveProjectCallback(object state)
		{
			var service = (IProjectService)state;
			return service.Save();
		}

		public override void Update(Command command)
		{
			base.Update(command);

			command.Enabled = mProjectService.CurrentProject.Model.IsOnDisk;
		}
	};

	[CommandHandler]
	class FileSaveProjectAsCommandHandler
		: CommandHandlerBase<FileSaveProjectAsCommandDefinition>
	{
#pragma warning disable 649
		[Import] IEventAggregator mEventAggregator;
		[Import] IProjectService mProjectService;
#pragma warning restore 649

		[ImportingConstructor]
		public FileSaveProjectAsCommandHandler(IEventAggregator eventAggregator
			, IProjectService service)
		{
			mEventAggregator = eventAggregator;
			mProjectService = service;
		}

		public override async Task Run(Command command)
		{
			var dialog = new SaveFileDialog();
			dialog.SetupViaEditorFileType(PhxStudioProject.FileType);
			dialog.FileName = mProjectService.CurrentProjectFilePath ?? "";
			if (dialog.ShowDialog() != true)
				return;

			var project_task = Task.Factory.StartNew(SaveProjectAsCallback, dialog.FileName,
				CancellationToken.None,
				TaskCreationOptions.None,
				TaskScheduler.Default);
			var project_task_result = await project_task;
			if (project_task_result != null)
				return;
		}

		private Exception SaveProjectAsCallback(object state)
		{
			var file_name = (string)state;
			return mProjectService.Save(file_name);
		}
	};

	[CommandHandler]
	class ProjectLoadCommandHandler
		: CommandHandlerBase<ProjectLoadCommandDefinition>
	{
#pragma warning disable 649
		[Import] IProjectService mProjectService;
		[Import] TraceList.ITraceList mTraceList;
#pragma warning restore 649

		public override async Task Run(Command command)
		{
			int trace_count_for_preload = KSoft.TypeExtensions.kNone;
			int trace_count_for_load = KSoft.TypeExtensions.kNone;

			Exception preload_task_result = null;
			Exception load_task_result = null;

			var shell = IoC.Get<Main.IPhxShell>();
			shell.IsBusy = true;

			mTraceList.PauseTracing = true;
			do
			{
				int starting_trace_count = 0;

				#region Preload
				if (mProjectService.Engine == null || !mProjectService.Engine.HasAlreadyPreloaded)
				{
					starting_trace_count = mTraceList.TotalNumberOfTraces;
					var preload_task = Task.Factory.StartNew(ProjectEnginePreloadCommandHandler.PreloadEngineCallback, mProjectService,
						CancellationToken.None,
						TaskCreationOptions.None,
						TaskScheduler.Default);
					preload_task_result = await preload_task;
					trace_count_for_preload = mTraceList.TotalNumberOfTraces - starting_trace_count;
					if (preload_task_result != null)
						break;
				}
				#endregion

				#region Load
				if (mProjectService.Engine == null || !mProjectService.Engine.HasAlreadyLoaded)
				{
					starting_trace_count = mTraceList.TotalNumberOfTraces;
					var load_task = Task.Factory.StartNew(ProjectEngineLoadCommandHandler.LoadEngineCallback, mProjectService,
						CancellationToken.None,
						TaskCreationOptions.None,
						TaskScheduler.Default);
					load_task_result = await load_task;
					trace_count_for_load = mTraceList.TotalNumberOfTraces - starting_trace_count;
					if (load_task_result != null)
						break;
				}
				#endregion

			} while (false);
			mTraceList.PauseTracing = false;

			bool finished_with_problems = false;
			var event_args = new List<object>();

			#region Preload
			if (trace_count_for_preload.IsNone())
			{
				event_args.Add("Preload step failed");
				finished_with_problems = true;
			}
			else
			{
				if (trace_count_for_preload > 0)
				{
					string arg = string.Format("Traces logged during preload: {0}",
						trace_count_for_preload);
					event_args.Add(arg);
				}

				if (preload_task_result != null)
				{
					event_args.Add(preload_task_result);
					finished_with_problems = true;
				}
			}
			#endregion

			#region Load
			if (trace_count_for_load.IsNone())
			{
				event_args.Add("Load step failed");
				finished_with_problems = true;
			}
			else
			{
				if (trace_count_for_load > 0)
				{
					string arg = string.Format("Traces logged during load: {0}",
						trace_count_for_load);
					event_args.Add(arg);
				}

				if (load_task_result != null)
				{
					event_args.Add(load_task_result);
					finished_with_problems = true;
				}
			}
			#endregion

			shell.IsBusy = false;

			string message = finished_with_problems
				? "Finished loading "
				: "Finished loading (with problems) ";
			message += mProjectService.CurrentProjectFilePath;

			Debug.Trace.PhxStudio.TraceEvent(System.Diagnostics.TraceEventType.Information, KSoft.TypeExtensions.kNone,
				message,
				event_args.ToArray());
		}

		public override void Update(Command command)
		{
			base.Update(command);

			var engine = mProjectService.CurrentProject.Model.Engine;

			if (engine == null)
			{
				command.Enabled = false;
				return;
			}

			command.Enabled =
				!engine.HasAlreadyPreloaded &&
				!engine.HasAlreadyLoaded;
		}
	};

	[CommandHandler]
	class ProjectEnginePreloadCommandHandler
		: CommandHandlerBase<ProjectEnginePreloadCommandDefinition>
	{
#pragma warning disable 649
		[Import] IProjectService mProjectService;
#pragma warning restore 649

		public override async Task Run(Command command)
		{
			var project_task = Task.Factory.StartNew(PreloadEngineCallback, mProjectService,
				CancellationToken.None,
				TaskCreationOptions.None,
				TaskScheduler.Default);
			var project_task_result = await project_task;
			if (project_task_result != null)
				return;
		}

		public static Exception PreloadEngineCallback(object state)
		{
			var service = (IProjectService)state;
			return service.PreloadEngine();
		}

		public override void Update(Command command)
		{
			base.Update(command);

			var engine = mProjectService.CurrentProject.Model.Engine;

			command.Enabled = engine != null && !engine.HasAlreadyPreloaded;
		}
	};

	[CommandHandler]
	class ProjectEngineLoadCommandHandler
		: CommandHandlerBase<ProjectEngineLoadCommandDefinition>
	{
#pragma warning disable 649
		[Import] IProjectService mProjectService;
#pragma warning restore 649

		public override async Task Run(Command command)
		{
			var project_task = Task.Factory.StartNew(LoadEngineCallback, mProjectService,
				CancellationToken.None,
				TaskCreationOptions.None,
				TaskScheduler.Default);
			var project_task_result = await project_task;
			if (project_task_result != null)
				return;
		}

		public static Exception LoadEngineCallback(object state)
		{
			var service = (IProjectService)state;
			return service.LoadEngine();
		}

		public override void Update(Command command)
		{
			base.Update(command);

			var engine = mProjectService.CurrentProject.Model.Engine;

			command.Enabled = engine != null && !engine.HasAlreadyLoaded;
		}
	};
}
