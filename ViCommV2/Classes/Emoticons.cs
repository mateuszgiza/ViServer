using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace ViCommV2
{
    public class Emoticons
    {
        private static readonly string filePath = @"Resources\Images\Emoticons\default.xml";
        private static readonly Dictionary<string, Uri> _emoticons = new Dictionary<string, Uri>();
        private static int _lastColumn = 5;
        private static int _lastRow = -1;
        private static string _emoticonRegex = @"";
        public static Regex EmoticonRegex = new Regex(@"");

        public static void Add(string name, string path)
        {
            if (_emoticons.ContainsKey(name.ToLower()) == false) {
                _emoticons.Add(name.ToLower(), new Uri(path, UriKind.Relative));
                AddtoRegex(name.ToLower());

                InsertToGrid(FormHelper.Instance.Main.grid_emoticons, name);
            }
        }

        private static void InsertToGrid(Grid grid, string name)
        {
            if (_lastColumn > 4) {
                _lastColumn = 0;
                _lastRow++;

                grid.RowDefinitions.Add(new RowDefinition {
                    Height = (GridLength) new GridLengthConverter().ConvertFromString("25")
                });
                grid.Height = (_lastRow + 1)*25;
            }

            var Main = FormHelper.Instance.Main;

            var item = GetEmoticonFromString(name);
            item.MouseLeftButtonUp += (sender, e) => {
                Main.inputBox.AppendText(name);
                Main.emoticonsContainer.Visibility = Visibility.Hidden;
                Main.inputBox.CaretIndex = Main.inputBox.Text.Length;
                Main.inputBox.Focus();
            };
            item.Style = (Style) Main.FindResource("emoticonHover");
            item.ToolTip = name;

            Grid.SetColumn(item, _lastColumn);
            Grid.SetRow(item, _lastRow);
            _lastColumn++;

            grid.Children.Add(item);
        }

        public static void RefreshCollection(Grid grid)
        {
            _lastColumn = 5;
            _lastRow = -1;
            grid.Children.Clear();

            foreach (var name in _emoticons.Keys) {
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

        private static void AddtoRegex(string name)
        {
            if (_emoticonRegex.Length > 0) {
                _emoticonRegex += @"|";
            }

            _emoticonRegex += string.Format("({0})", Regex.Escape(name.ToLower()));
            EmoticonRegex = new Regex(_emoticonRegex);
        }

        public static Image GetEmoticonFromString(string word)
        {
            //float height = SettingsProvider.Instance.settings.MessageFont.Height / 1.5f;
            var image = new Image {Width = 20, Height = 20};
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            image.Source = new BitmapImage(GetPath(word));
            image.ToolTip = word;

            return image;
        }

        /// <summary>
        ///     Searches input(or lowered) in Regex and return it's value when found
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string Find(string word)
        {
            var match = EmoticonRegex.Match(word.ToLower()).Value;
            if (match == string.Empty) {
                return string.Empty;
            }

            var start = word.ToLower().IndexOf(match);

            return word.Substring(start, match.Length);
        }

        public static string[] FindAll(string word)
        {
            var matches = EmoticonRegex.Matches(word.ToLower()).OfType<Match>().Select(m => m.Value).ToArray();
            var wordMatches = new List<string>();
            var lastPos = 0;

            foreach (var match in matches) {
                var start = word.ToLower().IndexOf(match, lastPos);
                lastPos = start + match.Length;
                wordMatches.Add(word.Substring(start, match.Length));
            }

            return wordMatches.ToArray();
        }

        public static bool IsEmoticon(string word)
        {
            return EmoticonRegex.IsMatch(word);
        }

        public static void ReadXml()
        {
            using (var reader = XmlReader.Create(filePath)) {
                while (reader.Read()) {
                    if (!reader.IsStartElement()) {
                        continue;
                    }

                    switch (reader.Name) {
                        case "emot":
                            var regex = reader.GetAttribute("regex") ?? "";
                            var file = reader.GetAttribute("file") ?? "";

                            Add(regex, @"Resources\Images\Emoticons\" + file);
                            break;
                    }
                }
            }
        }
    }
}