using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ViCommV2.Classes
{
    public static class Extensions
    {
        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof (T), value, true);
        }

        public static float? ToFloat(this string s)
        {
            float? temp = null;
            if (!string.IsNullOrEmpty(s)) {
                temp = float.Parse(s);
            }

            return temp;
        }

        public static bool? ToBoolean(this string s)
        {
            bool? temp = null;
            if (!string.IsNullOrEmpty(s)) {
                temp = bool.Parse(s);
            }

            return temp;
        }

        public static void CenterTextVertically(this RichTextBox rtb)
        {
            var text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            text.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
        }

        public static void ColorizeName(this Paragraph p, int length, string color)
        {
            var start = p.ContentStart.GetPositionAtOffset(0);
            var end = p.ContentStart.GetPositionAtOffset(length + 1);
            var name = new TextRange(start, end);

            name.ApplyPropertyValue(TextElement.ForegroundProperty, BrushExtension.FromARGB(color));
        }

        public static void DetectEmoticonsAndUrl(this Paragraph p)
        {
            var full = p.Inlines.FirstInline as Run;
            var text = new TextRange(p.ContentStart, p.ContentEnd).Text;
            var addedChars = 0;

            foreach (var word in text.Split(' ', '\n', '\r').ToList()) {
                if (Emoticons.IsEmoticon(word.ToLower())) {
                    var emoticons = Emoticons.FindAll(word);

                    var lastPos = 0;
                    foreach (var emoticon in emoticons) {
                        var lastEmoticon = word.IndexOf(emoticon, lastPos, StringComparison.Ordinal);
                        var textBefore = word.Substring(lastPos, lastEmoticon - lastPos);

                        var before = new Run(textBefore);

                        p.Inlines.Add(before);
                        p.Inlines.Add(Emoticons.GetEmoticonFromString(emoticon.ToLower()));

                        lastPos = lastEmoticon + emoticon.Length;
                    }

                    if (word.Length - lastPos > 0) {
                        var textAfter = word.Substring(lastPos, word.Length - lastPos);
                        p.Inlines.Add(new Run(textAfter));
                    }
                }
                else if (HyperlinkManager.IsHyperlink(word)) {
                    var uri = new Uri(word, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri) {
                        uri = new Uri(@"http://" + word, UriKind.Absolute);
                    }

                    if (uri != null) {
                        var url = new Run(word) {Foreground = BrushExtension.FromARGB("#FF0066CC")};
                        var link = new Hyperlink(url) {
                            NavigateUri = uri
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
            var c = (Color) brush.GetValue(SolidColorBrush.ColorProperty);
            return c;
        }

        public static SolidColorBrush FromARGB(string argb)
        {
            if (argb.Length != 9) {
                return null;
            }

            var brush = (SolidColorBrush)
                        new BrushConverter().ConvertFrom(argb);

            return brush;
        }

        public static Brush Brightness(this Brush brush, double value)
        {
            var color = brush.GetColor();
            var newColor = Color.FromArgb(color.A, (byte) (color.R*value), (byte) (color.G*value),
                                          (byte) (color.B*value));
            return new SolidColorBrush(newColor);
        }

        public static string ToARGB(this SolidColorBrush brush)
        {
            if (brush == null) {
                return string.Empty;
            }

            var c = brush.Color;
            return $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        public static SolidColorBrush FromARGB(this SolidColorBrush brush, string argb)
        {
            if (argb.Length != 9) {
                return null;
            }

            brush = (SolidColorBrush) new BrushConverter().ConvertFrom(argb);

            return brush;
        }
    }

    public class Sound
    {
        public enum SoundType
        {
            Available,
            Message
        }

        private static readonly Dictionary<SoundType, MediaPlayer> Sounds = new Dictionary<SoundType, MediaPlayer>();
        private MediaPlayer _player;

        public static void AddSound(SoundType key, Uri path)
        {
            if (Sounds.ContainsKey(key)) {
                return;
            }

            var p = new MediaPlayer();
            p.Open(path);
            Sounds.Add(key, p);
        }

        public void Play(SoundType type)
        {
            _player = Sounds[type];

            _player.Stop();
            _player.Play();
        }
    }
}