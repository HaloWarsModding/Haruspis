
namespace KSoft.Wwise.FilePackage
{
	static class FNVHash64 // http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
	{
		public static ulong Compute(string text)
		{
			var encoding = new System.Text.ASCIIEncoding();
			return Compute(encoding.GetBytes(text));
		}

		public static ulong Compute(byte[] data)
		{
			ulong num = 0xCBF29CE484222325L;
			for (int i = 0; i < data.Length; i++)
			{
				num *= (ulong)0x100000001B3L;
				num ^= data[i];
			}
			return num;
		}
	};
}