using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ViData;

namespace ViServer
{
    public class Client : IDisposable
    {
        private int _id;
        private string _guid;
        public string Name;

        private Socket _clientSocket;
        private Thread _thread;

        public Client(int id, Socket socket)
        {
            _id = id;
            _guid = Guid.NewGuid().ToString();
            _clientSocket = socket;
        }

        public Thread SetThread
        {
            set { _thread = value; }
        }

        public int Id
        {
            get { return _id; }
        }

        public void Start(object o)
        {
            Receive();
        }

        public void Send(Packet p)
        {
            switch (p.Type) {
                case PacketType.Login:
                    // Reserved
                    break;
                case PacketType.Disconnect:
                    // Reserved
                    break;
                case PacketType.Register:
                    // Reserved
                    break;
                case PacketType.SingleChat:
                    // Implemented
                    break;
                case PacketType.MultiChat:
                    // Implemented
                    break;
            }
        }

        public void Receive()
        {
            byte[] buffer;
            int readBytes;

            while (_clientSocket != null && _clientSocket.Connected) {
                try {
                    buffer = new byte[_clientSocket.SendBufferSize];
                    readBytes = _clientSocket.Receive(buffer);

                    if (readBytes > 0) {
                        Packet p = new Packet(buffer);

                        if (p.Type != PacketType.Disconnect) {
                            new Task(() => Received(p)).Start();
                        }
                        else {
                            CloseConnection();
                        }
                    }
                }
                catch (SocketException e) {
                    Console.WriteLine(e);
                    CloseConnection();
                }
            }
        }

        public void Received(Packet p)
        {
            switch (p.Type) {
                case PacketType.Login:
                    Login(p.User);
                    break;
                case PacketType.Disconnect:
                    CloseConnection();
                    break;
                case PacketType.Register:
                    Register(p.User);
                    break;

                case PacketType.Information:
                    InformationReceived(p);
                    break;

                case PacketType.SingleChat:
                    SingleChat(p);
                    break;
                case PacketType.MultiChat:
                    MultiChat(p);
                    break;
            }
        }

        public static void SingleChat(Packet p)
        {
            Client client = Server.Clients.Find(c => c.Name.Equals(p.Receiver));

            client?._clientSocket.Send(p.ToBytes());
        }

        public static void MultiChat(Packet p)
        {
            foreach (Client c in Server.Clients) {
                c._clientSocket.Send(p.ToBytes());
            }
        }

        private void MultiChatWithoutMe(Packet p)
        {
            foreach (Client c in Server.Clients) {
                if (c != this) {
                    c._clientSocket.Send(p.ToBytes());
                }
            }
        }

        private void Login(User u)
        {
            bool result = u.Login();
            string msg;

            if (result) {
                Name = u.Username;
                Console.WriteLine("[{0}][Login] {1} with Guid: {2}", _id, Name, _guid);
                msg = _guid;

                Packet p = new Packet(PacketType.Information);
                p.Information = new Information(InformationType.Joining, Name, "joined!");
                MultiChatWithoutMe(p);
            }
            else {
                u = null;
                msg = "error";
            }

            Packet p1 = new Packet(PacketType.Login);
            p1.User = u;
            p1.Message = msg;
            p1.Information = new Information(InformationType.Contacts, Server.GetContacts());

            _clientSocket.Send(p1.ToBytes());
        }

        private void Register(User user)
        {
            //User user = new User(u);
            bool result = user.Register();

            Packet res = new Packet(PacketType.Register);
            if (result) {
                res.Message = "1You've successfully registered!";
                res.User = user;
            }
            else {
                res.Message = "0Something went wrong!";
            }
            _clientSocket.Send(res.ToBytes());
        }

        private void InformationReceived(Packet p)
        {
            if (p.Information.Type == InformationType.Writing) {
                MultiChatWithoutMe(p);
            }
        }

        private void CloseConnection()
        {
            Console.WriteLine("[{0}] User {1} disconnected!", _id, Name);

            Packet p = new Packet(PacketType.Information);
            p.Information = new Information(InformationType.Leaving, Name, "disconnected!");
            MultiChatWithoutMe(p);

            Disconnect();
        }

        private void Disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            _clientSocket.Close();
            _clientSocket.Dispose();
            _clientSocket = null;
            Server.Clients.Remove(this);
            _thread.Abort();
        }
    }
}
