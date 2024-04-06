using System;
using System.Windows;

namespace PhxStudio.Modules.PhxInspectors.Controls
{
	public static class PhxInspectorGrid
	{
		public static event EventHandler PropertyNameColumnWidthChanged;
		public static event EventHandler PropertyValueColumnWidthChanged;

		private static GridLength gPropertyNameColumnWidth = new GridLength(1, GridUnitType.Star);
		public static GridLength PropertyNameColumnWidth
		{
			get { return gPropertyNameColumnWidth; }
			set
			{
				gPropertyNameColumnWidth = value;
				var handler = PropertyNameColumnWidthChanged;
				if (handler != null)
					handler(null, EventArgs.Empty);
			}
		}


		private static GridLength gPropertyValueColumnWidth = new GridLength(1.5, GridUnitType.Star);
		public static GridLength PropertyValueColumnWidth
		{
			get { return gPropertyValueColumnWidth; }
			set
			{
				gPropertyValueColumnWidth = value;
				var handler = PropertyValueColumnWidthChanged;
				if (handler != null)
					handler(null, EventArgs.Empty);
			}
		}
	};
}
