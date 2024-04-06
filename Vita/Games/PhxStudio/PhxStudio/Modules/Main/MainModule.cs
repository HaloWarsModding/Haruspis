using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Modules.StatusBar;
using Gemini.Modules.StatusBar.ViewModels;
using MahApps.Metro.Controls;

namespace PhxStudio.Modules.Main
{
	[Export(typeof(IModule))]
	[Export(typeof(MainModule))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	sealed class MainModule
		: ModuleBase
	{
		#region Imports
#pragma warning disable 649

		[Import] IStatusBar mStatusBar;

#pragma warning restore 649
		#endregion

		private IEventAggregator mEventAggregator;

		public StatusBarItemViewModel AppStateStatusBarItem { get; private set; }

		[ImportingConstructor]
		public MainModule(IEventAggregator eventAggregator)
		{
			mEventAggregator = eventAggregator;
		}

		private void OnAppClosing()
		{
			StoreWindowLocation();
		}

		public override void Initialize()
		{
			base.Initialize();

			Shell.ToolBars.Visible = true;

			mStatusBar.AddItem("Ready", new GridLength(1, GridUnitType.Star));
			AppStateStatusBarItem = mStatusBar.Items[0];
			//mStatusBar.AddItem("", new GridLength(100));
			//mStatusBar.AddItem("", new GridLength(100));

			RestoreWindowLocation();
		}

		public override void PostInitialize()
		{
			base.PostInitialize();
		}

		private void StoreWindowLocation()
		{
			var settings = Properties.Settings.Default;
			var mainWindow = Application.Current.MainWindow;
			if (settings == null || mainWindow == null)
				return;

			if (mainWindow.WindowState == WindowState.Normal)
			{
				settings.MainWindowTop = mainWindow.Top;
				settings.MainWindowLeft = mainWindow.Left;
			}
			else
			{
				settings.MainWindowTop = 0;
				settings.MainWindowLeft = 0;
			}

			settings.MainWindowWidth = mainWindow.Width;
			settings.MainWindowHeight = mainWindow.Height;
			settings.MainWindowState = mainWindow.WindowState == WindowState.Minimized
				? WindowState.Normal
				: mainWindow.WindowState;

			settings.Save();
		}

		private void RestoreWindowLocation()
		{
			var settings = Properties.Settings.Default;
			var mainWindow = Application.Current.MainWindow as MetroWindow;
			if (mainWindow == null)
				return;

			mainWindow.Top = settings.MainWindowTop;
			mainWindow.Left = settings.MainWindowLeft;

			if (mainWindow.Top <= 0 || mainWindow.Left <= 0)
				mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			else
				mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;

			mainWindow.Width = settings.MainWindowWidth;
			mainWindow.Height = settings.MainWindowHeight;

			mainWindow.WindowState = settings.MainWindowState;

			mainWindow.Closing += (sender, args) => OnAppClosing();
		}
	};
}
