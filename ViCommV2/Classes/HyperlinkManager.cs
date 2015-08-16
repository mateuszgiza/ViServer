using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace ViCommV2.Classes
{
    public static class HyperlinkManager
    {
        private static readonly Regex UrlRegex =
            new Regex(
                @"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");

        public static void DetectUrl(this Paragraph p)
        {
            var text = new TextRange(p.ContentStart, p.ContentEnd).Text;
            var lastPos = 0;
            var insertedUrlChars = 0;

            foreach (var word in text.Split(' ', '\n', '\r').ToList()) {
                var position = p.ContentStart;

                if (IsHyperlink(word)) {
                    var uri = new Uri(word, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri) {
                        uri = new Uri(@"http://" + word, UriKind.Absolute);
                    }

                    if (uri != null) {
                        var index = text.IndexOf(word, lastPos, StringComparison.Ordinal) + insertedUrlChars + 1;
                        var indexEnd = index + word.Length;

                        var point = position.GetPositionAtOffset(index);
                        var endpoint = position.GetPositionAtOffset(indexEnd);

                        var range = new TextRange(point, endpoint);

                        var link = new Hyperlink(point, endpoint) {
                            NavigateUri = uri
                        };
                        link.Click += link_Click;

                        range.ApplyPropertyValue(TextElement.ForegroundProperty, BrushExtension.FromARGB("#FF0066CC"));

                        lastPos = index;
                        insertedUrlChars += 6;
                    }
                }
            }
        }

        public static void link_Click(object sender, RoutedEventArgs e)
        {
            Process.Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
        }

        public static bool IsHyperlink(string word)
        {
            // First check to make sure the word has at least one of the characters we need to make a hyperlink
            if (word.IndexOfAny(@":.\/".ToCharArray()) != -1) {
                if (UrlRegex.IsMatch(word)) {
                    var uri = new Uri(word, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri) {
                        // rebuild it it with http to turn it into an Absolute URI
                        uri = new Uri(@"http://" + word, UriKind.Absolute);
                    }

                    if (uri.IsAbsoluteUri) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}