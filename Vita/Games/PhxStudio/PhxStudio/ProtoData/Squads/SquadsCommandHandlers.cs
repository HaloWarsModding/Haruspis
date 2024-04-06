using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;

namespace PhxStudio.ProtoData.Squads.Commands
{
	[CommandHandler]
	sealed class ViewSquadsExplorerHandler
		: Modules.ProtoData.ProtoDataViewObjectExplorerCommandHandlerBase<ViewSquadsExplorerDefinition>
	{
		public override Task Run(Command command)
		{
			Shell.ShowTool<SquadsExplorerViewModel>();
			return TaskUtility.Completed;
		}
	};
}