using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PhxStudio.Modules.PhxInspectors
{
	using BoundPropertyDescriptor = Gemini.Modules.Inspector.BoundPropertyDescriptor;
	using Conventions;
	using Inspectors;

	public abstract class InspectorBuilderBase
	{
		private readonly List<IInspector> mInspectors = new List<IInspector>();
		protected List<IInspector> Inspectors
		{
			get { return mInspectors; }
		}

		public bool HasInspectors => mInspectors != null && mInspectors.Count > 0;

		private Dictionary<Type, PropertyDescriptorCollection> mCachedPropertyDescriptors;

		protected PropertyDescriptorCollection GetPropertyDescriptors(Type type)
		{
			if (mCachedPropertyDescriptors == null)
				mCachedPropertyDescriptors = new Dictionary<Type, PropertyDescriptorCollection>();

			PropertyDescriptorCollection pdc;
			if (!mCachedPropertyDescriptors.TryGetValue(type, out pdc))
			{
				pdc = TypeDescriptor.GetProperties(type);
				mCachedPropertyDescriptors.Add(type, pdc);
			}

			return pdc;
		}

		protected static void AddProperties(object instance, IEnumerable<PropertyDescriptor> properties, List<IInspector> inspectors)
		{
			foreach (var property in properties)
			{
				var editor = DefaultPropertyInspectors.CreateEditor(property);
				if (editor != null)
				{
					editor.BoundPropertyDescriptor = new BoundPropertyDescriptor(instance, property);
					inspectors.Add(editor);
				}
			}
		}
	};

	public class InspectorBuilder<TBuilder>
		: InspectorBuilderBase
		where TBuilder : InspectorBuilder<TBuilder>
	{
		public TBuilder WithCollapsibleGroup(string name, Func<CollapsibleGroupBuilder, CollapsibleGroupBuilder> callback)
		{
			var builder = new CollapsibleGroupBuilder();
			return WithCollapsibleGroup(name, callback(builder));
		}

		public TBuilder WithCollapsibleGroup(string name, CollapsibleGroupBuilder builder)
		{
			Inspectors.Add(builder.ToCollapsibleGroup(name));
			return (TBuilder)this;
		}

		public TBuilder WithCheckBoxEditor<T>(T instance, Expression<Func<T, bool>> propertyExpression)
			=> WithEditor<T, bool, CheckBoxEditorViewModel>(instance, propertyExpression);

		public TBuilder WithSignedEditor<T>(T instance, Expression<Func<T, sbyte>> propertyExpression)
			=> WithEditor<T, sbyte, TextBoxEditorViewModel<sbyte>>(instance, propertyExpression);
		public TBuilder WithSignedEditor<T>(T instance, Expression<Func<T, short>> propertyExpression)
			=> WithEditor<T, short, TextBoxEditorViewModel<short>>(instance, propertyExpression);
		public TBuilder WithSignedEditor<T>(T instance, Expression<Func<T, int>> propertyExpression)
			=> WithEditor<T, int, TextBoxEditorViewModel<int>>(instance, propertyExpression);

		public TBuilder WithColorEditor<T>(T instance, Expression<Func<T, Color>> propertyExpression)
			=> WithEditor<T, Color, ColorEditorViewModel>(instance, propertyExpression);

		public TBuilder WithEnumEditor<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyExpression)
			=> WithEditor<T, TProperty, EnumEditorViewModel<TProperty>>(instance, propertyExpression);

		public TBuilder WithPoint3DEditor<T>(T instance, Expression<Func<T, Point3D>> propertyExpression)
			=> WithEditor<T, Point3D, Point3DEditorViewModel>(instance, propertyExpression);

		public TBuilder WithRangeEditor<T>(T instance, Expression<Func<T, float>> propertyExpression, float minimum, float maximum)
			=> WithEditor(instance, propertyExpression, new RangeEditorViewModel<float>(minimum, maximum));

		public TBuilder WithRangeEditor<T>(T instance, Expression<Func<T, double>> propertyExpression, double minimum, double maximum)
			=> WithEditor(instance, propertyExpression, new RangeEditorViewModel<double>(minimum, maximum));

		public TBuilder WithEditor<T, TProperty, TEditor>(T instance, Expression<Func<T, TProperty>> propertyExpression)
			where TEditor : IEditor, new()
		{
			return WithEditor(instance, propertyExpression, new TEditor());
		}

		public TBuilder WithEditor<T, TProperty, TEditor>(T instance, Expression<Func<T, TProperty>> propertyExpression, TEditor editor)
			where TEditor : IEditor
		{
			var propertyName = KSoft.Reflection.Util.PropertyNameFromExpr(propertyExpression);
			editor.BoundPropertyDescriptor = BoundPropertyDescriptor.FromProperty(instance, propertyName);
			Inspectors.Add(editor);
			return (TBuilder)this;
		}

		public TBuilder WithObjectProperties(object instance, Func<PropertyDescriptor, bool> propertyFilter)
		{
			var properties = TypeDescriptor.GetProperties(instance)
				.Cast<PropertyDescriptor>()
				.Where(x => x.IsBrowsable && propertyFilter(x))
				.ToList();

			// If any properties are not in the default group, show all properties in collapsible groups.
			if (properties.Any(x => !string.IsNullOrEmpty(x.Category) && x.Category != CategoryAttribute.Default.Category))
			{
				foreach (var category in properties.GroupBy(x => x.Category))
				{
					var actualCategory = (string.IsNullOrEmpty(category.Key) || category.Key == CategoryAttribute.Default.Category)
						? "Miscellaneous"
						: category.Key;

					var collapsibleGroupBuilder = new CollapsibleGroupBuilder();
					AddProperties(instance, category, collapsibleGroupBuilder.Inspectors);
					if (collapsibleGroupBuilder.Inspectors.Any())
						Inspectors.Add(collapsibleGroupBuilder.ToCollapsibleGroup(actualCategory));
				}
			}
			else // Otherwise, show properties in flat list.
			{
				AddProperties(instance, properties, Inspectors);
			}

			return (TBuilder)this;
		}

		public TBuilder WithObjectProperty<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyExpression)
		{
			var propertyName = KSoft.Reflection.Util.PropertyNameFromExpr(propertyExpression);

			return WithObjectProperty(instance, propertyName, typeof(T));
		}

		public TBuilder WithObjectProperty(object instance, string propertyName, Type instanceType = null)
		{
			if (string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException(nameof(propertyName));

			if (instance == null)
				throw new ArgumentNullException(nameof(instance), propertyName);

			if (instanceType == null)
				instanceType = instance.GetType();

			var propDescs = GetPropertyDescriptors(instanceType);
			var propDesc = propDescs.Find(propertyName, ignoreCase: false);

			if (propDesc == null)
				throw new ArgumentException(string.Format(
					"Property '{0}' not found on {1}",
					propertyName, instanceType)
					, nameof(propertyName));

			return WithObjectProperty(instance, propDesc);
		}

		public TBuilder WithObjectProperty(object instance, PropertyDescriptor property)
		{
			var editor = DefaultPropertyInspectors.CreateEditor(property);
			if (editor != null)
			{
				editor.BoundPropertyDescriptor = new BoundPropertyDescriptor(instance, property);
				Inspectors.Add(editor);
			}

			return (TBuilder)this;
		}
	};

	public class InspectablePhxObjectBuilder
		: InspectorBuilder<InspectablePhxObjectBuilder>
	{
		public InspectableObject ToInspectableObject() => new InspectableObject(Inspectors);
	};
}
