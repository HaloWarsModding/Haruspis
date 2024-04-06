using System;
using KSoft.Phoenix.Engine;

namespace PhxStudio.Modules.Project
{
	/// <summary>Info relating to the project opening</summary>
	public class ProjectOpeningEventArgs
		: EventArgs
	{
	};

	/// <summary>Event fired when PhxStudioProject.Engine is constructed</summary>
	public class ProjectEngineCreatedEventArgs
		: EventArgs
	{
		public PhxEngine Engine { get; private set; }

		public ProjectEngineCreatedEventArgs(PhxEngine engine)
		{
			Engine = engine;
		}
	};

	/// <summary>Event fired when PhxStudioProject.Engine is unloaded (nulled)</summary>
	public class ProjectEngineUnloadedEventArgs
		: EventArgs
	{
	};

	public class ProjectEnginePreloadedEventArgs
		: EventArgs
	{
		public PhxEngine Engine { get; private set; }

		public ProjectEnginePreloadedEventArgs(PhxEngine engine)
		{
			Engine = engine;
		}
	};

	public class ProjectEngineLoadedEventArgs
		: EventArgs
	{
		public PhxEngine Engine { get; private set; }

		public ProjectEngineLoadedEventArgs(PhxEngine engine)
		{
			Engine = engine;
		}
	};
}
