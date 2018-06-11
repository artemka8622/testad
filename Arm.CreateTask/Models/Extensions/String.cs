using System.ComponentModel;

namespace Arm.CreateTask.Models.Extensions
{
	/// <summary>
	/// Расширение строчки. 
	/// </summary>
	public static class StringExtension
	{
		/// <summary>
		/// Преобразует строчку в Nullable объект другого типа.
		/// </summary>
		/// <typeparam name="T">Целевой тип объекта.</typeparam>
		/// <param name="s">Преобразуемая строчка.</param>
		/// <returns>Новый Nullable объект.</returns>
		public static T? ToNullable<T>(this string s) where T : struct
		{
			var result = new T?();
			try
			{
				if (!string.IsNullOrWhiteSpace(s))
				{
					TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
					result = (T)conv.ConvertFrom(s);
				}
			}
			catch { }

			return result;
		}
	}
}