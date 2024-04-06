using System.Diagnostics;
using PhxStudio.Modules.TraceList;

namespace PhxStudio.Modules.TraceList
{
	public enum TraceListItemType
	{
		Invalid = KSoft.TypeExtensions.kNone,

		Critical = 0,
		Error,
		Warning,
		Information,
		Verbose,

		Start,
		Stop,
		Suspend,
		Resume,
		Transfer,
	};
}

namespace PhxStudio
{
	public static partial class TypeExtensionsPhxStudio
	{
		public static TraceListItemType ToTraceListItemType(this TraceEventType type)
		{
			switch (type)
			{
				case TraceEventType.Critical:
					return TraceListItemType.Critical;

				case TraceEventType.Error:
					return TraceListItemType.Error;

				case TraceEventType.Warning:
					return TraceListItemType.Warning;

				case TraceEventType.Information:
					return TraceListItemType.Information;

				case TraceEventType.Verbose:
					return TraceListItemType.Verbose;

				case TraceEventType.Start:
					return TraceListItemType.Start;

				case TraceEventType.Stop:
					return TraceListItemType.Stop;

				case TraceEventType.Suspend:
					return TraceListItemType.Suspend;

				case TraceEventType.Resume:
					return TraceListItemType.Resume;

				case TraceEventType.Transfer:
					return TraceListItemType.Transfer;

				default:
					return TraceListItemType.Invalid;
			}
		}

		public static string ToDisplayString(this TraceListItemType type, bool plural)
		{
			switch (type)
			{
				case TraceListItemType.Error:
					if (plural)
						return nameof(TraceListItemType.Error) + "s";
					else
						return nameof(TraceListItemType.Error);

				case TraceListItemType.Warning:
					if (plural)
						return nameof(TraceListItemType.Warning) + "s";
					else
						return nameof(TraceListItemType.Warning);

				case TraceListItemType.Start:
					if (plural)
						return nameof(TraceListItemType.Start) + "s";
					else
						return nameof(TraceListItemType.Start);

				case TraceListItemType.Stop:
					if (plural)
						return nameof(TraceListItemType.Stop) + "s";
					else
						return nameof(TraceListItemType.Stop);

				case TraceListItemType.Suspend:
					return nameof(TraceListItemType.Suspend) + "ed";

				case TraceListItemType.Resume:
					if (plural)
						return nameof(TraceListItemType.Resume) + "s";
					else
						return nameof(TraceListItemType.Resume);

				case TraceListItemType.Transfer:
					if (plural)
						return nameof(TraceListItemType.Transfer) + "s";
					else
						return nameof(TraceListItemType.Transfer);

				default:
					return type.ToString();
			}
		}
	};
}
