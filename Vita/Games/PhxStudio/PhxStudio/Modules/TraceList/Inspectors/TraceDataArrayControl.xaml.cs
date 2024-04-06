using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using KSoft;

namespace PhxStudio.Modules.TraceList.Inspectors
{
	/// <summary>
	/// Interaction logic for TraceDataArrayControl.xaml
	/// </summary>
	public partial class TraceDataArrayControl : UserControl
	{
		public sealed class DataElementModel
		{
			public DataElementModel Parent { get; set; }
			public string Level { get; set; }
			public string Value { get; set; }

			public static void Populate(ObservableCollection<DataElementModel> collection, object[] source)
			{
				for (int x = 0; x < source.Length; x++)
				{
					var src = source[x];
					var model = new DataElementModel
					{
						Parent = null,
						Level = x.ToString(),
					};
					collection.Add(model);

					if (src is Exception)
					{
						DecomposeException(collection, 0, model, model, (Exception)src);
					}
					else
					{
						model.Value = src.ToString();
					}
				}
			}

			private static void DecomposeException(ObservableCollection<DataElementModel> collection, int depth
				, DataElementModel root, DataElementModel model, Exception e)
			{
				model.Value = e.Message;

				var callstack_model = new DataElementModel
				{
					Parent = model,
					Level = model.Level + ".Stack",
					Value = GetStackTrace(e),
				};
				collection.Add(callstack_model);

				if (e.InnerException != null)
				{
					depth++;
					var inner_model = new DataElementModel
					{
						Parent = model,
						Level = string.Format("{0}.{1}", root.Level, depth),
					};
					collection.Add(inner_model);

					DecomposeException(collection, depth, root, inner_model, e.InnerException);
				}
			}

			private static string GetStackTrace(Exception e)
			{
				var trace = new StackTrace(e, fNeedFileInfo: true);
				if (trace.FrameCount == 0)
					return string.Empty;

				var sb = new System.Text.StringBuilder(512);
				for (int x = 0; x < trace.FrameCount; x++)
				{
					if (x > 0)
						sb.Append("\n");

					var frame = trace.GetFrame(x);

					var mb = frame.GetMethod();
					if (mb == null)
						continue;

					Type classType = mb.DeclaringType;
					if (classType == null)
						continue;

					// Add namespace.classname:MethodName
					string ns = classType.Namespace;
					if (!string.IsNullOrEmpty(ns))
					{
						sb.Append(ns);
						sb.Append(".");
					}

					sb.Append(classType.Name);
					sb.Append(":");
					sb.Append(mb.Name);
					sb.Append("(");

					bool firstParam = true;
					foreach (var param in mb.GetParameters())
					{
						if (firstParam)
							firstParam = false;
						else
							sb.Append(", ");

						sb.Append(param.ParameterType.Name);
					}

					sb.Append(")");

					string path = frame.GetFileName();
					if (path.IsNotNullOrEmpty())
					{
						// Unify path names to unix style
						//path = path.Replace('\\', '/');

						const string kBasePath = @"KStudio\Vita\";
						int base_path_index = path.IndexOf(kBasePath);
						if (base_path_index >= 0)
						{
							path = path.Substring(base_path_index + kBasePath.Length);
						}

						sb.Append(" (");
						sb.Append(path);

						int lineNum = frame.GetFileLineNumber();
						if (lineNum > 0)
						{
							sb.Append(":");
							sb.Append(lineNum);
						}
						sb.Append(")");
					}
				}

				return sb.ToString();
			}
		};

		#region DataElements
		public ObservableCollection<DataElementModel> DataElements
		{
			get { return (ObservableCollection<DataElementModel>)GetValue(DataElementsProperty); }
			set { SetValue(DataElementsProperty, value); }
		}
		public static readonly DependencyProperty DataElementsProperty = DependencyProperty.Register(
			nameof(DataElements), typeof(ObservableCollection<DataElementModel>), typeof(TraceDataArrayControl),
			new PropertyMetadata(new ObservableCollection<DataElementModel>()));
		#endregion

		#region DataArray
		public object[] DataArray
		{
			get { return (object[])GetValue(DataArrayProperty); }
			set { SetValue(DataArrayProperty, value); }
		}
		public static readonly DependencyProperty DataArrayProperty = DependencyProperty.Register(
			nameof(DataArray), typeof(object[]), typeof(TraceDataArrayControl),
			new PropertyMetadata(null, new PropertyChangedCallback(OnDataArrayPropertyChanged)));
		#endregion

		public TraceDataArrayControl()
		{
			InitializeComponent();
		}

		private static void OnDataArrayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (TraceDataArrayControl)d;
			var source = (object[])e.NewValue;

			ctrl.DataElements.Clear();
			if (!source.IsNullOrEmpty())
			{
				DataElementModel.Populate(ctrl.DataElements, source);
			}
		}
	};
}
