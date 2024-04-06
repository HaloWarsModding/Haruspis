using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Xml.Serialization;
using Caliburn.Micro;

namespace PhxStudio.Modules.TraceList
{
	[SettingsSerializeAs(SettingsSerializeAs.Xml)]
	[XmlRoot("TraceSourceSettings")]
	public sealed class TraceSourceSettings
		: PropertyChangedBase
	{
		int mMaxTraceListItems = KSoft.TypeExtensions.kNone;
		[DefaultSettingValue("-1")]
		public int MaxTraceListItems
		{
			get { return mMaxTraceListItems; }
			set { this.SetFieldVal(ref mMaxTraceListItems, value); }
		}

		[XmlArray(ElementName="SourceSettings")]
		[XmlArrayItem(ElementName="Source", Type=typeof(TraceSourceSetting))]
		public ObservableCollection<TraceSourceSetting> SourceSettings { get; private set; }
			= new ObservableCollection<TraceSourceSetting>();

		public TraceSourceSettings Clone()
		{
			var copy = new TraceSourceSettings();

			copy.MaxTraceListItems = this.MaxTraceListItems;

			foreach (var src_setting in this.SourceSettings)
			{
				var dst_setting = src_setting.Clone();
				copy.SourceSettings.Add(dst_setting);
			}

			return copy;
		}

		public void Sync(TraceSourceSettings src)
		{
			if (this.SourceSettings.Count != src.SourceSettings.Count)
				throw new ArgumentException("Counts are not equal");

			this.MaxTraceListItems = src.MaxTraceListItems;

			for (int x = 0; x < SourceSettings.Count; x++)
			{
				var src_setting = src.SourceSettings[x];
				var dst_setting = this.SourceSettings[x];

				if (src_setting.Name != dst_setting.Name)
					throw new ArgumentException("Setting names are not equal");

				dst_setting.Sync(src_setting);
			}
		}

		public void UpdateAfterSettingsLoaded()
		{
			RemoveAnySourcesNotInAllTraceSources();
			AddAllTraceSourcesNotPresent();
		}

		private void RemoveAnySourcesNotInAllTraceSources()
		{
			if (SourceSettings == null || SourceSettings.Count == 0)
				return;

			var source_names = (from source in App.AllTraceSources
								select source.Name).ToList();

			for (int x = SourceSettings.Count-1; x >= 0; x--)
			{
				var source_setting_name = SourceSettings[x].Name;
				if (source_names.BinarySearch(source_setting_name) >= 0)
					continue;

				SourceSettings.RemoveAt(x);
			}
		}

		private void AddAllTraceSourcesNotPresent()
		{
			if (SourceSettings == null)
				SourceSettings = new ObservableCollection<TraceSourceSetting>();

			var source_setting_names = (from source in SourceSettings
										select source.Name).ToList();
			var source_names = (from source in App.AllTraceSources
								select source.Name).ToList();

			foreach (var name in source_setting_names)
			{
				int existing_index = source_names.BinarySearch(name);
				if (existing_index < 0)
					continue;

				source_names.RemoveAt(existing_index);
			}

			foreach (var name in source_names)
			{
				var setting = new TraceSourceSetting();
				setting.Name = name;

				SourceSettings.Add(setting);
			}
		}
	};
}
