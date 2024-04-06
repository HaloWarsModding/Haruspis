using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace PhxStudio.Modules.ProjectExplorer.Commands
{
	[CommandHandler]
	sealed class ViewProjectExplorerHandler
		: CommandHandlerBase<ViewProjectExplorerDefinition>
	{
		private readonly IShell mShell;

		[ImportingConstructor]
		public ViewProjectExplorerHandler(IShell shell)
		{
			mShell = shell;
		}

		public override Task Run(Command command)
		{
			mShell.ShowTool<ProjectExplorerViewModel>();
			return TaskUtility.Completed;
		}
	};
}