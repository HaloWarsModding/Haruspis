using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Microsoft.Win32;
using EditorFileType = Gemini.Framework.Services.EditorFileType;

namespace PhxStudio
{
	static class PhxStudioUtils
	{
		public static List<T> SortAndReturn<T>(this List<T> list, Comparison<T> comparison)
		{
			list.Sort(comparison);
			return list;
		}

		#region PropertyChangedBase
		public static bool SetFieldVal<T>(this PropertyChangedBase obj
			, ref T field, T value
			, bool overrideChecks = false
			, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
			where T : struct, IEquatable<T>
		{
			if (obj == null)
				return false;

			if (!overrideChecks)
				if (field.Equals(value))
					return false;

			field = value;

			if (obj.IsNotifying)
				obj.NotifyOfPropertyChange(propertyName);

			return true;
		}

		public static bool SetFieldEnum<TEnum>(this PropertyChangedBase obj
			, ref TEnum field, TEnum value
			, bool overrideChecks = false
			, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
			where TEnum : struct, IComparable, IFormattable, IConvertible
		{
			if (obj == null)
				return false;

			if (!overrideChecks)
				if (field.ToInt64(null) == value.ToInt64(null))
					return false;

			field = value;

			if (obj.IsNotifying)
				obj.NotifyOfPropertyChange(propertyName);

			return true;
		}

		public static bool SetFieldObj<T>(this PropertyChangedBase obj
			, ref T field, T value
			, bool overrideChecks = false
			, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
			where T : class, IEquatable<T>
		{
			if (obj == null)
				return false;

			if (!overrideChecks)
			{
				if (field == null)
				{
					if (value == null)
						return false;
				}
				else if (field.Equals(value))
					return false;
			}

			field = value;

			if (obj.IsNotifying)
				obj.NotifyOfPropertyChange(propertyName);

			return true;
		}

		public static bool SetFieldRef<T>(this PropertyChangedBase obj
			, ref T field, T value
			, bool overrideChecks = false
			, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
			where T : class
		{
			if (obj == null)
				return false;

			if (!overrideChecks)
				if (object.ReferenceEquals(field, value))
					return false;

			field = value;

			if (obj.IsNotifying)
				obj.NotifyOfPropertyChange(propertyName);

			return true;
		}

		public static bool SetField<T>(this PropertyChangedBase obj
			, ref T field, T value
			, bool overrideChecks = false
			, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			if (obj == null)
				return false;

			if (!overrideChecks)
				if (EqualityComparer<T>.Default.Equals(field, value))
					return false;

			field = value;

			if (obj.IsNotifying)
				obj.NotifyOfPropertyChange(propertyName);

			return true;
		}

		public static bool SetPropertyChanged(this PropertyChangedBase obj
			, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			if (obj == null)
				return false;

			if (obj.IsNotifying)
				obj.NotifyOfPropertyChange(propertyName);

			return true;
		}
		#endregion

		public static void SetupViaEditorFileType(this FileDialog dialog, EditorFileType fileType)
		{
			dialog.AddExtension = true;
			dialog.Filter = string.Format("{0} | *{1}",
				fileType.Name, fileType.FileExtension);
			dialog.DefaultExt = fileType.FileExtension;
		}
	};
}
