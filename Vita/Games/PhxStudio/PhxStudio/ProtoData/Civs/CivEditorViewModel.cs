using System.ComponentModel.Composition;

namespace PhxStudio.ProtoData.Civs
{
	using Modules.ProtoData;
	using Modules.PhxInspectors;
	using Modules.PhxInspectors.Inspectors;

	[Export(typeof(CivEditorViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	class CivEditorViewModel
		: ProtoDataObjectEditorViewModel<KSoft.Phoenix.Phx.BCiv, CivsExplorerViewModel>
	{
		protected override void BuildProtoInspector(InspectablePhxObjectBuilder builder)
		{
			base.BuildProtoInspector(builder);

			base.BuildInspector(builder, Proto.UserInterfaceTextData);

			builder
				.WithCheckBoxEditor(Proto, o => o.IsExcludedFromAlpha)
				.WithCheckBoxEditor(Proto, o => o.PowerFromHero)
				.WithObjectProperty(Proto, o => o.TechID)
				.WithObjectProperty(Proto, o => o.CommandAckObjectID)
				.WithObjectProperty(Proto, o => o.RallyPointObjectID)
				.WithObjectProperty(Proto, o => o.LocalRallyPointObjectID)
				.WithObjectProperty(Proto, o => o.TransportObjectID)
				.WithObjectProperty(Proto, o => o.TransportTriggerObjectID)
				.WithObjectProperty(Proto, o => o.HullExpansionRadius)
				.WithObjectProperty(Proto, o => o.TerrainPushOffRadius)
				.WithObjectProperty(Proto, o => o.BuildingMagnetRange)
				.WithObjectProperty(Proto, o => o.SoundBank)
				.WithObjectProperty(Proto, o => o.UIControlBackground)
				;
		}

		protected override void BuildInspectorForUserInterfaceText(CollapsibleGroupBuilder group)
		{
			base.BuildInspectorForUserInterfaceText(group);

			group
				.WithObjectProperty(Proto, o => o.LeaderMenuNameID);
		}
	};
}
