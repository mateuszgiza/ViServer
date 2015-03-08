using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

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
			Console.WriteLine("\n** Starting server...\n");

			try {
				listener = new TcpListener(ipaddr, port);
				listener.Start();

				Listen();
			}
			catch (SocketException e) {
				Console.WriteLine("|#| SocketException: {0}", e);
			}
			finally {
				listener.Stop();
			}
		}

		private void Listen()
		{
			Byte[] bytes = new Byte[256];
			String data = null;

			while ( true ) {
				Console.WriteLine("* Waiting for a connection...");

				TcpClient client = listener.AcceptTcpClient();
				Console.WriteLine("^ Connected!");

				data = null;

				NetworkStream stream = client.GetStream();

				int i;
				while((i = stream.Read(bytes, 0, bytes.Length)) != 0) {
					data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
					Console.WriteLine("^ Received: {0}", data);

					data.ToUpper();

					byte[] msg = System.Text.Encoding.UTF8.GetBytes(data);

					stream.Write(msg, 0, msg.Length);
					Console.WriteLine("^ Sent: {0}", data);
				}
				client.Close();
			}
		}
	}
}
