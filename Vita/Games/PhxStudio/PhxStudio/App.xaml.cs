using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using KSoft;

namespace PhxStudio
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static List<TraceSource> AllTraceSources { get; private set; } = KSoft.Debug.AssemblyTraceSourcesCollector.FromClasses(null
			, KSoft.Program.DebugTraceClass
			, KSoft.Phoenix.Program.DebugTraceClass
			, typeof(Debug.Trace)
			).SortAndReturn(CompareTraceSourcesByName);
		private static int CompareTraceSourcesByName(TraceSource x, TraceSource y)
		{
			return string.CompareOrdinal(x.Name, y.Name);
		}

		internal static Modules.Project.PhxStudioProjectViewModel CurrentProjectViewModel { get {
			return (Modules.Project.PhxStudioProjectViewModel)Application.Current.FindResource("CurrentProjectViewModel");
		} }

		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.UnhandledException += new
				UnhandledExceptionEventHandler(this.AppDomainUnhandledExceptionHandler);

			base.OnStartup(e);

			KSoft.Program.Initialize();
			KSoft.Phoenix.Program.Initialize();

			var settings = PhxStudio.Properties.Settings.Default;
			if (settings.TraceSourceOptions == null)
				settings.TraceSourceOptions = new Modules.TraceList.TraceSourceSettings();

			settings.TraceSourceOptions.UpdateAfterSettingsLoaded();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			KSoft.Phoenix.Program.Dispose();
			KSoft.Program.Dispose();
		}

		void AppDomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs ea)
		{
			Exception e = (Exception)ea.ExceptionObject;
			Debug.Trace.PhxStudio.TraceData(TraceEventType.Error, TypeExtensions.kNone,
				"Unhandled Exception!",
				e);
		}

		#region AppIconBitmap
		static RenderTargetBitmap gAppIconBitmap;
		public static RenderTargetBitmap AppIconBitmap { get {
			// I used to have this setup in OnActivated, but that is called every time the app is put in the foreground.
			// Initializing it in OnStartup or OnLoaded is too late.
			// Lazy loading, however, works (on my machine)
			if (gAppIconBitmap == null)
				RenderAppIconBitmap();

			return gAppIconBitmap;
		} }

		private static void RenderAppIconBitmap()
		{
			var grid = (Grid)Application.Current.FindResource("PhxLogoGrid");
			if (grid == null)
				throw new ArgumentException("Failed to find logo Grid", "PhxLogoGrid");

			var viewbox = new Viewbox();
			viewbox.Child = grid;
			viewbox.Measure(new Size(512, 512));
			viewbox.Arrange(new Rect(0, 0, 512, 512));
			viewbox.UpdateLayout();

			var viewbox_ps = PresentationSource.FromVisual(viewbox);
			double dpiX = 96.0, dpiY = 96.0;
			if (viewbox_ps != null)
			{
				dpiX *= viewbox_ps.CompositionTarget.TransformToDevice.M11;
				dpiY *= viewbox_ps.CompositionTarget.TransformToDevice.M22;
			}

			gAppIconBitmap = new RenderTargetBitmap((int)viewbox.ActualWidth, (int)viewbox.ActualHeight, dpiX, dpiY, PixelFormats.Pbgra32);
			gAppIconBitmap.Render(viewbox);
		}
		#endregion
	};
}
