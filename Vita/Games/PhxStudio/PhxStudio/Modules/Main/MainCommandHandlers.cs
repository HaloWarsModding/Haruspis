using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Microsoft.Win32;
using KSoft;

namespace PhxStudio.Modules.Main.Commands
{
	[CommandHandler]
	public class PhxOpenFileCommandHandler
		: CommandHandlerBase<PhxOpenFileCommandDefinition>
	{
		private readonly IShell mShell;
		private readonly IEditorProvider[] mEditorProviders;

		[ImportingConstructor]
		public PhxOpenFileCommandHandler(IShell shell, [ImportMany] IEditorProvider[] editorProviders)
		{
			mShell = shell;
			mEditorProviders = editorProviders;
		}

		public override void Update(Command command)
		{
			base.Update(command);

			// #TODO_PHXSTUDIO this does not work
			// https://github.com/tgjones/gemini/issues/174
			// https://github.com/tgjones/gemini/pull/134/commits/7fa412c75010748902d17a44d2a9f871b1840893
			command.Visible = mEditorProviders.IsNotNullOrEmpty();
		}

		public override async Task Run(Command command)
		{
			// #HACK_PHXSTUDIO due to issue 174
			if (!command.Visible)
				return;

			var dialog = new OpenFileDialog();

			string filter = null;

			filter = "All Supported Files|" + string.Join(";",
				mEditorProviders
					.SelectMany(x => x.FileTypes)
					.Select(x => "*" + x.FileExtension)
				);
			filter += "|" + string.Join("|",
				mEditorProviders
					.SelectMany(x => x.FileTypes)
					.Select(x => x.Name + "|*" + x.FileExtension)
				);

			dialog.Filter = filter;

			if (dialog.ShowDialog() == true)
				mShell.OpenDocument(await GetEditor(dialog.FileName));
		}

		internal static Task<IDocument> GetEditor(string path)
		{
			var provider = IoC.GetAllInstances(typeof(IEditorProvider))
				.Cast<IEditorProvider>()
				.FirstOrDefault(p => p.Handles(path));
			if (provider == null)
				return null;

			var editor = provider.Create();

			var viewAware = (IViewAware)editor;
			viewAware.ViewAttached += (sender, e) =>
			{
				var frameworkElement = (FrameworkElement)e.View;

				RoutedEventHandler loadedHandler = null;
				loadedHandler = async (sender2, e2) =>
				{
					frameworkElement.Loaded -= loadedHandler;
					await provider.Open(editor, path);
				};
				frameworkElement.Loaded += loadedHandler;
			};

			return Task.FromResult(editor);
		}
	};
}
