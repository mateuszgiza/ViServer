using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViServer
{
	class Program
	{
		static void Main(string[] args)
		{
			ShowMenu();

			Console.ReadKey();
		}

		static void ShowMenu()
		{
			ConsoleKey choice;

			while ( choice != ConsoleKey.D0 ) {
				Console.WriteLine("--------------------------");
				Console.WriteLine("\tViServer");
				Console.WriteLine("--------------------------");
				Console.WriteLine("[1] Start server");

				choice = Console.ReadKey().Key;

				switch ( choice ) {
					case ConsoleKey.D0:
						Console.WriteLine("* Stopping server...");
						break;
					case ConsoleKey.D1:
						CreateServer();
						break;
					default:
						Console.WriteLine("* Try again!");
						break;
				}
			}
		}

		static void CreateServer()
		{
			
		}
	}

	class Server
	{
		public Server(string ip, int port, int size, string pwd = null)
		{

		}

		public void Start()
		{
			Console.WriteLine("** Starting server...");
		}
	}
}
