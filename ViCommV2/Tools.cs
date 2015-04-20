using System;
using System.Globalization;
using System.Windows.Media;

namespace ViCommV2
{
	public static class Extensions
	{
		public static T ParseEnum<T>(string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static float ToFloat(this string s)
		{
			return float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
		}
	}

	public static class BrushExtension
	{
		public static Color GetColor(this Brush brush)
		{
			Color c = (Color)brush.GetValue(SolidColorBrush.ColorProperty);
			return c;
		}

		public static SolidColorBrush FromARGB(string argb)
		{
			if (argb.Length != 9)
				return null;

			SolidColorBrush brush = (System.Windows.Media.SolidColorBrush)
				new System.Windows.Media.BrushConverter().ConvertFrom(argb);

			return brush;
		}

		public static Brush Brightness(this Brush brush, double value)
		{
			Color color = brush.GetColor();
			Color newColor = Color.FromArgb(color.A, (byte)(color.R * value), (byte)(color.G * value), (byte)(color.B * value));
			return new SolidColorBrush(newColor);
		}

		public static string ToARGB(this SolidColorBrush brush)
		{
			if (brush == null)
				return string.Empty;

			var c = brush.Color;
			return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
		}

		public static SolidColorBrush FromARGB(this SolidColorBrush brush, string argb)
		{
			if (argb.Length != 9)
				return null;

			brush = (System.Windows.Media.SolidColorBrush)new System.Windows.Media.BrushConverter().ConvertFrom(argb);

			return brush;
		}
	}
}
