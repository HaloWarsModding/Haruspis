using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Framework.Services;

namespace PhxStudio.Modules.Main
{
	public interface IEditorProviderSelector
	{
		IEditorProvider GetEditor(string path, string pathName = null, string pathExtension = null);
	};

	[Export(typeof(IEditorProviderSelector))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	sealed class EditorProviderSelector
		: IEditorProviderSelector
	{
		private readonly IEditorProvider[] _editors;

		[ImportingConstructor]
		public EditorProviderSelector([ImportMany] IEditorProvider[] editors)
		{
			_editors = editors;
		}

		public IEditorProvider GetEditor(string path, string pathName = null, string pathExtension = null)
		{
			return _editors.FirstOrDefault(e => e.Handles(path));
		}
	}
}
