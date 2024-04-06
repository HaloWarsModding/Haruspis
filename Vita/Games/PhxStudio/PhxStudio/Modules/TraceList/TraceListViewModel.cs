using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Inspector;
using KSoft;

namespace PhxStudio.Modules.TraceList
{
	// #TODO_PHXSTUDIO Filtering should probably be done with a CollectionViewSoruce instead

	[Export(typeof(ITraceList))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public sealed class TraceListViewModel
		: Tool
		, ITraceList
	{
		public override PaneLocation PreferredLocation => PaneLocation.Bottom;

		#region Imports
#pragma warning disable 649

		[Import] IInspectorTool mInspectorTool;

#pragma warning restore 649
		#endregion

		private int mItemNumber;

		BindableCollection<TraceListItem> mItems;
		public IObservableCollection<TraceListItem> Items => mItems;

		public IEnumerable<TraceListItem> FilteredItems { get {
			if (ShowEverything)
				return mItems;

			var items =
				from item in mItems
				where (ShowCritical		&& item.ItemType == TraceListItemType.Critical)
					||(ShowError		&& item.ItemType == TraceListItemType.Error)
					||(ShowWarning		&& item.ItemType == TraceListItemType.Warning)
					||(ShowInformation	&& item.ItemType == TraceListItemType.Information)
					||(ShowVerbose		&& item.ItemType == TraceListItemType.Verbose)
					||(ShowStart		&& item.ItemType == TraceListItemType.Start)
					||(ShowStop			&& item.ItemType == TraceListItemType.Stop)
					||(ShowSuspend		&& item.ItemType == TraceListItemType.Suspend)
					||(ShowResume		&& item.ItemType == TraceListItemType.Resume)
					||(ShowTransfer		&& item.ItemType == TraceListItemType.Transfer)
				select item;
			return items;
		} }

		#region PauseTracing
		bool mPauseTracing;
		public bool PauseTracing
		{
			get { return mPauseTracing; }
			set { this.SetFieldVal(ref mPauseTracing, value); }
		}
		#endregion

		#region TotalNumberOfTraces
		int mTotalNumberOfTraces;
		public int TotalNumberOfTraces
		{
			get { return mTotalNumberOfTraces; }
			set { this.SetFieldVal(ref mTotalNumberOfTraces, value); }
		}
		#endregion

		private bool ShowEverything
			=> ShowCritical
			&& ShowError
			&& ShowWarning
			&& ShowInformation
			&& ShowVerbose
			&& ShowStart
			&& ShowStop
			&& ShowSuspend
			&& ShowResume
			&& ShowTransfer;

		#region ShowCritical
		bool mShowCritical = true;
		public bool ShowCritical
		{
			get { return mShowCritical; }
			set
			{
				if (this.SetFieldVal(ref mShowCritical, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region ShowError
		bool mShowError = true;
		public bool ShowError
		{
			get { return mShowError; }
			set
			{
				if (this.SetFieldVal(ref mShowError, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region ShowWarning
		bool mShowWarning = true;
		public bool ShowWarning
		{
			get { return mShowWarning; }
			set
			{
				if (this.SetFieldVal(ref mShowWarning, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region ShowInformation
		bool mShowInformation = true;
		public bool ShowInformation
		{
			get { return mShowInformation; }
			set
			{
				if (this.SetFieldVal(ref mShowInformation, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region ShowVerbose
		bool mShowVerbose = true;
		public bool ShowVerbose
		{
			get { return mShowVerbose; }
			set
			{
				if (this.SetFieldVal(ref mShowVerbose, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region ShowStart
		bool mShowStart = true;
		public bool ShowStart
		{
			get { return mShowStart; }
			set
			{
				if (this.SetFieldVal(ref mShowStart, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region ShowStop
		bool mShowStop = true;
		public bool ShowStop
		{
			get { return mShowStop; }
			set
			{
				if (this.SetFieldVal(ref mShowStop, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region ShowSuspend
		bool mShowSuspend = true;
		public bool ShowSuspend
		{
			get { return mShowSuspend; }
			set
			{
				if (this.SetFieldVal(ref mShowSuspend, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region ShowResume
		bool mShowResume = true;
		public bool ShowResume
		{
			get { return mShowResume; }
			set
			{
				if (this.SetFieldVal(ref mShowResume, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region ShowTransfer
		bool mShowTransfer = true;
		public bool ShowTransfer
		{
			get { return mShowTransfer; }
			set
			{
				if (this.SetFieldVal(ref mShowTransfer, value))
				{
					NotifyOfPropertyChange(nameof(FilteredItems));
				}
			}
		}
		#endregion

		#region TailTraces
		bool mTailTraces = true;
		[Description("When enabled, UI will snap to new traces as they come in")]
		public bool TailTraces
		{
			get { return mTailTraces; }
			set { this.SetFieldVal(ref mTailTraces, value); }
		}
		#endregion

		public TraceListViewModel()
		{
			DisplayName = "Trace List";

			ToolBarDefinition = ToolBarDefinitions.TraceListToolBar;

			mItems = new BindableCollection<TraceListItem>();
			mItems.CollectionChanged += OnItemsCollectionChanged;
		}

		private void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			NotifyOfPropertyChange(nameof(FilteredItems));
		}

		public void AddItem(TraceListItemType type, long timeStamp, string sourceName, string message
			, object[] data = null, System.Action onClick = null)
		{
			if (PauseTracing)
			{
				// #NOTE_PHXSTUDIO I think I still want to track the total number of would-be traces
				++mItemNumber;
				TotalNumberOfTraces = mItemNumber;
				return;
			}

			var settings = PhxStudio.Properties.Settings.Default;
			var traceSettings = settings.TraceSourceOptions;
			if (traceSettings != null)
			{
				if (traceSettings.MaxTraceListItems.IsNotNone())
				{
					int surplus_count = Items.Count;
					surplus_count += 1; // we're adding one
					surplus_count -= traceSettings.MaxTraceListItems;
					while (surplus_count-- > 0)
					{
						Items.RemoveAt(0);
					}
				}
			}

			var item = new TraceListItem
			{
				ItemType = type,
				Number = ++mItemNumber,
				TimeStamp = timeStamp,
				SourceName = sourceName,
				Message = message,
				Data = data,
				OnClick = onClick,
			};

			Items.Add(item);
			TotalNumberOfTraces = mItemNumber;
		}

		public void ClearAll() => Items.Clear();

		public void OnSelectedItemChanged(TraceListItem selectedItem)
		{
			if (mInspectorTool == null)
				return;

			if (selectedItem == null)
			{
				mInspectorTool.SelectedObject = null;
				return;
			}

			var item_inspector = new InspectableObjectBuilder()
				.WithObjectProperties(selectedItem, TraceListItemPropertyFilter);
			if (selectedItem.HasData)
			{
				item_inspector
					.WithEditor(selectedItem, x => x.Data, new Inspectors.TraceDataEditorViewModel());
			}

			mInspectorTool.SelectedObject = item_inspector.ToInspectableObject();
		}

		private static bool TraceListItemPropertyFilter(PropertyDescriptor pd)
		{
			switch (pd.Name)
			{
				case nameof(TraceListItem.SourceName):
				case nameof(TraceListItem.Message):
					return true;

				default:
					return false;
			}
		}
	};
}
