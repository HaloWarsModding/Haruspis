using System;
using System.Configuration;
using System.Diagnostics;
using Caliburn.Micro;

namespace PhxStudio.Modules.TraceList
{
	[SettingsSerializeAs(SettingsSerializeAs.Xml)]
	public sealed class TraceSourceSetting
		: PropertyChangedBase
	{
		string mName;
		public string Name
		{
			get { return mName; }
			set { this.SetFieldObj(ref mName, value); }
		}

		TraceLevel mLevel = TraceLevel.Verbose;
		[DefaultSettingValue(nameof(TraceLevel.Verbose))]
		public TraceLevel Level
		{
			get { return mLevel; }
			set { this.SetFieldEnum(ref mLevel, value); }
		}

		bool mDisabled;
		[DefaultSettingValue("false")]
		public bool Disabled
		{
			get { return mDisabled; }
			set { this.SetFieldVal(ref mDisabled, value); }
		}

		public TraceSourceSetting Clone()
		{
			var copy = new TraceSourceSetting();
			copy.mName = this.Name;
			copy.mLevel = this.Level;
			copy.mDisabled = this.Disabled;
			return copy;
		}

		public void Sync(TraceSourceSetting src)
		{
			this.Name = src.Name;
			this.Level = src.Level;
			this.Disabled = src.Disabled;
		}
	};
}
