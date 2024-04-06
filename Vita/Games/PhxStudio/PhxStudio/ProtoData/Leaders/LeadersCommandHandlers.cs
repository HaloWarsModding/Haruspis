using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;

namespace PhxStudio.ProtoData.Leaders.Commands
{
	[CommandHandler]
	sealed class ViewLeadersExplorerHandler
		: Modules.ProtoData.ProtoDataViewObjectExplorerCommandHandlerBase<ViewLeadersExplorerDefinition>
	{
		public override Task Run(Command command)
		{
			Shell.ShowTool<LeadersExplorerViewModel>();
			return TaskUtility.Completed;
		}
	};
}