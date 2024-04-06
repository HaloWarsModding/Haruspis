using System.ComponentModel.Composition;
using Gemini.Framework;

namespace PhxStudio.Modules.Startup
{
	[Export(typeof(IModule))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	sealed class StartupModule
		: ModuleBase
	{
		public override void Initialize()
		{
			MainWindow.Title = "PhxStudio";
			MainWindow.Icon = App.AppIconBitmap;
		}
	};
}
