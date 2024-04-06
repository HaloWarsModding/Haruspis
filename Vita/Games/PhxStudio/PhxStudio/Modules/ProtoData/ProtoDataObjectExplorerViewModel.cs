using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace PhxStudio.Modules.ProtoData
{
	public interface IProtoDataObjectExplorerTool
		: ITool
	{
		ProtoDataObjectLookupViewModel LookupViewModel { get; }
	};

	public abstract class ProtoDataObjectExplorerViewModel
		: Tool
		, IProtoDataObjectExplorerTool
	{
		#region Imports
#pragma warning disable 649

		[Import] IShell mShell;

#pragma warning restore 649

		protected IShell Shell { get { return mShell; } }
		#endregion

		public override PaneLocation PreferredLocation => PaneLocation.Right;

		public override double PreferredWidth => 150;

		public ProtoDataObjectLookupViewModel LookupViewModel { get; protected set; }

		public void OnMouseDown(object source, KSoft.Collections.IListAutoIdObject obj, MouseButtonEventArgs args)
		{
			if (args.LeftButton == MouseButtonState.Pressed && args.ClickCount == 2)
			{
				OnOpenObject(obj);
			}
		}

		protected virtual void OnOpenObject(object obj)
		{
		}
	};

	abstract class ProtoDataViewObjectExplorerCommandHandlerBase<TCommandDefinition>
		: CommandHandlerBase<TCommandDefinition>
		where TCommandDefinition : CommandDefinition
	{
#pragma warning disable 649
		[Import] IShell mShell;
		[Import] Project.IProjectService mProjectService;
#pragma warning restore 649

		protected IShell Shell => mShell;

		public override void Update(Command command)
		{
			base.Update(command);

			var engine = mProjectService.CurrentProject.Model.Engine;

			command.Enabled = engine != null && engine.HasAlreadyPreloaded;
		}
	};
}
