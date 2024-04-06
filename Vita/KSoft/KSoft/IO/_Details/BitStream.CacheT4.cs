﻿using Contracts = System.Diagnostics.Contracts;
#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif

using TWord = System.UInt32;

namespace KSoft.IO
{
	partial class BitStream
	{
		/// <summary>Number of bytes in <see cref="mCache"/></summary>
		const int kWordByteCount = sizeof(TWord);
		/// <summary>Number of bits in <see cref="mCache"/></summary>
		const int kWordBitCount = sizeof(TWord) * Bits.kByteBitCount;
		/// <summary>Bit count to bit-mask look up table</summary>
		static readonly TWord[] kBitmaskLUT;

		static void InitializeBitmaskLookUpTable(out TWord[] table)
		{
			bool success = Bits.GetBitConstants(typeof(TWord),
				out int word_byte_count, out int word_bit_count, out int word_bit_shift, out int word_bit_mod);
			Contract.Assert(success, "TWord is an invalid type for BitStream");

			Bits.BitmaskLookUpTableGenerate(kWordBitCount, out table);
		}


		/// <summary>The bit cache we use for streaming to/from <see cref="BaseStream"/></summary>
		TWord mCache;

		// #REVIEW: change mIoBuffer to be kWordByteCount and do an entire Read/Write() instead of looping?
		// #REVIEW: maybe change the ReadWord implementation to not automatically populate the next word...
		#region Cache operations
		/// <summary>Fill the cache with <see cref="kWordByteCount"/> or fewer bytes bytes</summary>
		void FillCache()
		{
			mCache = 0;
			mCacheBitIndex = 0;
			mCacheBitsStreamedCount = 0;

			int byte_count = kWordByteCount-1; // number of bytes to try and read
			int shift = kWordBitCount-Bits.kByteBitCount; // start shifting to the MSB
			while (	!IsEndOfStream &&
					byte_count >= 0 &&
					BaseStream.Read(mIoBuffer, 0, sizeof(byte)) != 0 )
			{
				mCache |= ((TWord)mIoBuffer[0]) << shift;
				--byte_count;
				shift -= Bits.kByteBitCount;
				mCacheBitsStreamedCount += Bits.kByteBitCount;
			}

			if (byte_count != -1 && ThrowOnOverflow.CanRead())
				throw new System.IO.EndOfStreamException("Tried to read more bits than the stream has/can see");
		}
		/// <summary>Flush the cache to <see cref="BaseStream"/> with <see cref="kWordByteCount"/> or fewer bytes bytes</summary>
		void FlushCache()
		{
			#if !CONTRACTS_FULL_SHIM // can't do this with our shim! ValueAtReturn sets out param to default ON ENTRY
			Contract.Ensures(Contract.ValueAtReturn(out mCache) == 0);
			Contract.Ensures(Contract.ValueAtReturn(out mCacheBitIndex) == 0);
			#endif

			if (mCacheBitIndex == 0) // no bits to flush!
			{
				Contract.Assert(mCache == 0, "Why is there data in the cache?");
				return;
			}

			mCacheBitsStreamedCount = 0;

			int byte_count = (mCacheBitIndex-1) >> Bits.kByteBitShift; // number of bytes to try and write
			int shift = kWordBitCount-Bits.kByteBitCount; // start shifting from the MSB
			while (	/*!IsEndOfStream &&*/
					byte_count >= 0)
			{
				mIoBuffer[0] = (byte)(mCache >> shift);
				BaseStream.Write(mIoBuffer, 0, sizeof(byte));
				--byte_count;
				shift -= Bits.kByteBitCount;
				mCacheBitsStreamedCount += Bits.kByteBitCount;
			}

			if (byte_count != -1 && ThrowOnOverflow.CanWrite())
				throw new System.IO.EndOfStreamException("Tried to write more bits than the stream has/can see");

			mCache = 0;
			mCacheBitIndex = 0;
		}

		/// <remarks>Don't call me unless you are ReadWord</remarks>
		[Contracts.Pure]
		TWord ExtractWordFromCache(int bitCount)
		{
			// amount to shift the bits extracted from mCache
			int shift = kWordBitCount - (mCacheBitIndex + bitCount);
			TWord word_mask = kBitmaskLUT[bitCount];

			TWord word = mCache;
			word >>= shift;
			word &= word_mask;

			return word;
		}
		/// <remarks>Don't call me unless you are WriteWord</remarks>
		void PutWordInCache(TWord word, int bitCount)
		{
			Contract.Ensures(Contract.OldValue(mCacheBitIndex) == mCacheBitIndex);

			// amount to shift word before appending it to mCache bits
			int shift = (kWordBitCount - mCacheBitIndex) - bitCount;
			TWord word_mask = kBitmaskLUT[bitCount];

			word &= word_mask;
			word <<= shift;
			mCache |= word;
		}
		#endregion

		public bool ReadBoolean()
		{
			ReadWord(out TWord word, Bits.kBooleanBitCount);

			return 1 == word;
		}
	};
}
