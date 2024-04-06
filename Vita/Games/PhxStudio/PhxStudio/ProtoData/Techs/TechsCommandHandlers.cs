using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;

namespace PhxStudio.ProtoData.Techs.Commands
{
	[CommandHandler]
	sealed class ViewTechsExplorerHandler
		: Modules.ProtoData.ProtoDataViewObjectExplorerCommandHandlerBase<ViewTechsExplorerDefinition>
	{
		public override Task Run(Command command)
		{
			Shell.ShowTool<TechsExplorerViewModel>();
			return TaskUtility.Completed;
		}
	};
}