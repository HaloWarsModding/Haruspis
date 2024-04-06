using System;
using System.IO;
using Caliburn.Micro;

namespace PhxStudio.UI.ViewModels.FileTreeView
{
	public abstract class TreeViewItemBase
		: PropertyChangedBase
		, ITreeViewItem
	{
		string mFilePath;
		public string FilePath
		{
			get { return mFilePath; }
			private set { this.SetFieldObj(ref mFilePath, value, overrideChecks: true); }
		}

		string mFileName;
		public string FileName
		{
			get { return mFileName; }
			private set { this.SetFieldObj(ref mFileName, value, overrideChecks: true); }
		}

		string mFileExtension;
		public string FileExtension
		{
			get { return mFileExtension; }
			private set { this.SetFieldObj(ref mFileExtension, value, overrideChecks: true); }
		}

		public string FileNameAndExtension { get; private set; }

		object mUserData;
		public object UserData
		{
			get { return mUserData; }
			set { this.SetField(ref mUserData, value); }
		}

		protected void SetPathToFile(string filePath)
		{
			if (string.Equals(mFilePath, filePath, StringComparison.OrdinalIgnoreCase))
				return;

			FilePath = filePath;
			FileName = Path.GetFileNameWithoutExtension(filePath);
			FileExtension = Path.GetExtension(filePath) ?? "";
			FileNameAndExtension = FileName + FileExtension;
		}

		protected void SetPathToDirectory(string directoryPath)
		{
			if (string.Equals(mFilePath, directoryPath, StringComparison.OrdinalIgnoreCase))
				return;

			FilePath = directoryPath;
			FileName = Path.GetFileNameWithoutExtension(directoryPath);
			FileExtension = null;
			FileNameAndExtension = FileName;
		}
	};
}