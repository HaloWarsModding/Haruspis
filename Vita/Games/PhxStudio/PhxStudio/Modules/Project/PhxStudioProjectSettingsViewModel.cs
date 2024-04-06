using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Modules.Settings;
using KSoft;
using GameVersionType = KSoft.Phoenix.HaloWars.GameVersionType;

namespace PhxStudio.Modules.Project
{
	[Export(typeof(ISettingsEditor))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class PhxStudioProjectSettingsViewModel
		: PropertyChangedBase
		, ISettingsEditor
	{
		private IEventAggregator mEventAggregator;

		private PhxStudioProjectViewModel mProjectViewModel;

		#region ProjectName
		string mProjectName;
		public string ProjectName
		{
			get { return mProjectName; }
			set
			{
				if (this.SetFieldObj(ref mProjectName, value))
				{
					this.NotifyOfPropertyChange(nameof(ProjectNameIsValid));
				}
			}
		}

		public bool ProjectNameIsValid => ProjectName.IsNotNullOrEmpty();
		#endregion

		#region GameVersion
		GameVersionType mGameVersion = GameVersionType.DefinitiveEdition;
		public GameVersionType GameVersion
		{
			get { return mGameVersion; }
			set { this.SetFieldEnum(ref mGameVersion, value); }
		}
		#endregion

		#region WorkDirectory
		string mWorkDirectory;
		public string WorkDirectory
		{
			get { return mWorkDirectory; }
			set { this.SetFieldObj(ref mWorkDirectory, value); }
		}
		#endregion

		#region FinalDirectory
		string mFinalDirectory;
		public string FinalDirectory
		{
			get { return mFinalDirectory; }
			set { this.SetFieldObj(ref mFinalDirectory, value); }
		}
		#endregion

		#region RevertSettingsCommand
		ICommand mRevertSettingsCommand;
		public ICommand RevertSettingsCommand { get {
			if (mRevertSettingsCommand == null)
				mRevertSettingsCommand = new RelayCommand(_ => RevertSettings());
			return mRevertSettingsCommand;
		} }
		#endregion

		#region SaveSettingsCommand
		ICommand mSaveSettingsCommand;
		public ICommand SaveSettingsCommand { get {
			if (mSaveSettingsCommand == null)
				mSaveSettingsCommand = new RelayCommand(_ => SaveSettings());
			return mSaveSettingsCommand;
		} }
		#endregion

		[ImportingConstructor]
		public PhxStudioProjectSettingsViewModel(IEventAggregator events)
		{
			mEventAggregator = events;
			mProjectViewModel = App.CurrentProjectViewModel;

			RevertSettings();
		}

		private void RevertSettings()
		{
			ProjectName = mProjectViewModel.Model.ProjectName;
			GameVersion = mProjectViewModel.Model.GameVersion;
			WorkDirectory = mProjectViewModel.Model.WorkDirectory;
			FinalDirectory = mProjectViewModel.Model.FinalDirectory;
		}

		private void SaveSettings()
		{
			bool workDirChanged =
				!string.Equals(mProjectViewModel.Model.WorkDirectory, WorkDirectory, StringComparison.OrdinalIgnoreCase);
			bool finalDirChanged =
				!string.Equals(mProjectViewModel.Model.FinalDirectory, FinalDirectory, StringComparison.OrdinalIgnoreCase);

			mProjectViewModel.Model.ProjectName = ProjectName;
			mProjectViewModel.Model.GameVersion = GameVersion;
			mProjectViewModel.Model.WorkDirectory = WorkDirectory;
			mProjectViewModel.Model.FinalDirectory = FinalDirectory;

			if (workDirChanged)
				mEventAggregator.PublishOnUIThread(new ProjectWorkDirectoryChangedEventArgs());
			if (finalDirChanged)
				mEventAggregator.PublishOnUIThread(new ProjectFinalDirectoryChangedEventArgs());
		}

		#region ISettingsEditor
		public string SettingsPagePath => Constants.SettingsPaths.Project;
		public string SettingsPageName => Constants.SettingsPages.Project_Info;

		public void ApplyChanges()
		{
			SaveSettings();
		}
		#endregion
	}
}
