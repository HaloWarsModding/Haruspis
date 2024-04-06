using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace PhxStudio.Modules.TraceList.Commands
{
	[CommandHandler]
	public sealed class ViewTraceListCommandHandler
		: CommandHandlerBase<ViewTraceListCommandDefinition>
	{
		#region Imports
#pragma warning disable 649

		[Import] IShell mShell;

#pragma warning restore 649
		#endregion

		public override Task Run(Command command)
		{
			mShell.ShowTool<ITraceList>();
			return TaskUtility.Completed;
		}
	};

	[CommandHandler]
	public sealed class ClearTraceListCommandHandler
		: CommandHandlerBase<ClearTraceListCommandDefinition>
	{
		#region Imports
#pragma warning disable 649

		[Import] ITraceList mTraceList;

#pragma warning restore 649
		#endregion

		public override Task Run(Command command)
		{
			mTraceList.ClearAll();
			return TaskUtility.Completed;
		}
	};

	[CommandHandler]
	public sealed class ToggleTailTraceListCommandHandler
		: CommandHandlerBase<ToggleTailTraceListCommandDefinition>
	{
		#region Imports
#pragma warning disable 649

		[Import] ITraceList mTraceList;

#pragma warning restore 649
		#endregion

		public override Task Run(Command command)
		{
			mTraceList.TailTraces = !mTraceList.TailTraces;
			return TaskUtility.Completed;
		}

		public override void Update(Command command)
		{
			base.Update(command);

			command.Checked = mTraceList.TailTraces;
		}
	};

	[CommandHandler]
	public sealed class ToggleGroupsCommandHandler
		: ICommandHandler<ToggleCriticalGroupCommandDefinition>
		, ICommandHandler<ToggleErrorsGroupCommandDefinition>
		, ICommandHandler<ToggleWarningsGroupCommandDefinition>
		, ICommandHandler<ToggleInformationGroupCommandDefinition>
		, ICommandHandler<ToggleVerboseGroupCommandDefinition>
		, ICommandHandler<ToggleStartGroupCommandDefinition>
		, ICommandHandler<ToggleStopGroupCommandDefinition>
		, ICommandHandler<ToggleSuspendGroupCommandDefinition>
		, ICommandHandler<ToggleResumeGroupCommandDefinition>
		, ICommandHandler<ToggleTransferGroupCommandDefinition>
	{
		#region Imports
#pragma warning disable 649

		[Import] ITraceList mTraceList;

#pragma warning restore 649
		#endregion

		#region ToggleCriticalGroupCommandDefinition
		Task ICommandHandler<ToggleCriticalGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowCritical = !mTraceList.ShowCritical;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleCriticalGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Critical, mTraceList.ShowCritical);
		#endregion

		#region ToggleErrorsGroupCommandDefinition
		Task ICommandHandler<ToggleErrorsGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowError = !mTraceList.ShowError;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleErrorsGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Error, mTraceList.ShowError);
		#endregion

		#region ToggleWarningsGroupCommandDefinition
		Task ICommandHandler<ToggleWarningsGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowWarning = !mTraceList.ShowWarning;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleWarningsGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Warning, mTraceList.ShowWarning);
		#endregion

		#region ToggleInformationGroupCommandDefinition
		Task ICommandHandler<ToggleInformationGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowInformation = !mTraceList.ShowInformation;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleInformationGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Information, mTraceList.ShowInformation);
		#endregion

		#region ToggleVerboseGroupCommandDefinition
		Task ICommandHandler<ToggleVerboseGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowVerbose = !mTraceList.ShowVerbose;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleVerboseGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Verbose, mTraceList.ShowVerbose);
		#endregion

		#region ToggleStartGroupCommandDefinition
		Task ICommandHandler<ToggleStartGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowStart = !mTraceList.ShowStart;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleStartGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Start, mTraceList.ShowStart);
		#endregion

		#region ToggleStopGroupCommandDefinition
		Task ICommandHandler<ToggleStopGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowStop = !mTraceList.ShowStop;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleStopGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Stop, mTraceList.ShowStop);
		#endregion

		#region ToggleSuspendGroupCommandDefinition
		Task ICommandHandler<ToggleSuspendGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowSuspend = !mTraceList.ShowSuspend;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleSuspendGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Suspend, mTraceList.ShowSuspend);
		#endregion

		#region ToggleResumeGroupCommandDefinition
		Task ICommandHandler<ToggleResumeGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowResume = !mTraceList.ShowResume;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleResumeGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Resume, mTraceList.ShowResume);
		#endregion

		#region ToggleTransferGroupCommandDefinition
		Task ICommandHandler<ToggleTransferGroupCommandDefinition>.Run(Command command)
		{
			mTraceList.ShowTransfer = !mTraceList.ShowTransfer;
			return TaskUtility.Completed;
		}

		void ICommandHandler<ToggleTransferGroupCommandDefinition>.Update(Command command)
			=> UpdateCommand(command, TraceListItemType.Transfer, mTraceList.ShowTransfer);
		#endregion

		private void UpdateCommand(Command command, TraceListItemType type, bool showGroup)
		{
			int group_count = GetGroupCount(type);
			command.Enabled = group_count > 0;
			command.Checked = command.Enabled && showGroup;

			string text = string.Format("{0} {1}", group_count, type.ToDisplayString(group_count != 1));
			command.Text = command.ToolTip = text;
		}

		private int GetGroupCount(TraceListItemType type)
		{
			int count = 0;
			foreach (var item in mTraceList.Items)
				if (item.ItemType == type)
					count++;

			return count;
		}
	};


	[CommandHandler]
	public sealed class DebugTestTraceListCommandHandler
		: CommandHandlerBase<DebugTestTraceListCommandDefinition>
	{
		public override Task Run(Command command)
		{
			Debug.Trace.PhxStudio.TraceInformation("Testing TraceList");
			Debug.Trace.PhxStudio.TraceData(System.Diagnostics.TraceEventType.Information, 0,
				"Test TraceData",
				GetTestException(),
				new ArgumentException("Test2")
			);
			return TaskUtility.Completed;
		}

		private static Exception GetTestException()
		{
			try
			{
				Test();
			} catch (Exception e)
			{
				return e;
			}

			throw new Exception("FAILED TO TEST????");
		}

		private static void Test()
		{
			try
			{
				InnerTest();
			} catch (Exception inner)
			{
				throw new Exception("Test", inner);
			}
		}

		private static void InnerTest()
		{
			try
			{
				InnerInnerTest();
			} catch (Exception inner)
			{
				throw new Exception("Inner Test", inner);
			}
		}

		private static void InnerInnerTest() => throw new Exception("Inner Inner Test");
	};
}
