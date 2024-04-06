using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	using BoundPropertyDescriptor = Gemini.Modules.Inspector.BoundPropertyDescriptor;

	public interface IEditor
		: IInspector
	{
		BoundPropertyDescriptor BoundPropertyDescriptor { get; set; }
		bool CanReset { get; }
		void Reset();

		void HandleViewLoaded();
	};

	public abstract class EditorBase<TValue>
		: InspectorBase
		, IEditor
		, IDisposable
	{
		private BoundPropertyDescriptor mBoundPropertyDescriptor;
		protected IShell mShell;

		public EditorBase()
		{
			mShell = IoC.Get<IShell>();
			IsUndoEnabled = true;
		}

		public override string Name => BoundPropertyDescriptor.PropertyDescriptor.DisplayName;

		public override bool IsReadOnly => BoundPropertyDescriptor.PropertyDescriptor.IsReadOnly;

		public bool IsUndoEnabled { get; set; }

		public IValueConverter Converter { get; set; }

		public IValueConverter StringConverter { get; set; }

		public bool CanReset { get {
			if (IsReadOnly)
				return false;

			return BoundPropertyDescriptor.PropertyDescriptor.CanResetValue(BoundPropertyDescriptor.PropertyOwner);
		} }

		public void Reset()
		{
			if (CanReset)
			{
				var item = mShell.ActiveItem;
				if (IsUndoEnabled && item != null)
				{
					item.UndoRedoManager.ExecuteAction(
						new ResetObjectValueAction(BoundPropertyDescriptor, StringConverter));
				}
				else
				{
					BoundPropertyDescriptor.PropertyDescriptor.ResetValue(BoundPropertyDescriptor.PropertyOwner);
				}
			}
		}

		public virtual void HandleViewLoaded()
		{
		}

		public string Description { get {
			if (!string.IsNullOrEmpty(BoundPropertyDescriptor.PropertyDescriptor.Description))
				return BoundPropertyDescriptor.PropertyDescriptor.Description;
			return Name;
		} }

		private void CleanupPropertyChanged()
		{
			if (mBoundPropertyDescriptor != null)
			{
				if (mBoundPropertyDescriptor.PropertyDescriptor.SupportsChangeEvents)
				{
					mBoundPropertyDescriptor.ValueChanged -= OnValueChanged;
				}
				else if (typeof(INotifyPropertyChanged).IsAssignableFrom(mBoundPropertyDescriptor.PropertyOwner.GetType()))
				{
					((INotifyPropertyChanged)mBoundPropertyDescriptor.PropertyOwner).PropertyChanged -= OnPropertyChanged;
				}
			}
		}

		public BoundPropertyDescriptor BoundPropertyDescriptor
		{
			get { return mBoundPropertyDescriptor; }
			set
			{
				CleanupPropertyChanged();

				mBoundPropertyDescriptor = value;

				if (value.PropertyDescriptor.SupportsChangeEvents)
				{
					value.ValueChanged += OnValueChanged;
				}
				else if (typeof(INotifyPropertyChanged).IsAssignableFrom(value.PropertyOwner.GetType()))
				{
					((INotifyPropertyChanged)value.PropertyOwner).PropertyChanged += OnPropertyChanged;
				}
			}
		}

		public bool IsDirty { get {
			var defaultAttribute = BoundPropertyDescriptor.PropertyDescriptor.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
			if (defaultAttribute == null)
				/* Maybe not dirty, but we have no way to know if we don't have a default value */
				return true;

			return !Equals(defaultAttribute.Value, Value);
		} }

		private void OnValueChanged()
		{
			NotifyOfPropertyChange(nameof(Value));
			NotifyOfPropertyChange(nameof(IsDirty));
		}

		private void OnValueChanged(object sender, EventArgs e)
		{
			OnValueChanged();
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals(BoundPropertyDescriptor.PropertyDescriptor.Name))
				OnValueChanged();
		}

		public TValue Value
		{
			get
			{
				if (!typeof(TValue).IsAssignableFrom(BoundPropertyDescriptor.PropertyDescriptor.PropertyType))
				{
					if (Converter == null)
						throw new InvalidCastException("editor property value does not match editor type and no converter specified");

					return (TValue)Converter.Convert(RawValue, typeof(TValue), null, CultureInfo.CurrentCulture);
				}

				return (TValue)RawValue;
			}

			set
			{
				if (Equals(Value, value))
					return;

				object newValue = value;
				if (!typeof(TValue).IsAssignableFrom(BoundPropertyDescriptor.PropertyDescriptor.PropertyType))
				{
					if (Converter == null)
						throw new InvalidCastException("editor property value does not match editor type and no converter specified");

					newValue = Converter.ConvertBack(value, BoundPropertyDescriptor.PropertyDescriptor.PropertyType, null, CultureInfo.CurrentCulture);
				}

				/* Only notify of property change once */
				IsNotifying = false;

				try
				{
					var item = mShell.ActiveItem;
					if (IsUndoEnabled && item != null)
					{
						item.UndoRedoManager.ExecuteAction(
							new ChangeObjectValueAction(BoundPropertyDescriptor, newValue, StringConverter));
					}
					else
					{
						RawValue = newValue;
					}
				}
				finally
				{
					IsNotifying = true;
				}

				OnValueChanged();
			}
		}

		protected object RawValue
		{
			get { return BoundPropertyDescriptor.Value; }
			set { BoundPropertyDescriptor.Value = value; }
		}

		public virtual void Dispose()
		{
			CleanupPropertyChanged();
		}
	};

	/// <summary>
	/// This class is used for values that should only be updated after the
	/// user has finished editing them. The view needs to call OnBeginEdit when
	/// the user has started editing to capture the current value and call
	/// OnEndEdit to commit the old and new value to the undo / redo manager.
	/// </summary>
	/// <typeparam name="TValue">Type of the value</typeparam>
	public abstract class SelectiveUndoEditorBase<TValue>
		: EditorBase<TValue>
		, IDisposable
	{
		private object mOriginalValue = null;

		protected void OnBeginEdit()
		{
			IsUndoEnabled = false;
			mOriginalValue = RawValue;
		}

		protected void OnEndEdit()
		{
			if (mOriginalValue == null)
				return;

			try
			{
				var value = RawValue;
				if (!mOriginalValue.Equals(value))
					mShell.ActiveItem.UndoRedoManager.ExecuteAction(
						new ChangeObjectValueAction(BoundPropertyDescriptor, mOriginalValue, value, StringConverter));
			}
			finally
			{
				mOriginalValue = null;
				IsUndoEnabled = true;
			}
		}

		public override void Dispose()
		{
			OnEndEdit();
			base.Dispose();
		}
	};

	public sealed class TextBoxEditorViewModel<T>
		: EditorBase<T>
		, ILabelledInspector
	{
	};
}
