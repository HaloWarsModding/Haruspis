using System.ComponentModel.Composition;
using Gemini.Framework.Menus;

namespace PhxStudio.Modules.TraceList
{
	public static class MenuDefinitions
	{
		[Export]
		public static MenuItemDefinition DebugTestTraceList = new CommandMenuItemDefinition
			<Commands.DebugTestTraceListCommandDefinition>(
				Main.MenuDefinitions.DebugTestMenuGroup, 0);
	};
}
