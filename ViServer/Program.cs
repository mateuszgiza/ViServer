using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ViData;
using System.Runtime.InteropServices;

namespace ViServer
{
	class Program
	{
		static Server server;

		static void Main(string[] args)
		{
			_handler += new EventHandler(Handler);
			SetConsoleCtrlHandler(_handler, true);
			//ShowMenu();
			CreateServer();
		}

		static void ShowMenu()
		{
			ConsoleKey choice = new ConsoleKey();

			while ( choice != ConsoleKey.D0 ) {
				Console.WriteLine("--------------------------");
				Console.WriteLine("\tViServer");
				Console.WriteLine("--------------------------");
				Console.WriteLine("[1] Start server");

				choice = Console.ReadKey().Key;
				Console.Write("\r");

				switch ( choice ) {
					case ConsoleKey.D0:
						Console.WriteLine("** Stopping server...");
						break;
					case ConsoleKey.D1:
						CreateServer();
						break;
					default:
						Console.WriteLine("> Try again!");
						break;
				}
			}
		}

		static void CreateServer()
		{
			server = new Server();
			server.Start();
		}

		#region Closing Console Event

		[DllImport("Kernel32")]
		private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

		private delegate bool EventHandler(CtrlType sig);
		static EventHandler _handler;

		enum CtrlType
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT = 1,
			CTRL_CLOSE_EVENT = 2,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT = 6
		}

		private static bool Handler(CtrlType sig)
		{
			switch ( sig ) {
				case CtrlType.CTRL_C_EVENT:
				case CtrlType.CTRL_LOGOFF_EVENT:
				case CtrlType.CTRL_SHUTDOWN_EVENT:
				case CtrlType.CTRL_CLOSE_EVENT:
					server.Stop();
					break;
				default:
					return false;
			}

			return true;
		}

		#endregion
	}

	class Server
	{
		private Socket listenerSocket;
		private IPEndPoint ip;

		public Server()
		{
			this.ip = new IPEndPoint(IPAddress.Any, 5555);
		}

		public void Start()
		{
			//Database db = new Database(true);

			Console.WriteLine("* Starting server...");

			try {
				listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				listenerSocket.Bind(ip);

				Console.WriteLine("* Listening on {0}:{1}", ip.Address.ToString(), ip.Port);
				Listen();
			}
			catch ( SocketException e ) {
				Console.WriteLine("|#| SocketException: {0}", e);
			}

			listenerSocket.Close();
		}

		public void Stop()
		{
			Stop("Server closed!");
		}

		public void Stop(string msg)
		{
			Packet p = new Packet(PacketType.Disconnect, "Server", msg);

			Client.MultiChat(p);
		}

		public static List<Client> clients;
		public static List<Thread> threads;
		private void Listen()
		{
			clients = new List<Client>();
			threads = new List<Thread>();

			int id = 0;
			while ( true ) {
				listenerSocket.Listen(0);
				Console.WriteLine("\n* Waiting for a connection...");

				Client c1 = new Client(id, listenerSocket.Accept());
				clients.Add(c1);
				Console.WriteLine("^ Connected!");

				Thread t = new Thread(c1.Start);
				c1.SetThread = t;
				t.Start();
				//ThreadPool.QueueUserWorkItem(c1.Start);
				id++;
			}
		}
	}

	public class Client
	{
		private int _id;
		private string _guid;
		public string Name;

		private Socket clientSocket;
		private Thread thread;

		public Client(int id, Socket socket)
		{
			this._id = id;
			this._guid = Guid.NewGuid().ToString();
			this.clientSocket = socket;
		}

		public Thread SetThread
		{
			set
			{
				this.thread = value;
			}
		}

		public int Id
		{
			get
			{
				return this._id;
			}
		}

		public void Start(object o)
		{
			Receive();
		}

		public void Send(Packet p)
		{
			switch ( p.type ) {
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
					Console.WriteLine("[{0}] Sending packet ({1})...\n\tFrom: {2}\n\tTo: {3}",
						_id, p.type, p.sender, p.receiver);
					break;
				case PacketType.MultiChat:

					break;
				default:

					break;
			}
		}

		public void Receive()
		{
			byte[] buffer;
			int readBytes;

			while ( clientSocket != null && clientSocket.Connected ) {
				try {
					buffer = new byte[clientSocket.SendBufferSize];
					readBytes = clientSocket.Receive(buffer);

					if ( readBytes > 0 ) {
						Packet p = new Packet(buffer);

						if ( p.type != PacketType.Disconnect ) {
							new Task(() => Received(p)).Start();
						}
						else {
							CloseConnection();
						}
					}
				}
				catch ( SocketException e ) {
					Console.WriteLine(e);
					CloseConnection();
				}
			}
		}

		public void Received(Packet p)
		{
			switch ( p.type ) {
				case PacketType.Login:
					Login(p.user);
					break;
				case PacketType.Disconnect:
					CloseConnection();
					break;
				case PacketType.Register:
					Register(p.user);
					break;
				case PacketType.SingleChat:
					// Reserved
					break;
				case PacketType.MultiChat:
					MultiChat(p);
					break;
				default:

					break;
			}
		}

		public static void MultiChat(Packet p)
		{
			foreach ( Client c in Server.clients ) {
				c.clientSocket.Send(p.ToBytes());
			}
		}
		private void MultiChatWithoutMe(Packet p)
		{
			foreach ( Client c in Server.clients ) {
				if ( c != this ) {
					c.clientSocket.Send(p.ToBytes());
				}
			}
		}

		private void Login(User u)
		{
			bool result = u.Login();
			string msg;

			if ( result ) {
				Name = u.Username;
				Console.WriteLine("[{0}][Login] {1} with Guid: {2}", _id, Name, _guid);
				MultiChatWithoutMe(new Packet(PacketType.MultiChat, "Server", Name + " joined!"));
				msg = _guid;
			}
			else {
				u = null;
				msg = "error";
			}

			clientSocket.Send(new Packet(PacketType.Login, u, msg).ToBytes());
		}

		private void Register(User user)
		{
			//User user = new User(u);
			bool result = user.Register();

			Packet res = new Packet(PacketType.Register);
			if ( result ) {
				res.message = "1You've successfully registered!";
				res.user = user;
			}
			else {
				res.message = "0Something went wrong!";
			}
			clientSocket.Send(res.ToBytes());
		}

		private void CloseConnection()
		{
			Console.WriteLine("[{0}] User {1} disconnected!", _id, Name);
			MultiChatWithoutMe(new Packet(PacketType.MultiChat, "Server", Name + " diconnected!"));
			clientSocket.Close();
			clientSocket = null;
			Server.clients.Remove(this);
			this.thread.Abort();
		}
	}
}
