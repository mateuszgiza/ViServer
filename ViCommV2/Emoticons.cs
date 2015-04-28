using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;

namespace ViCommV2
{
	public class Emoticons
	{
		private static string filePath = @"Resources\Images\Emoticons\default.xml";
		private static Dictionary<string, Uri> _emoticons = new Dictionary<string, Uri>();

		public static void Add(string name, string path)
		{
			if (_emoticons.ContainsKey(name.ToLower()) == false) {
				_emoticons.Add(name.ToLower(), new Uri(path, UriKind.Relative));
				AddtoRegex(name.ToLower());

				InsertToGrid(FormHelper.Instance.Main.grid_emoticons, name);
			}
		}

		private static int lastColumn = 5;
		private static int lastRow = -1;

		private static void InsertToGrid(Grid grid, string name)
		{
			if (lastColumn > 4) {
				lastColumn = 0;
				lastRow++;

				grid.RowDefinitions.Add(new RowDefinition() { Height = (GridLength)new GridLengthConverter().ConvertFromString("25") });
				grid.Height = (lastRow + 1) * 25;
			}

			MainWindow Main = FormHelper.Instance.Main;

			Image item = GetEmoticonFromString(name);
			item.MouseLeftButtonUp += (sender, e) => {
				Main.inputBox.AppendText(name);
				Main.emoticonsContainer.Visibility = Visibility.Hidden;
				Main.inputBox.CaretIndex = Main.inputBox.Text.Length;
				Main.inputBox.Focus();
			};
			item.Style = (Style)Main.FindResource("emoticonHover");
			item.ToolTip = name;

			Grid.SetColumn(item, lastColumn);
			Grid.SetRow(item, lastRow);
			lastColumn++;

			grid.Children.Add(item);
		}

		public static void RefreshCollection(Grid grid)
		{
			lastColumn = 5;
			lastRow = -1;
			grid.Children.Clear();

			foreach (string name in _emoticons.Keys) {
				InsertToGrid(grid, name);
			}
		}

		public static Uri GetPath(string name)
		{
			if (_emoticons.ContainsKey(name.ToLower())) {
				return _emoticons[name.ToLower()];
			}

			return null;
		}

		private static string _emoticonRegex = @"";
		public static Regex EmoticonRegex = new Regex(@"");

		private static void AddtoRegex(string name)
		{
			if (_emoticonRegex.Length > 0) {
				_emoticonRegex += @"|";
			}

			_emoticonRegex += String.Format("({0})", Regex.Escape(name.ToLower()));
			EmoticonRegex = new Regex(_emoticonRegex);
		}

		public static Image GetEmoticonFromString(string word)
		{
			//float height = SettingsProvider.Instance.settings.MessageFont.Height / 1.5f;
			Image image = new Image() { Width = 20, Height = 20 };
			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
			image.Source = new BitmapImage(GetPath(word));

			return image;
		}

		/// <summary>
		/// Searches input(or lowered) in Regex and return it's value when found
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		public static string Find(string word)
		{
			string match = EmoticonRegex.Match(word.ToLower()).Value;
			if (match != String.Empty) {
				int start = word.ToLower().IndexOf(match);

				return word.Substring(start, match.Length);
			}

			return String.Empty;
		}

		public static string[] FindAll(string word)
		{
			string[] matches = EmoticonRegex.Matches(word.ToLower()).OfType<Match>().Select(m => m.Value).ToArray();
			List<string> wordMatches = new List<string>();
			int lastPos = 0;

			foreach (string match in matches) {
				int start = word.ToLower().IndexOf(match, lastPos);
				lastPos = start + match.Length;
				wordMatches.Add(word.Substring(start, match.Length));
			}

			return wordMatches.ToArray();
		}

		public static bool IsEmoticon(string word)
		{
			if (EmoticonRegex.IsMatch(word)) {
				return true;
			}

			return false;
		}

		public static void ReadXML()
		{
			string regex;
			string file;

			using (XmlReader reader = XmlReader.Create(filePath)) {
				while (reader.Read()) {
					if (reader.IsStartElement()) {
						switch (reader.Name) {
							case "emot":
								regex = reader.GetAttribute("regex") ?? "";
								file = reader.GetAttribute("file") ?? "";

								Add(regex, @"Resources\Images\Emoticons\" + file);
								break;
						}
					}
				}
			}
		}
	}
}