using System;
using System.Collections.Generic;
using System.Linq;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	public sealed class EnumValueViewModel<TEnum>
	{
		public TEnum Value { get; set; }
		public string Text { get; set; }
	};

	public class EnumEditorViewModel<TEnum>
		: EditorBase<TEnum>
		, ILabelledInspector
	{
		public IEnumerable<EnumValueViewModel<TEnum>> Items { get; private set; }

		public EnumEditorViewModel()
		{
			Items = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(x => new EnumValueViewModel<TEnum>
			{
				Value = x,
				Text = Enum.GetName(typeof(TEnum), x)
			}).ToList();
		}
	};

	public sealed class EnumValueViewModel
	{
		public object Value { get; set; }
		public string Text { get; set; }
	};

	public sealed class EnumEditorViewModel
		: EditorBase<Enum>
		, ILabelledInspector
	{
		public IEnumerable<EnumValueViewModel> Items { get; private set; }

		public EnumEditorViewModel(Type enumType)
		{
			Items = Enum.GetValues(enumType).Cast<object>().Select(x => new EnumValueViewModel
			{
				Value = x,
				Text = Enum.GetName(enumType, x)
			}).ToList();
		}
	};
}
