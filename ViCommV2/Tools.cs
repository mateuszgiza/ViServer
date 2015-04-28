using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ViCommV2
{
	public static class Extensions
	{
		public static T ParseEnum<T>(string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static Nullable<float> ToFloat(this string s)
		{
			Nullable<float> temp = null;
			if (s != null && s != String.Empty) {
				temp = float.Parse(s);
			}

			return temp;
		}

		public static Nullable<bool> ToBoolean(this string s)
		{
			Nullable<bool> temp = null;
			if (s != null && s != String.Empty) {
				temp = bool.Parse(s);
			}

			return temp;
		}

		public static void CenterText(this RichTextBox rtb)
		{
			var text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
			text.ApplyPropertyValue(Inline.BaselineAlignmentProperty, System.Windows.BaselineAlignment.Center);
		}

		public static void ColorizeName(this Paragraph p, int length, string color)
		{
			TextPointer start = p.ContentStart.GetPositionAtOffset(0);
			TextPointer end = p.ContentStart.GetPositionAtOffset(length + 1);
			TextRange name = new TextRange(start, end);

			name.ApplyPropertyValue(TextElement.ForegroundProperty, BrushExtension.FromARGB(color));
		}

		public static void DetectEmoticonsAndURL(this Paragraph p)
		{
			Run full = p.Inlines.FirstInline as Run;
			string text = new TextRange(p.ContentStart, p.ContentEnd).Text;
			int addedChars = 0;

			foreach (string word in text.Split(new char[] { ' ', '\n', '\r' }).ToList()) {
				if (Emoticons.IsEmoticon(word.ToLower())) {
					string[] emoticons = Emoticons.FindAll(word);

					int lastPos = 0;
					foreach (string emoticon in emoticons) {
						int lastEmoticon = word.IndexOf(emoticon, lastPos);
						string textBefore = word.Substring(lastPos, lastEmoticon - lastPos);

						Run before = new Run(textBefore);

						p.Inlines.Add(before);
						p.Inlines.Add(Emoticons.GetEmoticonFromString(emoticon.ToLower()));

						lastPos = lastEmoticon + emoticon.Length;
					}

					if (word.Length - lastPos > 0) {
						string textAfter = word.Substring(lastPos, word.Length - lastPos);
						p.Inlines.Add(new Run(textAfter));
					}
				}
				else if (HyperlinkManager.IsHyperlink(word)) {
					Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);

					if (!uri.IsAbsoluteUri) {
						uri = new Uri(@"http://" + word, UriKind.Absolute);
					}

					if (uri != null) {
						Run url = new Run(word) { Foreground = BrushExtension.FromARGB("#FF0066CC") };
						Hyperlink link = new Hyperlink(url) {
							NavigateUri = uri,
						};
						link.Click += HyperlinkManager.link_Click;

						p.Inlines.Add(link);
					}
				}
				else {
					p.Inlines.Add(word);
					addedChars += word.Length;
				}

				if (addedChars < text.Length) {
					p.Inlines.Add(" ");
					addedChars += 1;
				}
			}

			p.Inlines.Remove(full);
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

	public class Sound
	{
		private static Dictionary<SoundType, MediaPlayer> Sounds = new Dictionary<SoundType, MediaPlayer>();
		private MediaPlayer s;

		public static void AddSound(SoundType key, Uri path)
		{
			if (Sounds.ContainsKey(key) == false) {
				MediaPlayer p = new MediaPlayer();
				p.Open(path);
				Sounds.Add(key, p);
			}
		}

		public void Play(SoundType type)
		{
			s = Sounds[type];

			s.Stop();
			s.Play();
		}

		public enum SoundType
		{
			Available,
			Message
		}
	}
}