
namespace KSoft.Wwise
{
	struct AkSubchunkHeader
		: IO.IEndianStreamSerializable
	{
		/// <summary>Size of this struct on disk</summary>
		public const int kSizeOf = sizeof(uint) * 2;

		public uint Tag, ChunkSize;

		#region IEndianStreamSerializable Members
		public void Serialize(IO.EndianStream s)
		{
			s.StreamTagBigEndian(ref Tag);
			s.Stream(ref ChunkSize);
		}
		#endregion

		public override int GetHashCode() { return (int)Tag; }
	};
}