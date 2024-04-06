using System.ComponentModel.Composition;
using Gemini.Framework.ToolBars;

namespace PhxStudio.Modules.TraceList
{
	using Commands;

	public static class ToolBarDefinitions
	{
		public static ToolBarDefinition TraceListToolBar = new ToolBarDefinition(0, "TraceList");
		private static int gSortOrder = -1;

		[Export]
		public static ToolBarItemGroupDefinition TraceListOperationsGroup = new ToolBarItemGroupDefinition(
			TraceListToolBar, ++gSortOrder);

		[Export]
		public static ToolBarItemDefinition ClearTraceListToolBarItem = new CommandToolBarItemDefinition<ClearTraceListCommandDefinition>(
			TraceListOperationsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleTailTraceListToolBarItem = new CommandToolBarItemDefinition<ToggleTailTraceListCommandDefinition>(
			TraceListOperationsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);


		[Export]
		public static ToolBarItemGroupDefinition TraceListToggleEventsGroup = new ToolBarItemGroupDefinition(
			TraceListToolBar, ++gSortOrder);

		[Export]
		public static ToolBarItemDefinition ToggleCriticalToolBarItem = new CommandToolBarItemDefinition<ToggleCriticalGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleErrorsToolBarItem = new CommandToolBarItemDefinition<ToggleErrorsGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleWarningsToolBarItem = new CommandToolBarItemDefinition<ToggleWarningsGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleInformationToolBarItem = new CommandToolBarItemDefinition<ToggleInformationGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleVerboseToolBarItem = new CommandToolBarItemDefinition<ToggleVerboseGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleStartToolBarItem = new CommandToolBarItemDefinition<ToggleStartGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleStopToolBarItem = new CommandToolBarItemDefinition<ToggleStopGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleSuspendToolBarItem = new CommandToolBarItemDefinition<ToggleSuspendGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleResumeToolBarItem = new CommandToolBarItemDefinition<ToggleResumeGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
		[Export]
		public static ToolBarItemDefinition ToggleTransferToolBarItem = new CommandToolBarItemDefinition<ToggleTransferGroupCommandDefinition>(
			TraceListToggleEventsGroup, gSortOrder, ToolBarItemDisplay.IconAndText);
	};
}
