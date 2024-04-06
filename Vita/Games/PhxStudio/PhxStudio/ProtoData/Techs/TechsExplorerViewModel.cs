using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace PhxStudio.ProtoData.Techs
{
	using Modules.ProtoData;

	[Export(typeof(TechsExplorerViewModel))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class TechsExplorerViewModel
		: ProtoDataObjectExplorerViewModel
	{
		public TechsExplorerViewModel()
		{
			DisplayName = "Techs";
			LookupViewModel = IoC.Get<TechsLookupViewModel>();
		}
	};

	[Export(typeof(TechsLookupViewModel))]
	[Export(kExportContractName, typeof(IProtoDataObjectLookup))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class TechsLookupViewModel
		: ProtoDataObjectLookupViewModel
	{
		public const string kExportContractName
			= nameof(KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database)
			+ "."
			+ nameof(KSoft.Phoenix.Phx.DatabaseObjectKind.Tech)
			;

		public TechsLookupViewModel()
			: base((int)KSoft.Phoenix.Phx.DatabaseObjectKind.Tech)
		{
			base.ObjectSource = new KSoft.Phoenix.Phx.ProtoDataObjectSource(
				KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database,
				KSoft.Phoenix.Phx.BProtoTech.kXmlFileInfo);
		}
	};
}
