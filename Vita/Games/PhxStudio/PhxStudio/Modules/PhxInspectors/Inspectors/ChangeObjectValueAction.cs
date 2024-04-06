using System.Globalization;
using System.Windows.Data;
using Gemini.Modules.UndoRedo;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	using BoundPropertyDescriptor = Gemini.Modules.Inspector.BoundPropertyDescriptor;

	public class ChangeObjectValueAction
		: IUndoableAction
	{
		private readonly BoundPropertyDescriptor mBoundPropertyDescriptor;
		private readonly object mOriginalValue;
		private readonly object mNewValue;
		private readonly IValueConverter mStringConverter;

		public string Name
		{
			get
			{
				string origText;
				string newText;

				if (mStringConverter != null)
				{
					origText = (string)mStringConverter.Convert(mOriginalValue, typeof(string), null, CultureInfo.CurrentUICulture);
					newText = (string)mStringConverter.Convert(mNewValue, typeof(string), null, CultureInfo.CurrentUICulture);
				}
				else
				{
					origText = mOriginalValue.ToString();
					newText = mNewValue.ToString();
				}

				return string.Format("Change {0} from {1} to {2}",
					mBoundPropertyDescriptor.PropertyDescriptor.DisplayName,
					origText,
					newText);
			}
		}

		public ChangeObjectValueAction(BoundPropertyDescriptor boundPropertyDescriptor, object newValue, IValueConverter stringConverter) :
			this(boundPropertyDescriptor, boundPropertyDescriptor.Value, newValue, stringConverter)
		{ }

		public ChangeObjectValueAction(BoundPropertyDescriptor boundPropertyDescriptor, object originalValue, object newValue, IValueConverter stringConverter)
		{
			mBoundPropertyDescriptor = boundPropertyDescriptor;
			mOriginalValue = originalValue;
			mNewValue = newValue;
			mStringConverter = stringConverter;
		}

		public void Execute()
		{
			mBoundPropertyDescriptor.Value = mNewValue;
		}

		public void Undo()
		{
			mBoundPropertyDescriptor.Value = mOriginalValue;
		}
	};
}
