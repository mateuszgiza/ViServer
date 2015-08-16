using System;
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

			while (choice != ConsoleKey.D0) {
				Console.WriteLine("--------------------------");
				Console.WriteLine("\tViServer");
				Console.WriteLine("--------------------------");
				Console.WriteLine("[1] Start server");

				choice = Console.ReadKey().Key;
				Console.Write("\r");

				switch (choice) {
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
			switch (sig) {
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
}
