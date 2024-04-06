using System;
using Caliburn.Micro;

namespace PhxStudio.Modules.Project
{
	interface IProjectService
		: INotifyPropertyChangedEx
	{
		PhxStudioProjectViewModel CurrentProject { get; }
		string CurrentProjectFilePath { get; }
		KSoft.Phoenix.Engine.PhxEngine Engine { get; }

		Exception CreateNew();
		Exception Open(string path);
		Exception Save(string path = null);
		Exception PreloadEngine();
		Exception LoadEngine();
	};
}
