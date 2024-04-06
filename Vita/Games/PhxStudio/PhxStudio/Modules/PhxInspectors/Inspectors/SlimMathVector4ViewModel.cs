using System.Runtime.CompilerServices;
using Vector4f = System.Numerics.Vector4;

namespace PhxStudio.Modules.PhxInspectors.Inspectors
{
	class SlimMathVector4ViewModel
		: EditorBase<Vector4f>
		, ILabelledInspector
	{
		public float X
		{
			get { return Value.X; }
			set
			{
				if (value != Value.X)
				{
					Value = new Vector4f(value, Value.Y, Value.Z, Value.W);
					NotifyOfPropertyChange();
				}
			}
		}

		public float Y
		{
			get { return Value.Y; }
			set
			{
				if (value != Value.Y)
				{
					Value = new Vector4f(Value.X, value, Value.Z, Value.W);
					NotifyOfPropertyChange();
				}
			}
		}

		public float Z
		{
			get { return Value.Z; }
			set
			{
				if (value != Value.Z)
				{
					Value = new Vector4f(Value.X, Value.Y, value, Value.W);
					NotifyOfPropertyChange();
				}
			}
		}

		public float W
		{
			get { return Value.W; }
			set
			{
				if (value != Value.W)
				{
					Value = new Vector4f(Value.X, Value.Y, Value.Z, value);
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
				NotifyOfPropertyChange(nameof(W));
			}
			base.NotifyOfPropertyChange(propertyName);
		}
	};
}
