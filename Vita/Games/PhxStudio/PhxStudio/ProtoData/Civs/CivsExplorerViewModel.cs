using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace PhxStudio.ProtoData.Civs
{
	using Modules.ProtoData;

	[Export(typeof(CivsExplorerViewModel))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class CivsExplorerViewModel
		: ProtoDataObjectExplorerViewModel
	{
		public CivsExplorerViewModel()
		{
			DisplayName = "Civs";
			LookupViewModel = IoC.Get<CivsLookupViewModel>();
		}

		protected override void OnOpenObject(object obj)
		{
			var vm = new CivEditorViewModel();
			vm.Proto = (KSoft.Phoenix.Phx.BCiv)obj;

			Shell.OpenDocument(vm);
		}
	};

	[Export(typeof(CivsLookupViewModel))]
	[Export(kExportContractName, typeof(IProtoDataObjectLookup))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class CivsLookupViewModel
		: ProtoDataObjectLookupViewModel
	{
		public const string kExportContractName
			= nameof(KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database)
			+ "."
			+ nameof(KSoft.Phoenix.Phx.DatabaseObjectKind.Civ)
			;

		public CivsLookupViewModel()
			: base((int)KSoft.Phoenix.Phx.DatabaseObjectKind.Civ)
		{
			base.ObjectSource = new KSoft.Phoenix.Phx.ProtoDataObjectSource(
				KSoft.Phoenix.Phx.ProtoDataObjectSourceKind.Database,
				KSoft.Phoenix.Phx.BCiv.kXmlFileInfo);

			base.mObjectsArePreloaded = false;
		}
	};
}
