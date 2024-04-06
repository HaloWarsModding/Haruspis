using System.ComponentModel.Composition;
using Gemini.Framework.ToolBars;

namespace PhxStudio.Modules.Main
{
	public static class ToolBarDefinitions
	{
		#region File
		[Export]
		public static ExcludeToolBarItemDefinition ExcludeViewErrorListMenuItem = new ExcludeToolBarItemDefinition(
			Gemini.Modules.Shell.ToolBarDefinitions.OpenFileToolBarItem);


		[Export]
		public static ToolBarItemDefinition PhxOpenFileToolBarItem = new CommandToolBarItemDefinition<Commands.PhxOpenFileCommandDefinition>(
			Gemini.Modules.Shell.ToolBarDefinitions.StandardOpenSaveToolBarGroup, 0);
		#endregion

		#region Project
		[Export]
		public static ToolBarDefinition ProjectToolBar = new ToolBarDefinition(
			0, "Project");
		[Export]
		public static ToolBarItemGroupDefinition ProjectToolBarGroup = new ToolBarItemGroupDefinition(
			ProjectToolBar, 0);

		[Export]
		public static ToolBarItemDefinition LoadProjectToolBarItem = new CommandToolBarItemDefinition<Project.Commands.ProjectLoadCommandDefinition>(
			ProjectToolBarGroup, 0, ToolBarItemDisplay.IconAndText);
		#endregion
	};
}