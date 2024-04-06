using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;

namespace PhxStudio.ProtoData.Civs.Commands
{
	[CommandHandler]
	sealed class ViewCivsExplorerHandler
		: Modules.ProtoData.ProtoDataViewObjectExplorerCommandHandlerBase<ViewCivsExplorerDefinition>
	{
		public override Task Run(Command command)
		{
			Shell.ShowTool<CivsExplorerViewModel>();
			return TaskUtility.Completed;
		}
	};
}