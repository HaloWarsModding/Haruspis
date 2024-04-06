
namespace KSoft.Wwise.SoundBank
{
	abstract partial class AkSoundBankObjectBase
	{
		protected static long EndOfStream(IO.EndianStream s, AkSubchunkHeader header)
		{
			return s.BaseStream.Position + header.ChunkSize;
		}

		public abstract void Serialize(IO.EndianStream s, AkSubchunkHeader header);

		#region Factory
		static readonly Values.GroupTagData32 kGlobalSettingsSignature =
					new Values.GroupTagData32("STMG", "audiokinetic_global_settings"); // BankStateMgrChunkID

		static readonly Values.GroupTagData32 kFxParamsSignature =
					new Values.GroupTagData32("FXPR", "audiokinetic_fx_params"); // BankFXParamsChunkID
		static readonly Values.GroupTagData32 kEnvSettingsSignature =
					new Values.GroupTagData32("ENVS", "audiokinetic_env_settings"); // BankEnvSettingChunkID

		public static AkSoundBankObjectBase New(uint chunkId, uint generatorVersion)
		{
				 if (chunkId == kHierarchySignature.ID)		return NewHIRC(generatorVersion);
			else if (chunkId == kStringMappingSignature.ID)	return NewSTID(generatorVersion);
			else if (chunkId == kDataSignature.ID)			return NewDATA(generatorVersion);
			else if (chunkId == kMediaIndexSignature.ID)	return NewDIDX(generatorVersion);

			return null;
		}
		#endregion
	};
}