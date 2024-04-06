using System;
using System.ComponentModel;

namespace PhxStudio.Modules.PhxInspectors.Conventions
{
	using Inspectors;

	public abstract class PropertyEditorBuilder
	{
		public abstract bool IsApplicable(PropertyDescriptor propertyDescriptor);
		public abstract IEditor BuildEditor(PropertyDescriptor propertyDescriptor);
	};

	public sealed class StandardPropertyEditorBuilder<T, TEditor>
		: PropertyEditorBuilder
		where TEditor : IEditor, new()
	{
		public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
			=> propertyDescriptor.PropertyType == typeof(T);

		public override IEditor BuildEditor(PropertyDescriptor propertyDescriptor)
			=> new TEditor();
	};

	public sealed class EnumPropertyEditorBuilder
		: PropertyEditorBuilder
	{
		public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
			=> typeof(Enum).IsAssignableFrom(propertyDescriptor.PropertyType);

		public override IEditor BuildEditor(PropertyDescriptor propertyDescriptor)
			=> new EnumEditorViewModel(propertyDescriptor.PropertyType);
	};

	public sealed class ProtoReferenceEditorBuilder
		: PropertyEditorBuilder
	{
		public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
		{
			if (propertyDescriptor.PropertyType != typeof(int))
			{
				return false;
			}

			foreach (var attr in propertyDescriptor.Attributes)
			{
				if (attr is KSoft.Phoenix.Phx.Meta.IProtoDataReferenceAttribute)
					return true;
			}

			return false;
		}

		public override IEditor BuildEditor(PropertyDescriptor propertyDescriptor)
		{
			foreach (var attr in propertyDescriptor.Attributes)
			{
				var refAttr = attr as KSoft.Phoenix.Phx.Meta.IProtoDataReferenceAttribute;
				if (refAttr != null)
				{
					return new ProtoDataReferenceViewModel(refAttr);
				}
			}
			return null;
		}
	};
}
