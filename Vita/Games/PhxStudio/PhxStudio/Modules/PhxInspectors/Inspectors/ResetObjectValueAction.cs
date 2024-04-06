using System.Globalization;
using System.Windows.Data;
using Gemini.Modules.UndoRedo;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	using BoundPropertyDescriptor = Gemini.Modules.Inspector.BoundPropertyDescriptor;

	public class ResetObjectValueAction : IUndoableAction
	{
		private readonly BoundPropertyDescriptor mBoundPropertyDescriptor;
		private readonly object mOriginalValue;
		private object mNewValue;
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

				return string.Format("Reset {0} from {1} to {2}",
					mBoundPropertyDescriptor.PropertyDescriptor.DisplayName,
					origText,
					newText);
			}
		}

		public ResetObjectValueAction(BoundPropertyDescriptor boundPropertyDescriptor, IValueConverter stringConverter) :
			this(boundPropertyDescriptor, boundPropertyDescriptor.Value, stringConverter)
		{ }

		public ResetObjectValueAction(BoundPropertyDescriptor boundPropertyDescriptor, object originalValue, IValueConverter stringConverter)
		{
			mBoundPropertyDescriptor = boundPropertyDescriptor;
			mOriginalValue = originalValue;
			mStringConverter = stringConverter;
		}

		public void Execute()
		{
			mBoundPropertyDescriptor.PropertyDescriptor.ResetValue(mBoundPropertyDescriptor.PropertyOwner);
			mNewValue = mBoundPropertyDescriptor.Value;
		}

		public void Undo()
		{
			mBoundPropertyDescriptor.Value = mOriginalValue;
		}
	};
}
