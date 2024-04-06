using System;
using System.Collections.Generic;

namespace KSoft.Wwise.FilePackage
{
	sealed class AkFileLookupTable
		: IO.IEndianStreamSerializable
		, IEnumerable<AkFileLookupTableEntry>
	{
		AkFileLookupTableEntry[] mEntries;

		internal uint TotalSize;

		public int Count { get { return mEntries.Length; } }

		public AkFileLookupTableEntry this[int index]	{ get { return mEntries[index]; } }

		public AkFileLookupTableEntry Find(ulong id)
		{
			return Array.Find(mEntries, e => e.FileId64 == id);
		}

		public uint CalculateTotalSize()
		{
			return sizeof(uint) + (uint)(mEntries.Length * AkFileLookupTableEntry.kSizeOf);
		}

		#region IEndianStreamSerializable Members
		public void Serialize(IO.EndianStream s)
		{
			s.StreamArrayInt32(ref mEntries);
		}
		#endregion

		#region IEnumerable<FileEntry> Members
		public IEnumerator<AkFileLookupTableEntry> GetEnumerator()
		{
			return (mEntries as IEnumerable<AkFileLookupTableEntry>).GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return mEntries.GetEnumerator();
		}
		#endregion
	};
}