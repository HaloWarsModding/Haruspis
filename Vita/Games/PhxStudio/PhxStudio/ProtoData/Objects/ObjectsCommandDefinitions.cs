using System;
using Gemini.Framework.Commands;

namespace PhxStudio.ProtoData.Objects.Commands
{
	[CommandDefinition]
	sealed class ViewObjectsExplorerDefinition
		: CommandDefinition
	{
		public const string CommandName = "View.ObjectsExplorer";

		public override string Name => CommandName;

		public override string Text => "Objects Explorer";

		public override string ToolTip => "Open Objects Explorer";

		public override Uri IconSource => new Uri(
			"pack://application:,,,/Gemini;component/Resources/Icons/Open.png");
	};
}
