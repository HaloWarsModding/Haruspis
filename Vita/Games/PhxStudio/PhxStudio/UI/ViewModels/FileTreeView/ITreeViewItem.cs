using System.ComponentModel;

namespace PhxStudio.UI.ViewModels.FileTreeView
{
	public interface ITreeViewItem
		: INotifyPropertyChanged
	{
		string FilePath { get; }
		string FileName { get; }
		string FileExtension { get; }
	};
}