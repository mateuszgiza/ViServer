using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ViCommV2.Classes;
using ViCommV2.Interfaces;
using ViData;

// StickyWindow

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class MainWindow : IChatWindow
    {
        private double _timeElapsed;
        private bool _tStarted;
        private DispatcherTimer _tWriting;

        public MainWindow()
        {
            InitializeComponent();

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
            
            Forms = FormHelper.Instance;
            Connect();

            // Display user nickname
            Title = "ViComm | Nick: " + Client.User.Nickname;
            lb_Header.Content = "ViComm | Nick: " + Client.User.Nickname;
            BackgroundColor = Brushes.CadetBlue;

            State = FormState.Exit;
            inputBox.Focus();
            
            Client.MultiChatMessageReceived += MultiChatReceived;
            Client.SingleChatMessageReceived += SingleChatWindow.SingleChatReceived;
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
            Client.Send(s, PacketType.MultiChat);
        }

        public void MultiChatReceived(Packet p)
        {
            var sound = new Sound();

            if (p.Type == PacketType.Information) {
                sound.Play(Sound.SoundType.Available);
                
                AppendInfoText($"* {p.Information.User} {p.Information.Message}");
            }
            else {
                RowType rowType;

                if (p.Sender != Client.User.Nickname) {
                    rowType = RowType.Sender;
                    sound.Play(Sound.SoundType.Message);
                }
                else {
                    rowType = RowType.User;
                }

                AppendText(p, rowType);
            }
        }

        public void AppendText(Packet packet, RowType rowType)
        {
            chatBox.Document.Blocks.Add(MessageManager.Message(packet, rowType, chatBox));
            chatBox.CenterTextVertically();
            chatBox.ScrollToEnd();
        }

        public void AppendInfoText(string text)
        {
            var p = new Paragraph {Background = Brushes.Transparent};

            p.Inlines.Add(new Run(text));

            p.DetectUrl();

            chatBox.Document.Blocks.Add(p);
            chatBox.ScrollToEnd();
        }

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

        private void CM_Logout_Click(object sender, RoutedEventArgs e)
        {
            State = FormState.Logout;

            Forms.Dispatcher.Invoke(new Action(() => {
                foreach (var single in Forms.SingleChat.Values) {
                    single.Close();
                }

                Forms.SingleChat.Clear();

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
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
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
            Client?.Disconnect(false);
            FormHelper.IsClosing = true;
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
        
        private void inputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _timeElapsed = 0;

            if (_tStarted) {
                return;
            }

            _tStarted = true;
            _tWriting = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 100)};
            _tWriting.Tick += t_writing_Tick;
            _tWriting.Start();

            var p = new Packet(PacketType.Information) {
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
                    Information = new Information(InformationType.Writing, Client.User.Nickname, "stopped")
                };
                Client.Send(p);
            }
            else {
                _timeElapsed += 100;
            }
        }

        private void list_Contacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var l = sender as ListBox;
            l?.UpdateLayout();
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
            var notify = FormHelper.Instance.Notify;

            notify?.Close();
        }

        #region Properties

        public ClientManager Client { get; set; }

        public FormHelper Forms { get; set; }

        public SettingsWindow Settings { get; set; }

        public FormState State { get; set; }

        public Brush BackgroundColor { get; set; }

        public ListBox ListContacts => list_Contacts;

        public ScrollViewer GetEmoticonsContainer() => emoticonsContainer;

        public TextBox GetInputTextBox() => inputBox;

        #endregion Properties

        private void list_Contacts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var obj = (DependencyObject)e.OriginalSource;

            while (obj != null && !Equals(obj, list_Contacts)) {
                if (obj.GetType() == typeof(ListBoxItem)) {
                    var name = ((ListBox)e.Source).SelectedValue as string;

                    if (name == null) {
                        return;
                    }

                    if (!Forms.SingleChat.ContainsKey(name)) {
                        Forms.SingleChat.Add(name, new SingleChatWindow(name));
                    }

                    Forms.SingleChat[name].Show();
                    Forms.SingleChat[name].Focus();

                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
        }
    }
}