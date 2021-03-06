using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using ViData;

namespace ViCommV2
{
    public class ClientManager : IDisposable
    {
        private static ClientManager _instance;
        private readonly IPEndPoint ip;
        private readonly Socket socket;
        private byte[] buffer;
        public FormHelper forms = FormHelper.Instance;
        private string guid;
        public User user;

        public ClientManager(string host, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveTimeout = 10;
            socket.SendTimeout = 10;
            ip = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public static ClientManager Instance {
            get {
                if (_instance == null) {
                    _instance = new ClientManager(Tools.GetIP(), 5555);
                }

                return _instance;
            }
        }

        public bool Connected {
            get { return socket.Connected; }
        }

        public void Connect()
        {
            if (socket.Connected) {
                return;
            }

            try {
                socket.Connect(ip);
                StartReceiving();
            }
            catch {
                MessageBox.Show("Could not connect to server!", "Connecting error!");
            }
        }

        public void Login(string login, byte[] pwd)
        {
            var p = new Packet(PacketType.Login);
            p.User = new User(login, pwd);
            Send(p);
        }

        public void LoginResult(Packet p)
        {
            var msg = p.Message;

            if (p.User != null) {
                guid = p.Message;
                user = p.User;
                var contacts = p.Information.Contacts.ToArray();

                forms.Dispatcher.Invoke(new Action(() => {
                    forms.Main = new MainWindow();

                    if (forms.Login != null) {
                        forms.Login.state = LoginWindow.FormState.Logged;
                        forms.Login.Close();
                    }

                    foreach (var c in contacts) {
                        forms.Main.ListContacts.Items.Add(c);
                    }

                    forms.Main.Show();
                }));
            }
            else {
                MessageBox.Show(msg, "Login error!");
            }
        }

        public void Register(string name, string email, byte[] pwd)
        {
            var p = new Packet(PacketType.Register);
            p.User = new User(name, email, pwd, Tools.GenerateSalt());

            Send(p);
        }

        public void RegisterResult(Packet p)
        {
            var result = p.Message[0].ToString();
            var msg = p.Message.Substring(1);

            // Result of registration
            MessageBox.Show(msg, "Register");
            if (result == "1") {
                Login(p.User.Username, p.User.Password);
                forms.Dispatcher.Invoke(new Action(() => {
                    forms.Register.Registered = true;
                    forms.Register.Close();
                }));
            }
        }

        public void Send(string msg)
        {
            var p = new Packet(PacketType.MultiChat);
            p.User = new User(user.AvatarURI, user.NickColor);
            p.Date = DateTime.Now;
            p.Sender = user.Nickname;
            p.Message = msg;

            Send(p);
        }

        public void Send(Packet p)
        {
            try {
                socket.Send(p.ToBytes());
            }
            catch (SocketException e) {
                App.HandleException(e);
            }
            catch (Exception e) {
                App.HandleException(e);
            }
        }

        public void StartReceiving()
        {
            FormHelper.isClosing = false;
            buffer = new byte[8192];

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, null);
        }

        private void OnReceive(IAsyncResult ar)
        {
            try {
                if (FormHelper.isClosing) {
                    return;
                }

                var readBytes = socket.EndReceive(ar);

                if (readBytes > 0) {
                    Received(new Packet(buffer));
                }

                buffer = new byte[8192];
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, null);
            }
            catch (ObjectDisposedException) {}
            catch (SocketException e) {
                App.HandleException(e);
            }
            catch (Exception e) {
                App.HandleException(e);
            }
        }

        private void Received(Packet p)
        {
            switch (p.Type) {
                case PacketType.Login:
                    LoginResult(p);
                    break;

                case PacketType.Disconnect:
                    Disconnect(p);
                    break;

                case PacketType.Register:
                    RegisterResult(p);
                    break;

                case PacketType.Information:
                    Information(p);
                    break;

                case PacketType.SingleChat:
                    // Reserved
                    break;

                case PacketType.MultiChat:
                    AppendText(p);
                    break;

                default:
                    break;
            }
        }

        private void Information(Packet p)
        {
            var main = forms.Main;

            if (p.Information.Type == InformationType.Joining) {
                AddItemToListBox(main.ListContacts, p.Information.User);
                AppendText(p);
            }
            else if (p.Information.Type == InformationType.Leaving) {
                RemoveItemFromListBox(main.ListContacts, p.Information.User);
                AppendText(p);
            }
            else if (p.Information.Type == InformationType.Writing) {
                if (p.Information.Message == "started") {
                    ChangeItemInListBox(main.ListContacts, p.Information.User, "*| " + p.Information.User);
                }
                else {
                    ChangeItemInListBox(main.ListContacts, "*| " + p.Information.User, p.Information.User);
                }
            }
        }

        public void AddItemToListBox(ListBox list, object o)
        {
            forms.Dispatcher.Invoke(new Action(() => { list.Items.Add(o); }));
        }

        public void RemoveItemFromListBox(ListBox list, object o)
        {
            forms.Dispatcher.Invoke(new Action(() => {
                if (list.Items.Contains(o)) {
                    list.Items.Remove(o);
                }
            }));
        }

        public void ChangeItemInListBox(ListBox list, object o, object changed)
        {
            forms.Dispatcher.Invoke(new Action(() => {
                if (list.Items.Contains(o)) {
                    var i = list.Items.IndexOf(o);
                    list.Items[i] = changed;
                }
            }));
        }

        private void AppendText(Packet packet)
        {
            forms.Dispatcher.Invoke(new Action(() => {
                var main = forms.Main;
                var sound = new Sound();

                if (packet.Type == PacketType.Information) {
                    sound.Play(Sound.SoundType.Available);

                    var msg = packet.Information.User + " " + packet.Information.Message;
                    main.AppendInfoText(string.Format("* {0}", msg));
                }
                else {
                    MainWindow.RowType rowType;

                    if (packet.Sender != user.Nickname) {
                        rowType = MainWindow.RowType.Sender;
                        sound.Play(Sound.SoundType.Message);
                    }
                    else {
                        rowType = MainWindow.RowType.User;
                    }

                    main.AppendText(packet, rowType);
                }
            }));
        }

        public void Disconnect(Packet p)
        {
            MessageBox.Show(p.Message, p.Sender);

            forms.Dispatcher.Invoke(new Action(() => {
                forms.Login = new LoginWindow();
                forms.Login.Show();

                if (forms.Main != null) {
                    forms.Main.State = MainWindow.FormState.Close;
                    forms.Main.CloseWindow();
                }
            }));

            Disconnect(true);
        }

        public void Disconnect(bool server)
        {
            if (server == false) {
                if (socket.Connected) {
                    var p = new Packet(PacketType.Disconnect);
                    p.Message = guid;
                    socket.Send(p.ToBytes());
                }
            }

            CloseConnection();
        }

        private void CloseConnection()
        {
            if (socket != null) {
                socket.Close();
            }

            _instance = null;
        }

        #region Dispose Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ClientManager()
        {
            Dispose(false);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) {
                return;
            }

            if (disposing) {
                // Free any other managed objects here.
                socket.Dispose();
            }
            // Free any unmanaged objects here.

            _disposed = true;
        }

        #endregion Dispose Implementation
    }
}