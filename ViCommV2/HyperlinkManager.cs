using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace ViCommV2
{
	public static class HyperlinkManager
	{
		private static readonly Regex UrlRegex = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");

		public static void DetectURL(this Paragraph p)
		{
			string text = new TextRange(p.ContentStart, p.ContentEnd).Text;
			int lastPos = 0;
			int insertedUrlChars = 0;

			foreach (string word in text.Split(new char[] { ' ', '\n', '\r' }).ToList()) {
				TextPointer position = p.ContentStart;

				if (IsHyperlink(word)) {
					Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);

					if (!uri.IsAbsoluteUri) {
						uri = new Uri(@"http://" + word, UriKind.Absolute);
					}

					if (uri != null) {
						int index = text.IndexOf(word, lastPos) + insertedUrlChars + 1;
						int indexEnd = index + word.Length;

						TextPointer point = position.GetPositionAtOffset(index);
						TextPointer endpoint = position.GetPositionAtOffset(indexEnd);

						TextRange range = new TextRange(point, endpoint);

						Hyperlink link = new Hyperlink(point, endpoint) {
							NavigateUri = uri,
						};
						link.Click += link_Click;

						range.ApplyPropertyValue(TextElement.ForegroundProperty, BrushExtension.FromARGB("#FF0066CC"));

						lastPos = index;
						insertedUrlChars += 6;
					}
				}
			}
		}

		public static void link_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
		}

		public static bool IsHyperlink(string word)
		{
			// First check to make sure the word has at least one of the characters we need to make a hyperlink
			if (word.IndexOfAny(@":.\/".ToCharArray()) != -1) {
				if (UrlRegex.IsMatch(word)) {
					Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);

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