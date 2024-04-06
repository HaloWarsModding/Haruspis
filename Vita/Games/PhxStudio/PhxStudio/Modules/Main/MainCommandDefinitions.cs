using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace PhxStudio.Modules.Main.Commands
{
	[CommandDefinition]
	public sealed class PhxOpenFileCommandDefinition
		: CommandDefinition
	{
		public const string CommandName = "File.OpenFile";

		public override string Name => CommandName;

		public override string Text => "_Open";

		public override string ToolTip => "Open File";

		public override Uri IconSource => new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Open.png");

		[Export]
		public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<PhxOpenFileCommandDefinition>(
			new KeyGesture(Key.O, ModifierKeys.Control));
	}
}