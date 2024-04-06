using System.Windows.Controls;

namespace PhxStudio.Modules.PhxInspectors
{
	public partial class PhxInspectorView : UserControl
	{
		public PhxInspectorViewModel ViewModel
		{
			get { return DataContext as PhxInspectorViewModel; }
		}

		public PhxInspectorView()
		{
			InitializeComponent();
		}

		private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
		{
			var vm = ViewModel;
			if (vm != null)
			{
				vm.HandleViewLoaded();
			}
		}
	}
}
