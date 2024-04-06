
namespace KSoft.Wwise.FilePackage
{
	struct AkFilePackageHeader
		: IO.IEndianStreamSerializable
	{
		const uint kSizeOfHeader = AkSubchunkHeader.kSizeOf + sizeof(int);

		static readonly Values.GroupTagData32 kSignature = new Values.GroupTagData32("AKPK", "audiokinetic_package");
		const uint kVersion = 1;

		public uint HeaderSize;

		public void InitializeSize(uint sdkVersion, uint langMapTotalSize)
		{
			HeaderSize = 0;
			HeaderSize += kSizeOfHeader;
			HeaderSize += sizeof(uint); // field for lang map size
			HeaderSize += sizeof(uint); // field for LUT size (sound banks)
			HeaderSize += sizeof(uint); // field for LUT size (streamed files)
			if (AkVersion.HasExternalFiles(sdkVersion))
				HeaderSize += sizeof(uint); // field for LUT size (external files)
			HeaderSize += langMapTotalSize;
		}

		#region IEndianStreamSerializable Members
		public void Serialize(IO.EndianStream s)
		{
			s.StreamSignature(kSignature.TagString, Memory.Strings.StringStorage.AsciiString);
			s.Stream(ref HeaderSize);
			s.StreamVersion(kVersion);
		}
		#endregion
	};
}