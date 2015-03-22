using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System.Data.SqlClient;

namespace ViData
{
	public class Tools
	{
		private Tools()
		{

		}

		public static string GetStartupPath()
		{
			return Environment.CurrentDirectory;
		}

		public static void CreateFile(string file)
		{
			CreateFile(file, "127.0.0.1");
		}

		public static void CreateFile(string file, string line)
		{
			string path = GetStartupPath() + @"\" + file;
			if ( !File.Exists(path) ) {
				File.Create(path).Dispose();
				TextWriter tw = new StreamWriter(path);
				tw.WriteLine(line);
				tw.Close();
			}
		}

		public static string ReadLineTxt(string file)
		{
			string line = "";
			string path = GetStartupPath() + @"\" + file;
			if ( File.Exists(path) ) {
				StreamReader f = new StreamReader(path);
				line = f.ReadLine();
				f.Close();
			}

			return line;
		}

		public static string GetIP()
		{
			string ip = ReadLineTxt("ip.txt");

			if ( ip.Length > 0 ) {
				return ip;
			}
			else {
				return ( GetIPFromHostname("viserver.noip.pl") );
			}
		}

		public static string GetIPFromHostname(string host)
		{
			return Dns.GetHostAddresses(host)[0].ToString();
		}
	}

	[Serializable]
	public class Packet
	{
		public PacketType type;
		public string sender;
		public string receiver;
		public string message;

		public Packet(PacketType type, string msg)
		{
			this.type = type;
			this.message = msg;
		}

		public Packet(PacketType type)
		{
			this.type = type;
		}

		public Packet(byte[] buffer)
		{
			BinaryFormatter bin = new BinaryFormatter();
			MemoryStream ms = new MemoryStream(buffer);

			Packet p = (Packet) bin.Deserialize(ms);
			ms.Close();

			type = p.type;
			sender = p.sender;
			receiver = p.receiver;
			message = p.message;
		}

		public byte[] ToBytes()
		{
			BinaryFormatter bin = new BinaryFormatter();
			MemoryStream ms = new MemoryStream();

			bin.Serialize(ms, this);
			byte[] bytes = ms.ToArray();
			ms.Close();

			return bytes;
		}

		public static string GetIPv4()
		{
			IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

			foreach ( IPAddress ip in ips ) {
				if ( ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ) {
					return ip.ToString();
				}
			}

			return "127.0.0.1";
		}
	}

	public enum PacketType
	{
		MultiChat,
		SingleChat,
		Login,
		Register,
		Disconnect
	}

	public class Database
	{
		private SqlConnection sql;
		private SqlCommand command;
		private SqlDataReader reader;

		public string Query;

		public Database(bool start)
		{
			sql = new SqlConnection();

			string path = Environment.CurrentDirectory;

			sql.ConnectionString = @"Data Source=.\SQLEXPRESS;
						  AttachDbFilename=" + path + @"\ViServer.mdf;
						  Integrated Security=True;
						  Connect Timeout=30;
						  User Instance=True";

			if ( start )
				Connect();
		}

		public void Connect()
		{
			try {
				sql.Open();
				Console.WriteLine("Connection Open!");

			}
			catch ( Exception ex ) {
				Console.WriteLine("Can not open connection!");
			}
		}

		public bool Execute(string query)
		{
			Query = query;
			return Execute();
		}

		public bool Execute()
		{
			if ( Query == null ) {
				return false;
			}

			command = new SqlCommand(Query, sql);
			reader = command.ExecuteReader();
			while ( reader.Read() ) {
				//MessageBox.Show(reader.GetValue(0) + " - " + reader.GetValue(1) + " - " + reader.GetValue(2));
			}
			reader.Close();
			command.Dispose();

			return true;
		}

		public void Close()
		{
			sql.Close();
		}
	}

	[Serializable]
	public class Users
	{
		public int ID;
		public string LoginName;
		public string Nickname;
		public string Email;
		public string Password;
		public string Salt;

		public Users()
		{

		}
	}
}
