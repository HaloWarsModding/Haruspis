using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Caliburn.Micro;
using KSoft;

namespace PhxStudio.Modules.TraceList
{
	public sealed class TraceListTraceListener
		: TraceListener
	{
		const string kTraceAsTraceSource = "Trace";

		ITraceList mTraceList;

		public TraceListTraceListener()
		{
			mTraceList = IoC.Get<ITraceList>();
		}

		public override void Write(string message) => this.WriteLine(message);

		public override void WriteLine(string message) => this.TraceEvent(null, kTraceAsTraceSource, TraceEventType.Information, 0, message);

		public override void Fail(string message, string detailMessage)
		{
			var failMessage = new StringBuilder(message);
			if (detailMessage != null)
			{
				failMessage.Append(" ");
				failMessage.Append(detailMessage);
			}

			this.TraceEvent(null, kTraceAsTraceSource, TraceEventType.Error, TypeExtensions.kNone, failMessage.ToString());
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, data1: null, data: null))
				return;

			string message;
			if (!args.IsNullOrEmpty())
				message = string.Format(CultureInfo.InvariantCulture, format, args);
			else
				message = format;

			mTraceList.AddItem(eventType.ToTraceListItemType(),
				eventCache.Timestamp, source, message);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, args: null, data1: null, data: null))
				return;

			mTraceList.AddItem(eventType.ToTraceListItemType(),
				eventCache.Timestamp, source, message);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, args: null, data1: data, data: null))
				return;

			var item_data = GetDataEntryForTraceListItem(data);
			var message = item_data.ToString();

			mTraceList.AddItem(eventType.ToTraceListItemType(),
				eventCache.Timestamp, source, message, new object[] { item_data });
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, args: null, data1: null, data: data))
				return;

			var item_data = GetDataForTraceListItem(data);
			var message = item_data.Length > 0
				? item_data[0].ToString()
				: "NO MESSAGE";

			mTraceList.AddItem(eventType.ToTraceListItemType(),
				eventCache.Timestamp, source, message, item_data);
		}

		public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
		{
			var eventType = TraceEventType.Transfer;

			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, args: null, data1: null, data: null))
				return;

			mTraceList.AddItem(eventType.ToTraceListItemType(),
				eventCache.Timestamp, source, relatedActivityId.ToString());
		}

		private object[] GetDataForTraceListItem(params object[] args)
		{
			if (args.IsNullOrEmpty())
				return args;

			var result = new object[args.Length];

			for (int x = 0; x < result.Length; x++)
			{
				var arg = args[x];
				result[x] = GetDataEntryForTraceListItem(arg);
			}

			return result;
		}

		private object GetDataEntryForTraceListItem(object data)
		{
			if (data == null)
				return data;

			var result = data;
			if (result is Exception)
			{
			}
			else
			{
				result = result.ToString();
			}
			return result;
		}
	};
}
