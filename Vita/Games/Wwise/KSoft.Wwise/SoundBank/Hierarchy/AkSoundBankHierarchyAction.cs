
namespace KSoft.Wwise.SoundBank
{
	using AkActionTypeStreamer = IO.EnumBinaryStreamer<AkActionType>;

	sealed class AkSoundBankHierarchyAction
		: AkSoundBankHierarchyObjectBase
	{
		public AkActionType Type;
		public uint TargetID;

		public override void Serialize(IO.EndianStream s)
		{
			base.Serialize(s);

			s.Stream(ref Type, AkActionTypeStreamer.Instance);
			s.Stream(ref TargetID);
			// There's more...
		}
	};
}