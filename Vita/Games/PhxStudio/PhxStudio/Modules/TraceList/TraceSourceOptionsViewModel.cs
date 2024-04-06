using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Modules.Settings;

namespace PhxStudio.Modules.TraceList
{
	[Export(typeof(ISettingsEditor))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class TraceSourceOptionsViewModel
		: PropertyChangedBase
		, ISettingsEditor
	{
		public TraceSourceSettings Model { get; private set; }

		bool mIsDirty;

		public TraceSourceOptionsViewModel()
		{
			var settings = PhxStudio.Properties.Settings.Default;
			Model = settings.TraceSourceOptions.Clone();

			Model.PropertyChanged += ModelSourceSettingPropertyChanged;

			foreach (var source_setting in Model.SourceSettings)
				source_setting.PropertyChanged += ModelSourceSettingPropertyChanged;
		}

		private void ModelSourceSettingPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			mIsDirty = true;
		}

		#region ISettingsEditor
		public string SettingsPagePath { get {
			return Constants.SettingsPaths.Environment;
		} }
		public string SettingsPageName { get {
			return Constants.SettingsPages.Environment_TraceSources;
		} }

		public void ApplyChanges()
		{
			if (!mIsDirty)
				return;

			var settings = PhxStudio.Properties.Settings.Default;
			settings.TraceSourceOptions.Sync(Model);
		}
		#endregion
	};
}
