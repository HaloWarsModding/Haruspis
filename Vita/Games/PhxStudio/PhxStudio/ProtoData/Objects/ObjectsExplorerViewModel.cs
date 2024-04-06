using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace PhxStudio.ProtoData.Objects
{
	using Modules.ProtoData;

	[Export(typeof(ObjectsExplorerViewModel))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class ObjectsExplorerViewModel
		: ProtoDataObjectExplorerViewModel
	{
		public ObjectsExplorerViewModel()
		{
			DisplayName = "Objects";
			LookupViewModel = IoC.Get<ObjectsLookupViewModel>();
		}
	};

	[Export(typeof(ObjectsLookupViewModel))]
	[Export(kExportContractName, typeof(IProtoDataObjectLookup))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class ObjectsLookupViewModel
		: ProtoDataObjectLookupViewModel
	{
		public const string kExportContractName
			= nameof(KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database)
			+ "."
			+ nameof(KSoft.Phoenix.Phx.DatabaseObjectKind.Object)
			;

		public ObjectsLookupViewModel()
			: base((int)KSoft.Phoenix.Phx.DatabaseObjectKind.Object)
		{
			base.ObjectSource = new KSoft.Phoenix.Phx.ProtoDataObjectSource(
				KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database,
				KSoft.Phoenix.Phx.BProtoObject.kXmlFileInfo);
		}
	};
}
