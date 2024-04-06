using System;
using System.ComponentModel;
using Caliburn.Micro;
using KSoft;

namespace PhxStudio.Modules.TraceList
{
	public sealed class TraceListItem
		: PropertyChangedBase
	{
		TraceListItemType mItemType = TraceListItemType.Invalid;
		[ReadOnly(true)]
		public TraceListItemType ItemType
		{
			get { return mItemType; }
			set { this.SetFieldEnum(ref mItemType, value); }
		}

		int mNumber;
		[ReadOnly(true)]
		public int Number
		{
			get { return mNumber; }
			set { this.SetFieldVal(ref mNumber, value); }
		}

		long mTimeStamp;
		[ReadOnly(true)]
		public long TimeStamp
		{
			get { return mTimeStamp; }
			set { this.SetFieldVal(ref mTimeStamp, value); }
		}

		string mSourceName;
		[ReadOnly(true)]
		public string SourceName
		{
			get { return mSourceName; }
			set { this.SetFieldObj(ref mSourceName, value); }
		}

		string mMessage;
		[ReadOnly(true)]
		public string Message
		{
			get { return mMessage; }
			set { this.SetFieldObj(ref mMessage, value); }
		}

		// #HACK_PHXSTUDIO using EmptyArray here because Gemini's Inspector won't update
		// the TraceDataEditorView when using null and the previous item selected
		// actually had data.
		// Also not using ReadOnly because it disables the View's DataGrid completely.
		object[] mData = KSoft.Util.EmptyArray;
		[Browsable(false)]
		public object[] Data
		{
			get { return mData; }
			set
			{
				if (value.IsNullOrEmpty())
					value = KSoft.Util.EmptyArray;

				if (this.SetFieldRef(ref mData, value))
				{
					NotifyOfPropertyChange(nameof(HasData));
				}
			}
		}
		[Browsable(false)]
		public bool HasData => !mData.IsNullOrEmpty() && mData != KSoft.Util.EmptyArray;

		[Browsable(false)]
		public System.Action OnClick { get; set; }
	};
}
