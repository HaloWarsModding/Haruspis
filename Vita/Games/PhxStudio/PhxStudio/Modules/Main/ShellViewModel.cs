using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace PhxStudio.Modules.Main
{
	public interface IPhxShell
		: IShell
	{
		bool IsBusy { get; set; }
	};

	[Export(typeof(IShell))]
	[Export(typeof(IPhxShell))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class ShellViewModel
		: Gemini.Modules.Shell.ViewModels.ShellViewModel
		, IPhxShell
	{
		bool mIsBusy;
		public bool IsBusy
		{
			get { return mIsBusy; }
			set { this.SetFieldVal(ref mIsBusy, value); }
		}

		public override void CanClose(Action<bool> callback)
		{
			Coroutine.BeginExecute(CanClose().GetEnumerator(), null, (s, e) => callback(!e.WasCancelled));
		}

		private IEnumerable<IResult> CanClose()
		{
			yield return new ConfirmQuitMessageBoxResult();
		}

		private class ConfirmQuitMessageBoxResult
			: IResult
		{
			public event EventHandler<ResultCompletionEventArgs> Completed;

			public /*async*/ void Execute(CoroutineExecutionContext context)
			{
				var result = System.Windows.MessageBoxResult.Yes;
#if false
				var connectionHandler = IoC.Get<IConnectionHandler>();

				if (Settings.Default.Connection_ConfirmOnCloseConnected && connectionHandler.ConnectionRequestState == ConnectionRequestState.Start)
				{
					result = MessageBox.Show("You are connected, you want to exit?", "Confirm", MessageBoxButton.YesNo);
					if (result == System.Windows.MessageBoxResult.Yes)
						await connectionHandler.StopSession();
				}
#endif

				if (Completed != null)
					Completed(this, new ResultCompletionEventArgs { WasCancelled = (result != System.Windows.MessageBoxResult.Yes) });
			}
		}
	};
}
