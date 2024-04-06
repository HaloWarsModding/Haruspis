using System.ComponentModel.Composition;
using Gemini.Framework.Menus;

namespace PhxStudio.Modules.Main
{
	public static class MenuDefinitions
	{
		#region File
		[Export]
		public static MenuItemDefinition FileNewProject = new CommandMenuItemDefinition
			<Project.Commands.FileNewProjectCommandDefinition>(
				Gemini.Modules.MainMenu.MenuDefinitions.FileNewOpenMenuGroup, 1);

		[Export]
		public static MenuItemDefinition FileOpenProject = new CommandMenuItemDefinition
			<Project.Commands.FileOpenProjectCommandDefinition>(
				Gemini.Modules.MainMenu.MenuDefinitions.FileNewOpenMenuGroup, 2);

		[Export]
		public static MenuItemDefinition FileSaveProject = new CommandMenuItemDefinition
			<Project.Commands.FileSaveProjectCommandDefinition>(
				Gemini.Modules.MainMenu.MenuDefinitions.FileSaveMenuGroup, 1);

		[Export]
		public static MenuItemDefinition FileSaveProjectAs = new CommandMenuItemDefinition
			<Project.Commands.FileSaveProjectAsCommandDefinition>(
				Gemini.Modules.MainMenu.MenuDefinitions.FileSaveMenuGroup, 2);
		#endregion

		#region Project
		[Export]
		public static MenuDefinition ProjectMenu = new MenuDefinition(
			Gemini.Modules.MainMenu.MenuDefinitions.MainMenuBar,
			2,
			"Project");

		[Export]
		public static MenuItemGroupDefinition ProjectMenuGroup = new MenuItemGroupDefinition(
			ProjectMenu, 0);

		[Export]
		public static MenuItemDefinition ProjectPreload = new CommandMenuItemDefinition
			<Project.Commands.ProjectEnginePreloadCommandDefinition>(
				ProjectMenuGroup, 1);

		[Export]
		public static MenuItemDefinition ProjectLoad = new CommandMenuItemDefinition
			<Project.Commands.ProjectEngineLoadCommandDefinition>(
				ProjectMenuGroup, 2);
		#endregion

		#region View
		[Export]
		public static ExcludeMenuItemDefinition ExcludeViewErrorListMenuItem = new ExcludeMenuItemDefinition(
			Gemini.Modules.ErrorList.MenuDefinitions.ViewErrorListMenuItem);
		[Export]
		public static ExcludeMenuItemDefinition ExcludeViewOutputMenuItem = new ExcludeMenuItemDefinition(
			Gemini.Modules.Output.MenuDefinitions.ViewOutputMenuItem);
		[Export]
		public static ExcludeMenuItemDefinition ExcludeViewToolboxMenuItem = new ExcludeMenuItemDefinition(
			Gemini.Modules.Toolbox.MenuDefinitions.ViewToolboxMenuItem);


		[Export]
		public static MenuItemDefinition ViewProjectExplorer = new CommandMenuItemDefinition
			<ProjectExplorer.Commands.ViewProjectExplorerDefinition>(
				Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 0);

		[Export]
		public static MenuItemDefinition ViewTraceList = new CommandMenuItemDefinition
			<TraceList.Commands.ViewTraceListCommandDefinition>(
				Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 1);

		[Export]
		public static MenuItemGroupDefinition ViewProtoDataMenuGroup = new MenuItemGroupDefinition(
			Gemini.Modules.MainMenu.MenuDefinitions.ViewMenu, 10);

		[Export]
		public static MenuItemDefinition ViewProtoDataCivsExplorer = new CommandMenuItemDefinition
			<PhxStudio.ProtoData.Civs.Commands.ViewCivsExplorerDefinition>(
				ViewProtoDataMenuGroup, 0);

		[Export]
		public static MenuItemDefinition ViewProtoDataLeadersExplorer = new CommandMenuItemDefinition
			<PhxStudio.ProtoData.Leaders.Commands.ViewLeadersExplorerDefinition>(
				ViewProtoDataMenuGroup, 1);

		[Export]
		public static MenuItemDefinition ViewProtoDataObjectsExplorer = new CommandMenuItemDefinition
			<PhxStudio.ProtoData.Objects.Commands.ViewObjectsExplorerDefinition>(
				ViewProtoDataMenuGroup, 2);

		[Export]
		public static MenuItemDefinition ViewProtoDataPowersExplorer = new CommandMenuItemDefinition
			<PhxStudio.ProtoData.Powers.Commands.ViewPowersExplorerDefinition>(
				ViewProtoDataMenuGroup, 3);

		[Export]
		public static MenuItemDefinition ViewProtoDataSquadsExplorer = new CommandMenuItemDefinition
			<PhxStudio.ProtoData.Squads.Commands.ViewSquadsExplorerDefinition>(
				ViewProtoDataMenuGroup, 4);

		[Export]
		public static MenuItemDefinition ViewProtoDataTechsExplorer = new CommandMenuItemDefinition
			<PhxStudio.ProtoData.Techs.Commands.ViewTechsExplorerDefinition>(
				ViewProtoDataMenuGroup, 5);
		#endregion

		#region Debug
		[Export]
		public static MenuDefinition DebugMenu = new MenuDefinition(
			Gemini.Modules.MainMenu.MenuDefinitions.MainMenuBar,
			int.MaxValue,
			"DEBUG");

		[Export]
		public static MenuItemGroupDefinition DebugTestMenuGroup = new MenuItemGroupDefinition(
			DebugMenu, 0);
		#endregion
	};
}