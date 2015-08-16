using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ViData;

namespace ViCommV2.Classes
{
    public class MessageManager {
        public static Table Message(Packet packet, RowType rowType, RichTextBox chatBox)
        {
            var date = new Run($"{packet.Date.ToLocalTime():HH:mm:ss}");
            date.SetBinding(TextElement.FontFamilyProperty, new Binding("DateFont.Name"));
            date.SetBinding(TextElement.FontSizeProperty, new Binding("DateFont.Size"));
            date.SetBinding(TextElement.ForegroundProperty, new Binding("DateForeground"));

            var text = new Run(packet.Sender + ": " + packet.Message);

            var bitmap = new BitmapImage(new Uri(packet.User.AvatarUri, UriKind.Absolute) ??
                                         new Uri(@"Resources\Images\avatar.png", UriKind.Relative));

            var rect = new Rectangle {
                RadiusX = 10,
                RadiusY = 10,
                Width = 24,
                Height = 24,
                Fill = new ImageBrush(bitmap)
            };
            RenderOptions.SetBitmapScalingMode(rect, BitmapScalingMode.HighQuality);
            var container = new InlineUIContainer(rect);
            var avatar = new Paragraph(container);

            var txt = new Paragraph(text) {
                Margin = new Thickness(1)
            };
            txt.DetectEmoticonsAndUrl();
            txt.ColorizeName(packet.Sender.Length, packet.User.NickColor);
            txt.SetBinding(TextElement.ForegroundProperty, new Binding("MessageForeground"));

            var dT = new Paragraph(date) {
                Margin = new Thickness(0, 0, 0, 1)
            };

            var tab = new Table {
                Margin = new Thickness(0, 2, 0, 2)
            };

            var gridLenghtConverter = new GridLengthConverter();

            //1. col
            tab.Columns.Add(new TableColumn {
                Name = "colAvatar",
                Width = (GridLength) gridLenghtConverter.ConvertFromString("28")
            });
            //2.col
            tab.Columns.Add(new TableColumn {
                Name = "colMsg",
                Width = (GridLength) gridLenghtConverter.ConvertFromString("Auto")
            });
            //3.col
            tab.Columns.Add(new TableColumn {
                Name = "colDt",
                Width = (GridLength) gridLenghtConverter.ConvertFromString("50")
            });

            tab.RowGroups.Add(new TableRowGroup());
            tab.RowGroups[0].Rows.Add(new TableRow());

            var tabRow = tab.RowGroups[0].Rows[0];

            switch (rowType) {
                case RowType.User:
                    tabRow.Style = (Style) chatBox.Resources["RowUser"];
                    break;
                case RowType.Sender:
                    tabRow.Style = (Style) chatBox.Resources["RowSender"];

                    if (AppHandle.ApplicationIsActivated() == false) {
                        var notifyWindow = FormHelper.Instance.Notify;
                        notifyWindow.ShowMessage(bitmap, text.Text);
                        notifyWindow.Show();
                    }
                    break;
            }

            //1.col - NICK
            tabRow.Cells.Add(new TableCell(avatar));

            //2.col - MESSAGE
            tabRow.Cells.Add(new TableCell(txt));

            //3.col  - DATE TIME
            tabRow.Cells.Add(new TableCell(dT));

            return tab;
        }
    }
}