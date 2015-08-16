using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using ViCommV2.Interfaces;

namespace ViCommV2.Classes
{
    public class Emoticons
    {
        private static readonly string _filePath = @"Resources\Images\Emoticons\default.xml";
        private static readonly Dictionary<string, Uri> _emoticons = new Dictionary<string, Uri>();
        private static int _lastColumn = 5;
        private static int _lastRow = -1;
        private static string _emoticonRegex = @"";
        public static Regex EmoticonRegex = new Regex(@"");

        public static void Add(string name, string path)
        {
            if (_emoticons.ContainsKey(name.ToLower())) {
                return;
            }
            _emoticons.Add(name.ToLower(), new Uri(path, UriKind.Relative));
            AddtoRegex(name.ToLower());

            var main = FormHelper.Instance.Main;
            InsertToGrid(main, main.grid_emoticons, name);
        }

        private static void InsertToGrid(IChatWindow window, Grid grid, string name)
        {
            if (_lastColumn > 4) {
                _lastColumn = 0;
                _lastRow++;

                grid.RowDefinitions.Add(new RowDefinition {
                    Height = (GridLength) new GridLengthConverter().ConvertFromString("25")
                });
                grid.Height = (_lastRow + 1)*25;
            }

            var inputBox = window.GetInputTextBox();

            var item = GetEmoticonFromString(name);
            item.MouseLeftButtonUp += (sender, e) => {
                inputBox.AppendText(name);
                window.GetEmoticonsContainer().Visibility = Visibility.Hidden;
                inputBox.CaretIndex = inputBox.Text.Length;
                inputBox.Focus();
            };
            item.Style = (Style) window.FindResource("emoticonHover");
            item.ToolTip = name;

            Grid.SetColumn(item, _lastColumn);
            Grid.SetRow(item, _lastRow);
            _lastColumn++;

            grid.Children.Add(item);
        }

        public static void RefreshCollection(IChatWindow window, Grid grid)
        {
            _lastColumn = 5;
            _lastRow = -1;
            grid.Children.Clear();

            foreach (var name in _emoticons.Keys) {
                InsertToGrid(window, grid, name);
            }
        }

        public static Uri GetPath(string name)
        {
            return _emoticons.ContainsKey(name.ToLower()) ? _emoticons[name.ToLower()] : null;
        }

        private static void AddtoRegex(string name)
        {
            if (_emoticonRegex.Length > 0) {
                _emoticonRegex += @"|";
            }

            _emoticonRegex += $"({Regex.Escape(name.ToLower())})";
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

            var start = word.ToLower().IndexOf(match, StringComparison.Ordinal);

            return word.Substring(start, match.Length);
        }

        public static string[] FindAll(string word)
        {
            var matches = EmoticonRegex.Matches(word.ToLower()).OfType<Match>().Select(m => m.Value).ToArray();
            var wordMatches = new List<string>();
            var lastPos = 0;

            foreach (var match in matches) {
                var start = word.ToLower().IndexOf(match, lastPos, StringComparison.Ordinal);
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
            using (var reader = XmlReader.Create(_filePath)) {
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