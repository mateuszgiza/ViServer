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
using ViCommV2.Interfaces;

// StickyWindow

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class SingleChatWindow : IChatWindow
    {
        private double _timeElapsed;
        private bool _tStarted;
        private DispatcherTimer _tWriting;

        private readonly string _name;

        public SingleChatWindow(string name)
        {
            InitializeComponent();

            _name = name;

            DataContext = SettingsProvider.Instance.Settings;

            WindowStickManager.AddWindow(this);
        }

        private void Initialize()
        {
            Sound.AddSound(Sound.SoundType.Available, new Uri(@"Resources\Sounds\mp3_available.mp3", UriKind.Relative));
            Sound.AddSound(Sound.SoundType.Message, new Uri(@"Resources\Sounds\mp3_message.mp3", UriKind.Relative));

            Emoticons.ReadXml();
            Emoticons.RefreshCollection(this, grid_emoticons);
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
            Client.Send(s, PacketType.SingleChat, _name);
        }

        public void AppendText(Packet packet)
        {
                var sound = new Sound();

                if (packet.Type == PacketType.Information) {
                    sound.Play(Sound.SoundType.Available);

                    var msg = packet.Information.User + " " + packet.Information.Message;
                    window.AppendInfoText($"* {msg}");
                }
                else {
                    RowType rowType;

                    if (packet.Sender != Client.User.Nickname) {
                        rowType = RowType.Sender;
                        sound.Play(Sound.SoundType.Message);
                    }
                    else {
                        rowType = RowType.User;
                    }

                    window.AppendText(packet, rowType);
                }
        }

        private void AppendText(Packet packet, RowType rowType)
        {
            chatBox.Document.Blocks.Add(MessageManager.Message(packet, rowType, chatBox));
            chatBox.CenterTextVertically();
            chatBox.ScrollToEnd();
        }

        private void AppendInfoText(string text)
        {
            var p = new Paragraph {Background = Brushes.Transparent};

            p.Inlines.Add(new Run(text));

            p.DetectUrl();

            chatBox.Document.Blocks.Add(p);
            chatBox.ScrollToEnd();
        }
        
        public void CloseWindow()
        {
            State = FormState.Close;
            Close();
        }

        #region Properties

        public ClientManager Client { get; set; }

        public FormHelper Forms { get; set; }

        public SettingsWindow Settings { get; set; }

        public FormState State { get; set; }

        public Brush BackgroundColor { get; set; }
        
        public TextBox GetInputTextBox()
        {
            return inputBox;
        }

        public ScrollViewer GetEmoticonsContainer()
        {
            return emoticonsContainer;
        }

        #endregion

        #region Context Menus

        private void CM_Clear_Click(object sender, RoutedEventArgs e)
        {
            chatBox.Document.Blocks.Clear();
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
        
        private void CM_Close_Click(object sender, RoutedEventArgs e)
        {
            State = FormState.Close;
            Close();
        }

        private void CM_Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CM_Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        #endregion

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();

            Forms = FormHelper.Instance;
            Connect();

            // Display user nickname
            Title = "Private Chat: " + _name;
            BackgroundColor = Brushes.CadetBlue;

            State = FormState.Null;
            inputBox.Focus();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            var notify = FormHelper.Instance.Notify;

            notify?.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (State != FormState.Close) return;

            Forms.SingleChat.Remove(_name);
        }
        
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) {
                return;
            }
            e.Handled = true;
            DragMove();
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

        private void inputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _timeElapsed = 0;

            if (_tStarted) {
                return;
            }

            _tStarted = true;
            _tWriting = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 100) };
            _tWriting.Tick += t_writing_Tick;
            _tWriting.Start();

            var p = new Packet(PacketType.Information) {
                Receiver = _name,
                Information = new Information(InformationType.Writing, Client.User.Nickname, "started")
            };
            Client.Send(p);
        }

        private void t_writing_Tick(object sender, EventArgs e)
        {
            if (_timeElapsed >= 1000) {
                _tWriting.Tick -= t_writing_Tick;
                _tWriting.Stop();
                _tWriting = null;
                _tStarted = false;

                var p = new Packet(PacketType.Information) {
                    Receiver = _name,
                    Information = new Information(InformationType.Writing, Client.User.Nickname, "stopped")
                };
                Client.Send(p);
            }
            else {
                _timeElapsed += 100;
            }
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
        
        private void emoticonsContainer_LostFocus(object sender, RoutedEventArgs e)
        {
            emoticonsContainer.Visibility = Visibility.Hidden;
        }

        public static void SingleChatReceived(Packet p)
        {
            var forms = FormHelper.Instance;
            if (!forms.SingleChat.ContainsKey(p.Sender)) {
                forms.SingleChat.Add(p.Sender, new SingleChatWindow(p.Sender));
            }

            forms.SingleChat[p.Sender].Show();

            forms.SingleChat[p.Sender].AppendText(p);
        }

        #endregion
    }
}