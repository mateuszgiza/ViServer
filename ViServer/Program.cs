using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ViServer
{
	class Program
	{
		static void Main(string[] args)
		{
			ShowMenu();
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
			Server server = new Server("127.0.0.1", 5555, 10);
			server.Start();
		}
	}

	class Server
	{
		private TcpListener listener;
		private Int32 port;
		private IPAddress ipaddr;
		private string password;

		public Int32 size;

		public Server(string ip, int port, int size, string pwd = null)
		{
			listener = null;
			this.ipaddr = IPAddress.Parse(ip);
			this.port = port;
			this.size = size;
			this.password = pwd;
		}



		public void Start()
		{
			Console.WriteLine("\n** Starting server...");

			try {
				listener = new TcpListener(ipaddr, port);
				listener.Start();

				Listen();
			}
			catch ( SocketException e ) {
				Console.WriteLine("|#| SocketException: {0}", e);
			}
			finally {
				listener.Stop();
			}
		}
		public static List<Client> clients = new List<Client>();
		private void Listen()
		{
			int id = 0;
			while ( true ) {
				Console.WriteLine("\n* Waiting for a connection...");

				TcpClient client = listener.AcceptTcpClient();
				Console.WriteLine("^ Connected!");
				Client c1 = new Client(id);
				clients.Add(c1);
				ThreadPool.QueueUserWorkItem(c1.ThreadProc, client);
				id++;
			}
		}
	}

	struct Packet
	{
		public PacketStruct receive;

		public Packet(PacketStruct p)
		{
			receive = p;
		}

		public string PacketToString()
		{
			return JsonConvert.SerializeObject(receive, Formatting.Indented);
		}

		public PacketStruct ToPacket(string p)
		{
			receive = JsonConvert.DeserializeObject<PacketStruct>(p);
			return receive;
		}
	}

#pragma warning disable 0649
	public struct PacketStruct
	{
		public Int16 type;
		public String receiver;
		public String sender;
		public String message;
	}
#pragma warning restore 0649

	public class Client
	{
		private int _id;
		public String Name;
		TcpClient client;
		NetworkStream stream;
		Queue<PacketStruct> packets = new Queue<PacketStruct>();
		Byte[] data = new Byte[1024];

		public Client(int id)
		{
			this._id = id;
		}

		public int Id
		{
			get
			{
				return this._id;
			}
		}

		public void Send(PacketStruct packet)
		{
			// Individual Chat
			if ( packet.type == 1 ) {
				// Send message to Receiver
				Byte[] sender = Encoding.UTF8.GetBytes(new Packet(packet).PacketToString());
				stream.BeginWrite(sender, 0, sender.Length, new AsyncCallback(OnSend), stream);
				//stream.Write(sender, 0, sender.Length);

				Console.WriteLine("[{0}] Sending packet ({1})...\n\tFrom: {2}\n\tTo: {3}\n\t{4}",
						_id, packet.type, packet.sender, packet.receiver, sender.Length);
			}
		}

		private void OnSend(IAsyncResult result)
		{
			NetworkStream stream = (NetworkStream) result.AsyncState;

			try {
				stream.EndWrite(result);
			}
			catch ( SocketException socketException ) {
				if ( socketException.ErrorCode == 10054 ||
					( ( socketException.ErrorCode != 10004 ) &&
					( socketException.ErrorCode != 10053 ) ) ) {
					String remoteIP = ( (IPEndPoint) client.Client.RemoteEndPoint ).Address.ToString();
					String remotePort = ( (IPEndPoint) client.Client.RemoteEndPoint ).Port.ToString();
					CloseConnection();
				}
			}
			catch ( Exception exception ) {
				Console.WriteLine(exception.Message + "\n" + exception.StackTrace);
				CloseConnection();
			} 
		}

		public void Receive()
		{
			stream = client.GetStream();

			// Read Packet
			data = new Byte[1024];
			stream.BeginRead(data, 0, data.Length, new AsyncCallback(OnReceive), stream);
		}

		private void OnReceive(IAsyncResult result)
		{
			NetworkStream stream = (NetworkStream) result.AsyncState;
			
			try {
				int size = stream.EndRead(result);

				if ( size != 0 ) {
					PacketStruct packet = new Packet().ToPacket(Encoding.UTF8.GetString(data, 0, size));

					if ( packet.type > 0 ) {
						packets.Enqueue(packet);
						new Task(FindAndSend).Start();
					}
					else {
						if ( packet.message == "disconnect" ) {
							CloseConnection();
						}
						else if ( packet.message == "login" ) {
							Name = packet.sender;
							Console.WriteLine("[{0}] Logged as {1}.", _id, Name);
						}
					}
				}
				data = new Byte[1024];
				stream.BeginRead(data, 0, data.Length, new AsyncCallback(OnReceive), stream);
			}
			catch ( SocketException socketException ) {
				if (socketException.ErrorCode == 10054 ||
					( ( socketException.ErrorCode != 10004 ) && 
					( socketException.ErrorCode != 10053 ) ) ) {
					//String remoteIP = ( (IPEndPoint) client.Client.RemoteEndPoint ).Address.ToString();
					//String remotePort = ( (IPEndPoint) client.Client.RemoteEndPoint ).Port.ToString();
					CloseConnection();
				}
			}
			catch ( Exception exception ) {
				Console.WriteLine(exception.Message + "\n" + exception.StackTrace);
				CloseConnection();
			} 
		}

		private void FindAndSend()
		{
			PacketStruct packet = new PacketStruct();
			packet = packets.Dequeue();
			bool state = true;

			while ( state ) {
				int index = Server.clients.FindIndex(Client => Client.Name == packet.receiver);
				if ( index >= 0 ) {
					state = false;
					Console.WriteLine("Found! {0}", Server.clients[index].Name);
					Server.clients[index].Send(packet);
				}
			}

		}

		public void ThreadProc(object obj)
		{
			client = (TcpClient) obj;

			Receive();
		}

		private void CloseConnection()
		{
			Console.WriteLine("[{0}] Disconnected!", _id);
			client.Client.Disconnect(true);
			stream.Close();
			stream = null; 
			client.Close();
			Server.clients.Remove(this);
		}
	}
}
