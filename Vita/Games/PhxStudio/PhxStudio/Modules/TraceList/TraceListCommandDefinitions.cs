using System;
using Gemini.Framework.Commands;

namespace PhxStudio.Modules.TraceList.Commands
{
	[CommandDefinition]
	public sealed class ViewTraceListCommandDefinition
		: CommandDefinition
	{
		public const string CommandName = "View.TraceList";

		public override string Name => CommandName;

		public override string Text => "Trace List";

		public override string ToolTip => "View the app tracing list";

		public override Uri IconSource => new Uri(
			"/Modules/TraceList/Images/Information.png",
			UriKind.Relative);
	};

	[CommandDefinition]
	public sealed class ClearTraceListCommandDefinition
		: CommandDefinition
	{
		public const string CommandName = "TraceList.Clear";

		public override string Name => CommandName;

		public override string Text => "Clear";

		public override string ToolTip => "Clears all items in the Trace List";

		public override Uri IconSource => new Uri(
			"/Modules/TraceList/Images/Clear.png",
			UriKind.Relative);
	};

	[CommandDefinition]
	public sealed class ToggleTailTraceListCommandDefinition
		: CommandDefinition
	{
		public const string CommandName = "TraceList.ToggleTail";

		public override string Name => CommandName;

		public override string Text => "Tail";

		public override string ToolTip => "When enabled, UI will snap to new traces as they come in";

		public override Uri IconSource => new Uri(
			"/Modules/TraceList/Images/Tail.png",
			UriKind.Relative);
	};

	[CommandDefinition]
	public abstract class ToggleGroupCommandDefinitionBase
		: CommandDefinition
	{
		private string mCommandName;
		private Uri mImageSource;

		protected ToggleGroupCommandDefinitionBase(string commandName, TraceListItemType groupType)
		{
			mCommandName = commandName;

			mImageSource = new Uri(string.Format("/Modules/TraceList/Images/{0}.png",
				groupType.ToString()),
				UriKind.Relative);
		}

		public override string Name => mCommandName;

		public override string Text => "[NotUsed]";

		public override string ToolTip => "[NotUsed]";

		public override Uri IconSource => mImageSource;
	};

	[CommandDefinition]
	public sealed class ToggleCriticalGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleCritical";

		public ToggleCriticalGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Critical)
		{
		}
	};

	[CommandDefinition]
	public sealed class ToggleErrorsGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleErrors";

		public ToggleErrorsGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Error)
		{
		}
	};

	[CommandDefinition]
	public sealed class ToggleWarningsGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleWarnings";

		public ToggleWarningsGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Warning)
		{
		}
	};

	[CommandDefinition]
	public sealed class ToggleInformationGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleInformation";

		public ToggleInformationGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Information)
		{
		}
	};

	[CommandDefinition]
	public sealed class ToggleVerboseGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleVerbose";

		public ToggleVerboseGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Verbose)
		{
		}
	};

	[CommandDefinition]
	public sealed class ToggleStartGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleStart";

		public ToggleStartGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Start)
		{
		}
	};

	[CommandDefinition]
	public sealed class ToggleStopGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleStop";

		public ToggleStopGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Stop)
		{
		}
	};

	[CommandDefinition]
	public sealed class ToggleSuspendGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleSuspend";

		public ToggleSuspendGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Suspend)
		{
		}
	};

	[CommandDefinition]
	public sealed class ToggleResumeGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleResume";

		public ToggleResumeGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Resume)
		{
		}
	};

	[CommandDefinition]
	public sealed class ToggleTransferGroupCommandDefinition
		: ToggleGroupCommandDefinitionBase
	{
		public const string CommandName = "TraceList.ToggleTransfer";

		public ToggleTransferGroupCommandDefinition()
			: base(CommandName, TraceListItemType.Transfer)
		{
		}
	};


	[CommandDefinition]
	public sealed class DebugTestTraceListCommandDefinition
		: CommandDefinition
	{
		public const string CommandName = "Debug.TestTraceList";

		public override string Name => CommandName;

		public override string Text => "Test Trace List";

		public override string ToolTip => "Test Trace List";
	};
}
