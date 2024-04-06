using System.Collections.Generic;

namespace KSoft.Wwise.FilePackage
{
	public sealed class AkFilePackage
		: IO.IEndianStreamSerializable
	{
		internal AkFilePackageSettings Settings { get; private set; }

		AkFilePackageHeader mHeader;
		AkLanguageMap mLangMap;
		AkFileLookupTable mSoundBanksTable;
		AkFileLookupTable mStreamedFilesTable;
		AkFileLookupTable mExternalFilesTable;

		SoundBank.AkSoundBank[] mSoundBanks;
		Dictionary<uint, string> mIdToName;
		List<KeyValuePair<uint, string>> mIdToNameDups;

		bool HasExternalFiles { get {
			return mExternalFilesTable != null;
		} }

		public AkFilePackage(AkFilePackageSettings settings)
		{
			Settings = settings;

			mLangMap = new AkLanguageMap(settings.UseAsciiStrings);

			mSoundBanksTable = new AkFileLookupTable();
			mStreamedFilesTable = new AkFileLookupTable();

			if (AkVersion.HasExternalFiles(settings.SdkVersion))
				mExternalFilesTable = new AkFileLookupTable();
		}

		public IReadOnlyDictionary<uint, string> IdToName { get { return mIdToName; } }
		public IEnumerable<SoundBank.AkSoundBank> SoundBanks { get { return mSoundBanks; } }

		internal void MapIdToName(uint id, string name)
		{
			string existing_name;
			if (mIdToName.TryGetValue(id, out existing_name))
			{
				if (existing_name != name)
					mIdToNameDups.Add(new KeyValuePair<uint, string>(id, name));
			}
			else
				mIdToName.Add(id, name);
		}

		internal AkFileLookupTableEntry FindStreamedFileById(ulong streamedFileId)
		{
			return mStreamedFilesTable.Find(streamedFileId);
		}

		#region IEndianStreamSerializable Members
		public void Serialize(IO.EndianStream s)
		{
			s.Owner = this;

			if(s.IsWriting)
				mHeader.InitializeSize(Settings.SdkVersion, mLangMap.TotalMapSize);

			s.Stream(ref mHeader);
			s.Stream(ref mLangMap.TotalMapSize);
			s.Stream(ref mSoundBanksTable.TotalSize);
			s.Stream(ref mStreamedFilesTable.TotalSize);
			if (HasExternalFiles) s.Stream(ref mExternalFilesTable.TotalSize);
			s.Stream(mLangMap);
			s.Stream(mSoundBanksTable);
			s.Stream(mStreamedFilesTable);
			if (HasExternalFiles) s.Stream(mExternalFilesTable);
		}

		public void SerializeSoundBanks(IO.EndianStream s)
		{
			if (s.IsReading)
			{
				mSoundBanks = new SoundBank.AkSoundBank[mSoundBanksTable.Count];
				mIdToName = new Dictionary<uint, string>(mSoundBanks.Length);
				mIdToNameDups = new List<KeyValuePair<uint, string>>();
				for (int x = 0; x < mSoundBanksTable.Count; x++)
				{
					var entry = mSoundBanksTable[x];
					mSoundBanks[x] = new SoundBank.AkSoundBank((long)entry.FileSize64, entry.FileOffset, this);
				}
			}

			for (int x = 0; x < mSoundBanks.Length; x++)
				s.Stream(mSoundBanks[x]);

			mSoundBanks.ToString();
		}
		#endregion
	};
}