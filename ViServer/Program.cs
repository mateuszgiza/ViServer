using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ViData;

namespace ViServer
{
	class Program
	{
		static void Main(string[] args)
		{
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
			Server server = new Server();
			server.Start();
		}
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
		public String Name;

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

			while (clientSocket != null && clientSocket.Connected ) {
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
				catch (SocketException e) {
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
					foreach ( Client c in Server.clients ) {
						c.clientSocket.Send(p.ToBytes());
					}
					break;
				default:

					break;
			}
		}

		private void Login(User u)
		{
			bool result = u.Login();
			string msg;

			if ( result ) {
				Console.WriteLine("[{0}][Login] {1} with Guid: {2}", _id, u.Username, _guid);
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
			Console.WriteLine("[{0}] Disconnected!", _id);
			clientSocket.Close();
			clientSocket = null;
			Server.clients.Remove(this);
			this.thread.Abort();
		}
	}
}
