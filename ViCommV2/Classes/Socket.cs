using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using ViData;

namespace ViCommV2.Classes
{
    public class ClientManager
    {
        private static ClientManager _instance;
        private readonly IPEndPoint _ip;
        private readonly Socket _socket;
        private byte[] _buffer;
        public FormHelper Forms = FormHelper.Instance;
        private string _guid;
        public User User;

        public delegate void MultiChatEventHandler(Packet p);
        public delegate void SingleChatEventHandler(Packet p);

        public MultiChatEventHandler MultiChatMessageReceived;
        public SingleChatEventHandler SingleChatMessageReceived;

        public ClientManager(string host, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
                ReceiveTimeout = 10,
                SendTimeout = 10
            };
            _ip = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public static ClientManager Instance => _instance ?? (_instance = new ClientManager(Tools.GetIp(), 5555));

        public bool Connected => _socket.Connected;

        public void Connect()
        {
            if (_socket.Connected) {
                return;
            }

            try {
                _socket.ReceiveTimeout = 100;
                _socket.SendTimeout = 100;
                _socket.Connect(_ip);
                StartReceiving();
            }
            catch {
                MessageBox.Show("Could not connect to server!", "Connecting error!");
            }
        }

        public void Login(string login, byte[] pwd)
        {
            var p = new Packet(PacketType.Login) {
                User = new User(login, pwd)
            };
            Send(p);
        }

        public void LoginResult(Packet p)
        {
            var msg = p.Message;

            if (p.User != null) {
                _guid = p.Message;
                User = p.User;
                var contacts = p.Information.Contacts.ToArray();

                Forms.Dispatcher.Invoke(new Action(() => {
                    Forms.Main = new MainWindow();

                    if (Forms.Login != null) {
                        Forms.Login.State = LoginWindow.FormState.Logged;
                        Forms.Login.Close();
                        Forms.Login = null;
                    }

                    foreach (var c in contacts) {
                        Forms.Main.ListContacts.Items.Add(c);
                    }

                    Forms.Main.Show();
                }));
            }
            else {
                MessageBox.Show(msg, "Login error!");
            }
        }

        public void Register(string name, string email, byte[] pwd)
        {
            var p = new Packet(PacketType.Register) {
                User = new User(name, email, pwd, Tools.GenerateSalt())
            };

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
                Forms.Dispatcher.Invoke(new Action(() => {
                    Forms.Register.Registered = true;
                    Forms.Register.Close();
                    Forms.Register = null;
                }));
            }
        }

        public void Send(string msg, PacketType type)
        {
            Send(msg, type, null);
        }

        public void Send(string msg, PacketType type, string receiver)
        {
            var p = new Packet(type) {
                User = new User(User.AvatarUri, User.NickColor),
                Date = DateTime.Now,
                Sender = User.Nickname,
                Receiver = receiver,
                Message = msg
            };

            Send(p);
        }

        public void Send(Packet p)
        {
            try {
                _socket.Send(p.ToBytes());
                
                if (p.Type == PacketType.SingleChat) {
                    Forms.SingleChat[p.Receiver].AppendText(p);
                }
            }
            catch (Exception e) {
                App.HandleException(e);
            }
        }

        public void StartReceiving()
        {
            if (!_socket.Connected) {
                return;
            }

            FormHelper.IsClosing = false;
            _buffer = new byte[8192];

            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, null);
        }

        private void OnReceive(IAsyncResult ar)
        {
            try {
                if (FormHelper.IsClosing) {
                    return;
                }

                var readBytes = _socket.EndReceive(ar);

                if (readBytes > 0) {
                    Received(new Packet(_buffer));
                }

                _buffer = new byte[8192];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, null);
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
                    Forms.Dispatcher.Invoke(new Action(() => SingleChatMessageReceived(p)));
                    break;

                case PacketType.MultiChat:
                    Forms.Dispatcher.Invoke(new Action(() => MultiChatMessageReceived(p)));
                    break;
            }
        }

        private void Information(Packet p)
        {
            if (!string.IsNullOrEmpty(p.Receiver)) {
                if (p.Information.Type != InformationType.Writing) return;

                var sender = p.Information.User;

                Forms.Dispatcher.Invoke(new Action(() => {
                    if (!Forms.SingleChat.ContainsKey(sender)) {
                        return;
                    }

                    var window = Forms.SingleChat[sender];
                    window.Title = p.Information.Message == "started" ? $"Private Chat: {sender}..." : $"Private Chat: {sender}";
                }));
                
                return;
            }

            var main = Forms.Main;

            switch (p.Information.Type) {
                case InformationType.Joining:
                    AddItemToListBox(main.ListContacts, p.Information.User);
                    AppendText(p);
                    break;
                case InformationType.Leaving:
                    RemoveItemFromListBox(main.ListContacts, p.Information.User);
                    AppendText(p);
                    break;
                case InformationType.Writing:
                    if (p.Information.Message == "started") {
                        ChangeItemInListBox(main.ListContacts, p.Information.User, "*| " + p.Information.User);
                    }
                    else {
                        ChangeItemInListBox(main.ListContacts, "*| " + p.Information.User, p.Information.User);
                    }
                    break;
            }
        }
        
        public void AddItemToListBox(ListBox list, object obj)
        {
            Forms.Dispatcher.Invoke(new Action(() => { list.Items.Add(obj); }));
        }

        public void RemoveItemFromListBox(ListBox list, object obj)
        {
            Forms.Dispatcher.Invoke(new Action(() => {
                if (list.Items.Contains(obj)) {
                    list.Items.Remove(obj);
                }
            }));
        }

        public void ChangeItemInListBox(ListBox list, object current, object newObject)
        {
            Forms.Dispatcher.Invoke(new Action(() => {
                if (list.Items.Contains(current)) {
                    var i = list.Items.IndexOf(current);
                    list.Items[i] = newObject;
                }
            }));
        }

        private void AppendText(Packet packet)
        {
            Forms.Dispatcher.Invoke(new Action(() => {
                var sound = new Sound();

                if (packet.Type != PacketType.Information) {
                    return;
                }

                sound.Play(Sound.SoundType.Available);

                var msg = packet.Information.User + " " + packet.Information.Message;
                Forms.Main.AppendInfoText($"* {msg}");
            }));
        }

        public void Disconnect(Packet p)
        {
            MessageBox.Show(p.Message, p.Sender);

            Forms.Dispatcher.Invoke(new Action(() => {
                Forms.Login = new LoginWindow();
                Forms.Login.Show();

                if (Forms.Main == null) {
                    return;
                }

                Forms.Main.State = FormState.Close;
                Forms.Main.CloseWindow();
            }));

            Disconnect(true);
        }

        public void Disconnect(bool server)
        {
            if (server == false) {
                if (_socket.Connected) {
                    var p = new Packet(PacketType.Disconnect) { Message = _guid };
                    _socket.Send(p.ToBytes());
                }
            }

            CloseConnection();
        }

        private void CloseConnection()
        {
            _socket?.Close();

            _instance = null;
        }
    }
}