using System;
using Gemini.Framework.Commands;

namespace PhxStudio.ProtoData.Civs.Commands
{
	[CommandDefinition]
	sealed class ViewCivsExplorerDefinition
		: CommandDefinition
	{
		public const string CommandName = "View.CivsExplorer";

		public override string Name => CommandName;

		public override string Text => "Civs Explorer";

		public override string ToolTip => "Open Civs Explorer";

		public override Uri IconSource => new Uri(
			"pack://application:,,,/Gemini;component/Resources/Icons/Open.png");
	};
}
