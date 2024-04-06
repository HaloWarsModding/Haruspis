#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif

namespace KSoft.Wwise.SoundBank
{
	partial class AkSoundBank
	{
		struct AkBankHeader : IO.IEndianStreamSerializable
		{
			/// <summary>Size of this struct on disk</summary>
			public const int kSizeOf = sizeof(uint) * 4;

			public uint BankGeneratorVersion, SoundBankID, LanguageID, FeedbackSupported;

			#region IEndianStreamSerializable Members
			void SerializeOld(IO.EndianStream s)
			{
				Contract.Assert(s.IsReading);

				s.Pad32(); // Type; 0 or 1 (Init.bk)
				s.Pad32(); // LanguageID?
				s.Stream(ref BankGeneratorVersion);
				s.Pad32(); // seen as '0', '12'
				s.Pad32(); // some kind of ID
				s.Stream(ref SoundBankID);
			}
			public void Serialize(IO.EndianStream s)
			{
				uint sdk_ver = (s.Owner as AkSoundBank).SdkVersion;

				if (AkVersion.HasOldBankHeader(sdk_ver))
					SerializeOld(s);
				else
				{
					s.Stream(ref BankGeneratorVersion);
					s.Stream(ref SoundBankID);
					s.Stream(ref LanguageID);
					s.Stream(ref FeedbackSupported);
				}
			}
			#endregion
		};
	};
}