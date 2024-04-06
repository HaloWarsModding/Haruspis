
namespace KSoft.Wwise.SoundBank
{
	sealed class AkSoundBankHierarchySound
		: AkSoundBankHierarchyObjectBase
	{
		public AkBankSourceData Source = new AkBankSourceData();

		public string Name;
		public uint BankId;

		public override void Serialize(IO.EndianStream s)
		{
			base.Serialize(s);

			s.Stream(Source);
			// There's more...
		}

		internal void PrepareForExtraction(AkSoundBank bank)
		{
			if (Source.StreamType != AkBankSourceData.SourceType.Data)
				BankId = bank.Id;
			else
				BankId = Source.MediaInfo.FileID;
		}
	};
}