using KSoft;
using Caliburn.Micro;
using EditorFileType = Gemini.Framework.Services.EditorFileType;
using GameVersionType = KSoft.Phoenix.HaloWars.GameVersionType;

namespace PhxStudio.Modules.Project
{
	class PhxStudioProject
		: KSoft.ObjectModel.BasicViewModel
		, KSoft.IO.ITagElementStringNameStreamable
	{
		public const string XmlRootName = "Project";

		#region FileType
		public static string FileExtension => ".phxproj";
		private static EditorFileType gFileType;
		public static EditorFileType FileType { get {
			if (gFileType == null)
				gFileType = new EditorFileType("PhxStudio Project", FileExtension);
			return gFileType;
		} }
		#endregion

		#region ProjectFilePath
		string mProjectFilePath;
		/// <summary>Not serialized, just for remembering where a project was loaded and should be saved to</summary>
		public string ProjectFilePath
		{
			get { return mProjectFilePath; }
			set
			{
				if (!SetFieldObj(ref mProjectFilePath, value))
					return;

				IsOnDisk = IsOnDisk;
			}
		}

		public bool IsOnDisk
		{
			get { return ProjectFilePath.IsNotNullOrEmpty(); }
			set
			{
				bool dummy = false;
				SetFieldVal(ref dummy, value, overrideChecks: true);
			}
		}
		#endregion

		#region ProjectName
		const string kDefaultProjectName = "HaloWars Mod";

		string mProjectName = kDefaultProjectName;
		public string ProjectName
		{
			get { return mProjectName; }
			set
			{
				if (this.SetFieldObj(ref mProjectName, value))
				{
					this.OnPropertyChanged(nameof(ProjectNameIsValid));
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
			set
			{
				if (this.SetFieldObj(ref mWorkDirectory, value))
				{
					CreateOrUnloadEngine();
				}
			}
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

		#region Engine
		KSoft.Phoenix.Engine.PhxEngine mEngine;
		public KSoft.Phoenix.Engine.PhxEngine Engine
		{
			get { return mEngine; }
			private set { this.SetField(ref mEngine, value); }
		}
		#endregion

		bool mEngineCreationDisabled;
		private void CreateOrUnloadEngine()
		{
			if (mEngineCreationDisabled)
				return;

			// don't do anything when the engine was never loaded to begin with
			if (WorkDirectory.IsNullOrEmpty() && Engine == null)
				return;

			var eventAggregator = IoC.Get<IEventAggregator>();

			bool unload = Engine != null;
			bool reload = unload && WorkDirectory.IsNotNullOrEmpty();
			bool load = !unload && WorkDirectory.IsNotNullOrEmpty();

			if (unload)
			{
				Engine = null;
				eventAggregator.PublishOnUIThread(new ProjectEngineUnloadedEventArgs());
			}

			if (reload || load)
			{
				var engine = KSoft.Phoenix.Engine.PhxEngine.CreateForHaloWars(
					WorkDirectory, WorkDirectory,
					GameVersion == GameVersionType.Xbox360);

				Engine = engine;
				eventAggregator.PublishOnUIThread(new ProjectEngineCreatedEventArgs(Engine));
			}
		}

		public void Serialize<TDoc, TCursor>(KSoft.IO.TagElementStream<TDoc, TCursor, string> s)
			where TDoc : class
			where TCursor : class
		{
			mEngineCreationDisabled = true;

			s.StreamElementOpt("ProjectName", this, obj => obj.ProjectName, x => x != kDefaultProjectName);
			s.StreamElementEnumOpt("GameVersion", this, obj => obj.GameVersion, x => x != GameVersionType.DefinitiveEdition);
			s.StreamElementOpt("WorkDir", this, obj => obj.WorkDirectory, Predicates.IsNotNullOrEmpty);
			s.StreamElementOpt("FinalDir", this, obj => obj.FinalDirectory, Predicates.IsNotNullOrEmpty);

			mEngineCreationDisabled = false;

			if (s.IsReading)
			{
				CreateOrUnloadEngine();
			}
		}
	};
}
