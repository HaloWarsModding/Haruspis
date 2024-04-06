using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;

namespace PhxStudio.ProtoData.Objects.Commands
{
	[CommandHandler]
	sealed class ViewObjectsExplorerHandler
		: Modules.ProtoData.ProtoDataViewObjectExplorerCommandHandlerBase<ViewObjectsExplorerDefinition>
	{
		public override Task Run(Command command)
		{
			Shell.ShowTool<ObjectsExplorerViewModel>();
			return TaskUtility.Completed;
		}
	};
}