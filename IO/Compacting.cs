using System.Runtime.InteropServices;

namespace IO
{
    internal partial class Compacting
    {
        [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool Compact(
            string lpFileName,
            ushort CompressionType,
            IntPtr lpReserved,
            uint dwReserved
        );

        private const ushort COMPRESSION_FORMAT_XPRESS16K = 5;

        public static (bool, string) CompressDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return (false, "The specified directory does not exist.");
            }

            string[] files = Directory.GetFiles(directoryPath);
            List<string> failedFiles = [];

            foreach (string file in files)
            {
                if (!CompressFile(file))
                {
                    failedFiles.Add(file);
                }
            }

            return failedFiles.Count == 0
                ? (true, "All files were successfully compressed.")
                : (false, "Failed to compress the following files: " + string.Join(", ", failedFiles));
        }

        private static bool CompressFile(string filePath)
        {
            IntPtr lpReserved = IntPtr.Zero;
            uint dwReserved = 0;

            return Compact(filePath, COMPRESSION_FORMAT_XPRESS16K, lpReserved, dwReserved);
        }
    }
}
