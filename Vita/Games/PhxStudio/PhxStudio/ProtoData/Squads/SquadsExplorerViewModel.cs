using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace PhxStudio.ProtoData.Squads
{
	using Modules.ProtoData;

	[Export(typeof(SquadsExplorerViewModel))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class SquadsExplorerViewModel
		: ProtoDataObjectExplorerViewModel
	{
		public SquadsExplorerViewModel()
		{
			DisplayName = "Squads";
			LookupViewModel = IoC.Get<SquadsLookupViewModel>();
		}
	};

	[Export(typeof(SquadsLookupViewModel))]
	[Export(kExportContractName, typeof(IProtoDataObjectLookup))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class SquadsLookupViewModel
		: ProtoDataObjectLookupViewModel
	{
		public const string kExportContractName
			= nameof(KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database)
			+ "."
			+ nameof(KSoft.Phoenix.Phx.DatabaseObjectKind.Squad)
			;

		public SquadsLookupViewModel()
			: base((int)KSoft.Phoenix.Phx.DatabaseObjectKind.Squad)
		{
			base.ObjectSource = new KSoft.Phoenix.Phx.ProtoDataObjectSource(
				KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database,
				KSoft.Phoenix.Phx.BProtoSquad.kXmlFileInfo);
		}
	};
}
