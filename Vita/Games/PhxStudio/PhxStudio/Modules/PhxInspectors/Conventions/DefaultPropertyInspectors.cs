using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Vector4f = System.Numerics.Vector4;

namespace PhxStudio.Modules.PhxInspectors.Conventions
{
	using Inspectors;

	public static class DefaultPropertyInspectors
	{
		private static readonly List<PropertyEditorBuilder> gInspectorBuilders;

		public static List<PropertyEditorBuilder> InspectorBuilders
		{
			get { return gInspectorBuilders; }
		}

		static DefaultPropertyInspectors()
		{
			gInspectorBuilders = new List<PropertyEditorBuilder>
			{
				new RangePropertyEditorBuilder(),
				new EnumPropertyEditorBuilder(),

				// #NOTE_PHXSTUDIO needs to come before int editors! else they will take priority
				new ProtoReferenceEditorBuilder(),

				new StandardPropertyEditorBuilder<bool, CheckBoxEditorViewModel>(),

				new StandardPropertyEditorBuilder<sbyte, TextBoxEditorViewModel<sbyte>>(),

				new StandardPropertyEditorBuilder<short, TextBoxEditorViewModel<short>>(),

				new StandardPropertyEditorBuilder<int, TextBoxEditorViewModel<int>>(),
				new StandardPropertyEditorBuilder<int?, TextBoxEditorViewModel<int?>>(),

				new StandardPropertyEditorBuilder<float, TextBoxEditorViewModel<float>>(),
				new StandardPropertyEditorBuilder<float?, TextBoxEditorViewModel<float?>>(),

				new StandardPropertyEditorBuilder<double, TextBoxEditorViewModel<double>>(),
				new StandardPropertyEditorBuilder<double?, TextBoxEditorViewModel<double?>>(),

				new StandardPropertyEditorBuilder<string, TextBoxEditorViewModel<string>>(),

				new StandardPropertyEditorBuilder<Vector4f, SlimMathVector4ViewModel>(),

				new StandardPropertyEditorBuilder<Color, ColorEditorViewModel>(),
				new StandardPropertyEditorBuilder<Point3D, Point3DEditorViewModel>(),
				new StandardPropertyEditorBuilder<BitmapSource, BitmapSourceEditorViewModel>(),
			};
		}

		public static void Add(PropertyEditorBuilder builder)
		{
			InspectorBuilders.Add(builder);
		}

		public static void AddStandard<T, TEditor>()
			where TEditor : IEditor, new()
		{
			Add(new StandardPropertyEditorBuilder<T, TEditor>());
		}

		public static IEditor CreateEditor(PropertyDescriptor propertyDescriptor)
		{
			foreach (var inspectorBuilder in gInspectorBuilders)
			{
				if (inspectorBuilder.IsApplicable(propertyDescriptor))
					return inspectorBuilder.BuildEditor(propertyDescriptor);
			}
			return null;
		}
	};
}
