using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ViCommV2.Classes;
using ViData;

// StickyWindow

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class MainWindow : Window
    {
        public enum FormState
        {
            Null,
            Exit,
            Close,
            Logout
        }

        public enum RowType
        {
            Information,
            User,
            Sender
        }

        private readonly bool _enterPressed = false;
        private double _timeElapsed;
        private bool _tStarted;
        private DispatcherTimer _tWriting;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = SettingsProvider.Instance.settings;

            var stick = new WindowStickManager(this);
        }

        private void Initialize()
        {
            Sound.AddSound(Sound.SoundType.Available, new Uri(@"Resources\Sounds\mp3_available.mp3", UriKind.Relative));
            Sound.AddSound(Sound.SoundType.Message, new Uri(@"Resources\Sounds\mp3_message.mp3", UriKind.Relative));

            Emoticons.ReadXml();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
            Emoticons.RefreshCollection(grid_emoticons);

            Forms = FormHelper.Instance;
            Connect();

            // Display user nickname
            Title = "ViComm | Nick: " + Client.user.Nickname;
            lb_Header.Content = "ViComm | Nick: " + Client.user.Nickname;
            BackgroundColor = Brushes.CadetBlue;

            State = FormState.Exit;
            inputBox.Focus();
        }

        private void CM_Clear_Click(object sender, RoutedEventArgs e)
        {
            chatBox.Document.Blocks.Clear();
        }

        private void inputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.Enter)) {
                inputBox.Text += "\r\n";
                inputBox.SelectionStart = inputBox.Text.Length;

                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.Enter)) {
                if (inputBox.Text.Length > 0) {
                    Send(inputBox.Text);
                    inputBox.Clear();
                }

                e.Handled = true;
            }
        }

        public void Connect()
        {
            if (Client != null) {
                return;
            }

            Client = ClientManager.Instance;
            Client.Connect();
        }

        private void Send(string s)
        {
            Client.Send(s);
        }

        private Table Message(Packet packet, RowType rowType)
        {
            var date = new Run(string.Format("{0:HH:mm:ss}", packet.Date.ToLocalTime()));
            date.SetBinding(TextElement.FontFamilyProperty, new Binding("DateFont.Name"));
            date.SetBinding(TextElement.FontSizeProperty, new Binding("DateFont.Size"));
            date.SetBinding(TextElement.ForegroundProperty, new Binding("DateForeground"));

            var text = new Run(packet.Sender + ": " + packet.Message);

            var bitmap = new BitmapImage(new Uri(packet.User.AvatarURI, UriKind.Absolute) ??
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
            txt.DetectEmoticonsAndURL();
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

                    if (ApplicationIsActivated() == false) {
                        Forms.Notify.ShowMessage(bitmap, text.Text);
                        Forms.Notify.Show();
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

        public void AppendText(Packet packet, RowType rowType)
        {
            chatBox.Document.Blocks.Add(Message(packet, rowType));
            chatBox.CenterText();
            chatBox.ScrollToEnd();
        }

        public void AppendInfoText(string text)
        {
            var p = new Paragraph {Background = Brushes.Transparent};

            p.Inlines.Add(new Run(text));

            p.DetectURL();

            chatBox.Document.Blocks.Add(p);
            chatBox.ScrollToEnd();
        }

        private void CM_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (Settings == null) {
                Settings = new SettingsWindow();
                Settings.Closed += (s, ev) => { Settings = null; };

                Settings.Show();
            }

            Settings.Focus();
        }

        private void CM_Logout_Click(object sender, RoutedEventArgs e)
        {
            State = FormState.Logout;

            Forms.Dispatcher.Invoke(new Action(() => {
                Forms.Login = new LoginWindow();
                Forms.Login.Show();
            }));

            Close();
        }

        private void CM_Exit_Click(object sender, RoutedEventArgs e)
        {
            State = FormState.Exit;
            Close();
        }

        private void CM_Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CM_Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
            }
            else {
                WindowState = WindowState.Maximized;
            }
        }

        public void CloseWindow()
        {
            State = FormState.Close;
            Close();
        }

        public void Exit()
        {
            Logout();

            State = FormState.Null;
            Environment.Exit(0);
        }

        private void Logout()
        {
            if (Client != null) {
                Client.Disconnect(false);
            }
            FormHelper.isClosing = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            switch (State) {
                case FormState.Logout:
                    Logout();
                    break;
                case FormState.Close:
                    break;
                case FormState.Exit:
                    Exit();
                    break;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero) {
                return false; // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }

        private void inputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_enterPressed) {
                return;
            }

            _timeElapsed = 0;

            if (_tStarted) {
                return;
            }

            _tStarted = true;
            _tWriting = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 100)};
            _tWriting.Tick += t_writing_Tick;
            _tWriting.Start();

            var p = new Packet(PacketType.Information);
            p.Information = new Information(InformationType.Writing, Client.user.Nickname, "started");
            Client.Send(p);
        }

        private void t_writing_Tick(object sender, EventArgs e)
        {
            if ((_timeElapsed >= 1000) || _enterPressed) {
                _tWriting.Tick -= t_writing_Tick;
                _tWriting.Stop();
                _tWriting = null;
                _tStarted = false;

                var p = new Packet(PacketType.Information);
                p.Information = new Information(InformationType.Writing, Client.user.Nickname, "stopped");
                Client.Send(p);
            }
            else {
                _timeElapsed += 100;
            }
        }

        private void list_Contacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var l = sender as ListBox;
            l.UpdateLayout();
        }

        private void bt_menu_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) {
                return;
            }

            button.ContextMenu.IsEnabled = true;
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.Placement = PlacementMode.Bottom;
            button.ContextMenu.IsOpen = true;
        }

        private void bt_emoticons_Click(object sender, RoutedEventArgs e)
        {
            emoticonsContainer.Visibility = Visibility.Visible;
            emoticonsContainer.Focus();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) {
                return;
            }
            e.Handled = true;
            DragMove();
        }

        private void emoticonsContainer_LostFocus(object sender, RoutedEventArgs e)
        {
            emoticonsContainer.Visibility = Visibility.Hidden;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            var Notify = FormHelper.Instance.Notify;

            if (Notify != null) {
                Notify.Close();
            }
        }

        #region Properties

        public ClientManager Client { get; set; }

        public FormHelper Forms { get; set; }

        public SettingsWindow Settings { get; set; }

        public FormState State { get; set; }

        public Brush BackgroundColor { get; set; }

        public ListBox ListContacts {
            get { return list_Contacts; }
        }

        #endregion Fields
    }
}