using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhxStudio.Utils
{
	static class FileIconsLoader
	{
		const uint SHGFI_ICON = 0x100;
		const uint SHGFI_SMALLICON = 0x1;
		const uint SHGFI_LARGEICON = 0x0;

		struct ShFileInfo
		{
			// Handle to the icon representing the file
			public IntPtr hIcon;
			// Index of the icon within the image list
			public int iIcon;
			// Various attributes of the file
			public uint dwAttributes;
			// Path to the file
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szDisplayName;
			// File type
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		[DllImport("Shell32.dll")]
		static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref ShFileInfo psfi,
			int cbFileInfo, uint uFlags);

		static ImageSource GetIcon(string fileName, uint flags)
		{
			var shinfo = new ShFileInfo();
			SHGetFileInfo(fileName, 0, ref shinfo, Marshal.SizeOf(shinfo), SHGFI_ICON | flags);

			using (var icon = System.Drawing.Icon.FromHandle(shinfo.hIcon))
			{
				var img = Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
					new Int32Rect(0, 0, icon.Width, icon.Height),
					BitmapSizeOptions.FromEmptyOptions());
				return img;
			}
		}

		public static ImageSource GetSmallIcon(string fileName)
		{
			return GetIcon(fileName, SHGFI_SMALLICON);
		}

		public static ImageSource GetLargeIcon(string fileName)
		{
			return GetIcon(fileName, SHGFI_LARGEICON);
		}
	};
}
