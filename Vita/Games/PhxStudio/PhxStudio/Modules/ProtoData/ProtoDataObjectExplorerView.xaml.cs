using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PhxStudio.Modules.ProtoData
{
	/// <summary>
	/// Interaction logic for ProtoDataObjectExplorerView.xaml
	/// </summary>
	public partial class ProtoDataObjectExplorerView : UserControl
	{
		private ProtoDataObjectExplorerViewModel ViewModel => DataContext as ProtoDataObjectExplorerViewModel;

		private ICollectionView SourceObjectDatabaseCollectionView { get {
			if (SourceObjectDatabaseCollectionListView.ItemsSource == null)
				return null;

			return CollectionViewSource.GetDefaultView(SourceObjectDatabaseCollectionListView.ItemsSource);
		} }

		public ProtoDataObjectExplorerView()
		{
			InitializeComponent();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var vm = ViewModel;
			if (vm != null)
			{
				vm.LookupViewModel.PropertyChanged += OnViewModelPropertyChanged;
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			var vm = ViewModel;
			if (vm != null)
			{
				vm.LookupViewModel.PropertyChanged -= OnViewModelPropertyChanged;
			}
		}

		private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ProtoDataObjectLookupViewModel.SourceObjectDatabaseCollection))
			{
				var vm = ViewModel;
				if (vm.LookupViewModel.SourceObjectDatabaseCollection != null)
				{
					var view = SourceObjectDatabaseCollectionView;
					if (view != null)
					{
						view.Filter = vm.LookupViewModel.IsSourceObjectDatabaseCollectionItemFiltered;
					}
				}
			}
		}

		private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
		{
			var view = SourceObjectDatabaseCollectionView;
			if (view != null)
				view.Refresh();
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
		}
	};

	class SourceObjectDatabaseCollectionListViewDataTemplateSelector
		: DataTemplateSelector
	{
		private DataTemplate
			IDatabaseIdObjectDataTemplate,
			IListAutoIdObjectDataTemplate,
			StringDataTemplate
			;

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var element = container as FrameworkElement;

			if (item is KSoft.Phoenix.Phx.IDatabaseIdObject
				// #NOTE_PHXSTUDIO the DBID in techs is not used by the engine
				&& !(item is KSoft.Phoenix.Phx.BProtoTech)
				)
			{
				if (IDatabaseIdObjectDataTemplate == null)
					IDatabaseIdObjectDataTemplate = element.FindResource("IDatabaseIdObjectDataTemplate") as DataTemplate;

				return IDatabaseIdObjectDataTemplate;
			}
			else if (item is KSoft.Collections.IListAutoIdObject)
			{
				if (IListAutoIdObjectDataTemplate == null)
					IListAutoIdObjectDataTemplate = element.FindResource("IListAutoIdObjectDataTemplate") as DataTemplate;

				return IListAutoIdObjectDataTemplate;
			}
			else if (item is string)
			{
				if (StringDataTemplate == null)
					StringDataTemplate = element.FindResource("StringDataTemplate") as DataTemplate;

				return StringDataTemplate;
			}

			return null;
		}
	};
}
