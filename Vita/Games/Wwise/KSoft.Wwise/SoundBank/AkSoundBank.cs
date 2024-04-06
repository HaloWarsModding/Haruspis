using System;
using System.Collections.Generic;
using Contracts = System.Diagnostics.Contracts;
#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif

namespace KSoft.Wwise.SoundBank
{
	public sealed partial class AkSoundBank
		: IO.IEndianStreamSerializable
	{
		static readonly Values.GroupTagData32 kHeaderSignature =
					new Values.GroupTagData32("BKHD", "audiokinetic_sound_bank"); // BankHeaderChunkID
		#region wav chunks
		/*
		 * RIFX RIFXChunkId
		 * WAVE WAVEChunkId
		 * fmt  fmtChunkId
		 * XMA2 xma2ChunkId
		 * XMAc xmaCustomChunkId
		 * data dataChunkId
		 * fact factChunkId
		 * wavl wavlChunkId
		 * slnt slntChunkId
		 * cue  cueChunkId
		 * plst plstChunkId
		 * LIST LISTChunkId
		 * adtl adtlChunkId
		 * labl lablChunkId
		 * note noteChunkId
		 * ltxt ltxtChunkId
		 * smpl smplChunkId
		 * inst instChunkId
		 * rgn  rgnChunkId
		 * JUNK JunkChunkId
		 * WiiH WiiHChunkID
		*/
		#endregion

		long mStreamOffset, mEndOfStream;
		internal FilePackage.AkFilePackage mPackage;
		Dictionary<uint, string> mIdToName;

		#region Header
		AkSubchunkHeader mHeaderChunkHeader;
		AkBankHeader mHeader;

		public uint GeneratorVersion{ get { return mHeader.BankGeneratorVersion; } }
		public uint Id				{ get { return mHeader.SoundBankID; } }
		public uint LanguageId		{ get { return mHeader.LanguageID; } }
		public bool HasFeedback		{ get { return mHeader.FeedbackSupported > 0; } }
		#endregion
		Dictionary<AkSubchunkHeader, AkSoundBankObjectBase> mChunks;

		internal AkSoundBankData mData;
		internal AkSoundBankDataIndex mDataIndex;

		// TODO: mPackage can be null, need to get around this...
		public uint SdkVersion { get { return mPackage.Settings.SdkVersion; } }

		public AkSoundBank(long fileSize, long fileOffset = 0, FilePackage.AkFilePackage package = null)
		{
			mStreamOffset = fileOffset;
			mEndOfStream = fileOffset + fileSize;
			mPackage = package;

			if (package == null)
				mIdToName = new Dictionary<uint,string>();

			mChunks = new Dictionary<AkSubchunkHeader, AkSoundBankObjectBase>();
		}

		internal void MapIdToName(uint id, string name)
		{
			if (mPackage != null)
				mPackage.MapIdToName(id, name);
			else
				mIdToName.Add(id, name);
		}

		#region IEndianStreamSerializable Members
		[Contracts.Pure]
		bool EndOfStream(IO.EndianStream s)
		{
			if (s.IsReading)
			{
				Contract.Assert(s.BaseStream.Position <= mEndOfStream);
				return s.BaseStream.Position == mEndOfStream;
			}

			return false;
		}
		void ReadChunks(IO.EndianStream es, IO.EndianReader s)
		{
			while (!EndOfStream(es))
			{
				var hdr = new AkSubchunkHeader();
				hdr.Serialize(es);

				using (es.EnterVirtualBufferWithBookmark(hdr.ChunkSize))
				{
					var obj = AkSoundBankObjectBase.New(hdr.Tag, GeneratorVersion);
					if (obj != null)
					{
						obj.Serialize(es, hdr);

						mChunks.Add(hdr, obj);
					}
				}
			}
		}
		void WriteChunks(IO.EndianStream es, IO.EndianWriter s)
		{
			throw new NotSupportedException();
		}
		public void Serialize(IO.EndianStream s)
		{
			using (s.EnterOwnerBookmark(this))
			{
				s.Seek(mStreamOffset, System.IO.SeekOrigin.Begin);
				#region Header
				s.Stream(ref mHeaderChunkHeader);
				if (kHeaderSignature.ID!=mHeaderChunkHeader.Tag) throw new IO.SignatureMismatchException(s.BaseStream,
					kHeaderSignature.ID, mHeaderChunkHeader.Tag);

				s.Stream(ref mHeader);
				s.Pad((int)mHeaderChunkHeader.ChunkSize - AkBankHeader.kSizeOf);
				#endregion

				s.StreamMethods(s, ReadChunks, WriteChunks);
			}
		}
		#endregion

		internal void PrepareForExtraction()
		{
			foreach (var kv in mChunks)
			{
				var obj = kv.Value as AkSoundBankHierarchy;
				if (obj != null)
					obj.PrepareForExtraction(this);
			}
		}

		internal void CopyObjectsTo(FilePackage.AkFilePackageExtractor extractor)
		{
			foreach (var chunk in mChunks)
				if (chunk.Value is AkSoundBankHierarchy)
					((AkSoundBankHierarchy)chunk.Value).CopyObjectsTo(extractor);
				else if (chunk.Value is AkSoundBankData)
					mData = (AkSoundBankData)chunk.Value;
				else if (chunk.Value is AkSoundBankDataIndex)
				{
					mDataIndex = (AkSoundBankDataIndex)chunk.Value;
					foreach (var media in mDataIndex.LoadedMedia)
					{
						if (!extractor.mUntouched.ContainsKey(media.ID))
							extractor.mUntouched.Add(media.ID, new MediaReference { Media = media, BankId = this.Id });
					}
				}
		}
	};
}