using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PhxStudio.Modules.TraceList
{
	[ValueConversion(typeof(TraceListItemType), typeof(ImageSource))]
	public sealed class TraceListItemTypeToImageConverter
		: IValueConverter
	{
		public ImageSource InvalidImageSource { get; set; }

		public ImageSource CriticalImageSource { get; set; }
		public ImageSource ErrorImageSource { get; set; }
		public ImageSource WarningImageSource { get; set; }
		public ImageSource InformationImageSource { get; set; }
		public ImageSource VerboseImageSource { get; set; }

		public ImageSource StartImageSource { get; set; }
		public ImageSource StopImageSource { get; set; }
		public ImageSource SuspendImageSource { get; set; }
		public ImageSource ResumeImageSource { get; set; }
		public ImageSource TransferImageSource { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((TraceListItemType)value)
			{
				case TraceListItemType.Critical:
					return CriticalImageSource;
				case TraceListItemType.Error:
					return ErrorImageSource;
				case TraceListItemType.Warning:
					return WarningImageSource;
				case TraceListItemType.Information:
					return InformationImageSource;
				case TraceListItemType.Verbose:
					return VerboseImageSource;

				case TraceListItemType.Start:
					return StartImageSource;
				case TraceListItemType.Stop:
					return StopImageSource;
				case TraceListItemType.Suspend:
					return SuspendImageSource;
				case TraceListItemType.Resume:
					return ResumeImageSource;
				case TraceListItemType.Transfer:
					return TransferImageSource;

				default:
					return InvalidImageSource;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	};
}
