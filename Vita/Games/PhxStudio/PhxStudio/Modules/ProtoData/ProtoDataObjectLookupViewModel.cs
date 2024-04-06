using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace PhxStudio.Modules.ProtoData
{
	public interface IProtoDataObjectLookup
		: System.ComponentModel.INotifyPropertyChanged
	{
		KSoft.Phoenix.Phx.ProtoDataObjectSource ObjectSource { get; }

		int SourceObjectDatabaseKindId { get; }

		KSoft.Phoenix.Phx.ProtoDataObjectDatabase SourceObjectDatabase { get; }

		object SourceObjectDatabaseCollection { get; }
		string SourceObjectDatabaseCollectionFilter { get; }
		ObservableCollection<string> SourceObjectDatabaseUndefinedMembers { get; }
		bool HasSourceObjectDatabaseUndefinedMembers { get; }
	};

	public abstract class ProtoDataObjectLookupViewModel
		: KSoft.ObjectModel.BasicViewModel
		, IProtoDataObjectLookup
		, IHandle<Project.ProjectEngineUnloadedEventArgs>
		, IHandle<Project.ProjectEnginePreloadedEventArgs>
		, IHandle<Project.ProjectEngineLoadedEventArgs>
	{
		#region Imports
#pragma warning disable 649

		[Import] IShell mShell;
		IEventAggregator mEventAggregator;

#pragma warning restore 649

		protected IShell Shell { get { return mShell; } }
		#endregion

		KSoft.Phoenix.Phx.ProtoDataObjectSource mObjectSource;
		public KSoft.Phoenix.Phx.ProtoDataObjectSource ObjectSource
		{
			get { return mObjectSource; }
			protected set { this.SetField(ref mObjectSource, value); }
		}

		public int SourceObjectDatabaseKindId { get; private set; }

		KSoft.Phoenix.Phx.ProtoDataObjectDatabase mSourceObjectDatabase;
		public KSoft.Phoenix.Phx.ProtoDataObjectDatabase SourceObjectDatabase
		{
			get { return mSourceObjectDatabase; }
			protected set
			{
				if (this.SetField(ref mSourceObjectDatabase, value))
				{
					SetupSourceObjectDatabaseCollection();
				}
			}
		}

		object mSourceObjectDatabaseCollection;
		public object SourceObjectDatabaseCollection
		{
			get { return mSourceObjectDatabaseCollection; }
			private set { this.SetField(ref mSourceObjectDatabaseCollection, value); }
		}

		string mSourceObjectDatabaseCollectionFilter;
		public string SourceObjectDatabaseCollectionFilter
		{
			get { return mSourceObjectDatabaseCollectionFilter; }
			set { this.SetFieldObj(ref mSourceObjectDatabaseCollectionFilter, value); }
		}

		ObservableCollection<string> mSourceObjectDatabaseUndefinedMembers;
		public ObservableCollection<string> SourceObjectDatabaseUndefinedMembers
		{
			get { return mSourceObjectDatabaseUndefinedMembers; }
			private set
			{
				if (this.SetField(ref mSourceObjectDatabaseUndefinedMembers, value))
				{
					this.OnPropertyChanged(nameof(HasSourceObjectDatabaseUndefinedMembers));
				}
			}
		}

		public bool HasSourceObjectDatabaseUndefinedMembers => SourceObjectDatabaseUndefinedMembers != null && SourceObjectDatabaseUndefinedMembers.Count > 0;

		// #HACK_PHXSTUDIO Civs and Leaders are not currently preloaded and need this set
		protected bool mObjectsArePreloaded = false;//true;

		protected ProtoDataObjectLookupViewModel(int sourceObjectDatabaseKindId)
		{
			mEventAggregator = IoC.Get<IEventAggregator>();
			mEventAggregator.Subscribe(this);

			SourceObjectDatabaseKindId = sourceObjectDatabaseKindId;
		}

		private void SetupSourceObjectDatabaseCollection()
		{
			if (SourceObjectDatabase == null)
			{
				SourceObjectDatabaseCollection = null;
				SourceObjectDatabaseUndefinedMembers = null;
				return;
			}

			var provider = SourceObjectDatabase.Provider;
			var list = provider.GetNamesInterface(SourceObjectDatabaseKindId);
			SourceObjectDatabaseCollection = list.UnderlyingObjectsCollection;
		}

		private void SetupSourceObjectDatabaseUndefinedMembers()
		{
			if (SourceObjectDatabase == null)
			{
				return;
			}

			var provider = SourceObjectDatabase.Provider;
			var list = provider.GetNamesInterface(SourceObjectDatabaseKindId);
			SourceObjectDatabaseUndefinedMembers = list.UndefinedInterface.UndefinedMembers;
		}

		public bool IsSourceObjectDatabaseCollectionItemFiltered(object obj)
		{
			string filter = SourceObjectDatabaseCollectionFilter;
			if (string.IsNullOrEmpty(filter))
			{
				return true;
			}
			else if (obj == null)
			{
				return false;
			}

			var item = obj as KSoft.Collections.IListAutoIdObject;
			if (item != null)
			{
				return item.Data != null
					//&& item.Data.Contains(filter)
					&& item.Data.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
					;
			}

			var itemAsStr = obj as string;
			if (itemAsStr != null)
			{
				return
					//itemAsStr.Contains(filter)
					itemAsStr.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
					;
			}

			return true;
		}

		#region ProjectEngineUnloadedEventArgs
		void IHandle<Project.ProjectEngineUnloadedEventArgs>.Handle(Project.ProjectEngineUnloadedEventArgs message) => OnProjectEngineUnloaded(message);

		protected virtual void OnProjectEngineUnloaded(Project.ProjectEngineUnloadedEventArgs message) => SourceObjectDatabase = null;
		#endregion

		#region ProjectEnginePreloadedEventArgs
		void IHandle<Project.ProjectEnginePreloadedEventArgs>.Handle(Project.ProjectEnginePreloadedEventArgs message) => OnProjectEnginePreloaded(message);

		protected virtual void OnProjectEnginePreloaded(Project.ProjectEnginePreloadedEventArgs message)
		{
			if (mObjectsArePreloaded)
			{
				var engine = message.Engine;

				SourceObjectDatabase = ObjectSource.GetObjectDatabase(engine);
			}
		}
		#endregion

		#region ProjectEngineLoadedEventArgs
		void IHandle<Project.ProjectEngineLoadedEventArgs>.Handle(Project.ProjectEngineLoadedEventArgs message) => OnProjectEngineLoaded(message);

		protected virtual void OnProjectEngineLoaded(Project.ProjectEngineLoadedEventArgs message)
		{
			if (mObjectsArePreloaded == false)
			{
				var engine = message.Engine;

				SourceObjectDatabase = ObjectSource.GetObjectDatabase(engine);
			}

			// This needs to be setup after Load because the undefined members are modified during load, so there would be a data binding error if we set up after Preload
			SetupSourceObjectDatabaseUndefinedMembers();
		}
		#endregion
	};
}
