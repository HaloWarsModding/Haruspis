using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;

namespace PhxStudio.ProtoData.Powers.Commands
{
	[CommandHandler]
	sealed class ViewPowersExplorerHandler
		: Modules.ProtoData.ProtoDataViewObjectExplorerCommandHandlerBase<ViewPowersExplorerDefinition>
	{
		public override Task Run(Command command)
		{
			Shell.ShowTool<PowersExplorerViewModel>();
			return TaskUtility.Completed;
		}
	};
}