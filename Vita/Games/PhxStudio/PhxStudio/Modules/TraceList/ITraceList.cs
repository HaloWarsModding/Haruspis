using Caliburn.Micro;
using Gemini.Framework;

namespace PhxStudio.Modules.TraceList
{
	public interface ITraceList
		: ITool
	{
		int TotalNumberOfTraces { get; set; }
		bool PauseTracing { get; set; }

		bool ShowCritical { get; set; }
		bool ShowError { get; set; }
		bool ShowWarning { get; set; }
		bool ShowInformation { get; set; }
		bool ShowVerbose { get; set; }

		bool ShowStart { get; set; }
		bool ShowStop { get; set; }
		bool ShowSuspend { get; set; }
		bool ShowResume { get; set; }
		bool ShowTransfer { get; set; }

		bool TailTraces { get; set; }

		IObservableCollection<TraceListItem> Items { get; }

		void AddItem(TraceListItemType type, long timeStamp, string sourceName, string message
			, object[] data = null, System.Action onClick = null);

		void ClearAll();
	};
}
