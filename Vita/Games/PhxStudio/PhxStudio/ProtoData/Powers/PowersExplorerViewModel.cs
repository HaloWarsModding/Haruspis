using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace PhxStudio.ProtoData.Powers
{
	using Modules.ProtoData;

	[Export(typeof(PowersExplorerViewModel))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class PowersExplorerViewModel
		: ProtoDataObjectExplorerViewModel
	{
		public PowersExplorerViewModel()
		{
			DisplayName = "Powers";
			LookupViewModel = IoC.Get<PowersLookupViewModel>();
		}
	};

	[Export(typeof(PowersLookupViewModel))]
	[Export(kExportContractName, typeof(IProtoDataObjectLookup))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class PowersLookupViewModel
		: ProtoDataObjectLookupViewModel
	{
		public const string kExportContractName
			= nameof(KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database)
			+ "."
			+ nameof(KSoft.Phoenix.Phx.DatabaseObjectKind.Power)
			;

		public PowersLookupViewModel()
			: base((int)KSoft.Phoenix.Phx.DatabaseObjectKind.Power)
		{
			base.ObjectSource = new KSoft.Phoenix.Phx.ProtoDataObjectSource(
				KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database,
				KSoft.Phoenix.Phx.BProtoPower.kXmlFileInfo);
		}
	};
}
