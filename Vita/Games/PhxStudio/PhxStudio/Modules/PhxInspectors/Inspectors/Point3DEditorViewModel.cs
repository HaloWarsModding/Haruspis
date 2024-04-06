using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	public sealed class Point3DEditorViewModel
		: EditorBase<Point3D>
		, ILabelledInspector
	{
		public double X
		{
			get { return Value.X; }
			set
			{
				if (value != Value.X)
				{
					Value = new Point3D(value, Value.Y, Value.Z);
					NotifyOfPropertyChange();
				}
			}
		}

		public double Y
		{
			get { return Value.Y; }
			set
			{
				if (value != Value.Y)
				{
					Value = new Point3D(Value.X, value, Value.Z);
					NotifyOfPropertyChange();
				}
			}
		}

		public double Z
		{
			get { return Value.Z; }
			set
			{
				if (value != Value.Z)
				{
					Value = new Point3D(Value.X, Value.Y, value);
					NotifyOfPropertyChange();
				}
			}
		}

		public override void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
		{
			if (propertyName == nameof(base.Value))
			{
				NotifyOfPropertyChange(nameof(X));
				NotifyOfPropertyChange(nameof(Y));
				NotifyOfPropertyChange(nameof(Z));
			}
			base.NotifyOfPropertyChange(propertyName);
		}
	};
}
