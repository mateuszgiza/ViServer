using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ViData;

namespace ViServer
{
    public class Server
    {
        private Socket _listenerSocket;
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
                _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listenerSocket.Bind(ip);

                Console.WriteLine("* Listening on {0}:{1}", ip.Address.ToString(), ip.Port);
                Listen();
            }
            catch (SocketException e) {
                Console.WriteLine("|#| SocketException: {0}", e);
            }

            _listenerSocket.Close();
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

        public static List<Client> Clients;
        public static List<Thread> Threads;
        private void Listen()
        {
            Clients = new List<Client>();
            Threads = new List<Thread>();

            int id = 0;
            while (true) {
                _listenerSocket.Listen(0);
                Console.WriteLine("\n* Waiting for a connection...");

                Client c1 = new Client(id, _listenerSocket.Accept());
                Clients.Add(c1);
                Console.WriteLine("^ Connected!");

                Thread t = new Thread(c1.Start);
                c1.SetThread = t;
                t.Start();
                //ThreadPool.QueueUserWorkItem(c1.Start);
                id++;
            }
        }

        public static List<string> GetContacts()
        {
            return Clients.Select(c => c.Name).ToList();
        }
    }
}
