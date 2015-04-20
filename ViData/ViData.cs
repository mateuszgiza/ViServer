using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System.Data;
using System.Security.Cryptography;

using MySql.Data.MySqlClient;

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

		public static string GetIPFromHostname(string host)
		{
			return Dns.GetHostAddresses(host)[0].ToString();
		}

		public static byte[] GenerateSalt()
		{
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] buff = new byte[128 / 8]; // 128 bits
			rng.GetBytes(buff);

			return buff;
		}

		public static byte[] Hash(string value, byte[] salt)
		{
			return Hash(Encoding.UTF8.GetBytes(value), salt);
		}

		public static byte[] Hash(byte[] value, byte[] salt)
		{
			byte[] saltedValue = value.Concat(salt).ToArray();

			return new SHA256Managed().ComputeHash(saltedValue);
		}
	}

	public class Database : IDisposable
	{
		MySqlConnection mysql;
		MySqlCommand cmd;
		MySqlDataReader reader;
		//MySqlDataAdapter adapter;

		public string Query;

		public Database()
			: this(false)
		{
		}

		public Database(bool start)
		{
			mysql = new MySqlConnection();
			mysql.ConnectionString = "server=127.0.0.1;uid=ViServer;" +
							"pwd=lubiemaslo;database=server;Charset=utf8;";

			if ( start )
				Connect();
		}

		public void Connect()
		{
			try {
				mysql.Open();
			}
			catch ( MySqlException ex ) {
				Exception(ex);
			}
		}

		public MySqlConnection Connection
		{
			get
			{
				return this.mysql;
			}
		}

		public static DataTable Select(MySqlCommand cmd)
		{
			DataTable tb = new DataTable();
			MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

			using ( cmd ) {
				try {
					if ( cmd.Connection.State == ConnectionState.Closed ) {
						cmd.Connection.Open();
					}

					adapter.Fill(tb);

					cmd.Connection.Close();
				}
				catch ( MySqlException ex ) {
					Exception(ex);
				}
			}

			return tb;
		}

		public static int Insert(MySqlCommand cmd)
		{
			int ret = -1;
			using ( cmd ) {
				try {
					if ( cmd.Connection.State == ConnectionState.Closed ) {
						cmd.Connection.Open();
					}

					ret = cmd.ExecuteNonQuery();

					cmd.Connection.Close();
				}
				catch ( MySqlException ex ) {
					Exception(ex);
				}
			}

			return ret;
		}

		public static int Exception(MySqlException ex)
		{
			int id = -1;

			switch ( ex.Number ) {
				case 0:
					Console.WriteLine("[DB] Cannot connect to server. Contact administrator");
					break;
				case 1045:
					Console.WriteLine("[DB] Invalid username/password, please try again");
					break;
				default:
					Console.WriteLine("[DB] Exception nr: {0} | {1}", ex.Number, ex.Message);
					break;
			}

			return id;
		}

		public void Close()
		{
			mysql.Close();
		}

		public void Dispose()
		{
			if ( reader != null ) {
				reader.Dispose();
				reader = null;
			}

			if ( cmd != null ) {
				cmd.Dispose();
				cmd = null;
			}

			if ( mysql != null ) {
				mysql.Dispose();
				mysql = null;
			}
		}
	}

	[Serializable]
	public class Packet
	{
		public PacketType type;
		public User user;
		public Information info;
		public DateTime date;
		public string sender;
		public string receiver;
		public string message;

		public Packet(PacketType type)
		{
			this.type = type;
		}

		public Packet(PacketType type, string msg)
		{
			this.type = type;
			this.message = msg;
		}

		public Packet(PacketType type, string sender, string msg)
		{
			this.type = type;
			this.sender = sender;
			this.message = msg;
		}

		public Packet(PacketType type, User u, string msg)
		{
			this.type = type;
			this.user = u;
			this.message = msg;
		}

		public Packet(PacketType type, Information i)
		{
			this.type = type;
			this.info = i;
		}

		public Packet(byte[] buffer)
		{
			Packet p;

			using ( MemoryStream ms = new MemoryStream(buffer) ) {
				BinaryFormatter bin = new BinaryFormatter();
				p = (Packet) bin.Deserialize(ms);
			}

			type = p.type;
			user = p.user;
			info = p.info;
			date = p.date;
			sender = p.sender;
			receiver = p.receiver;
			message = p.message;
		}

		public byte[] ToBytes()
		{
			byte[] bytes;

			using ( MemoryStream ms = new MemoryStream() ) {
				BinaryFormatter bin = new BinaryFormatter();
				bin.Serialize(ms, this);
				bytes = ms.ToArray();
			}
			
			return bytes;
		}
	}

	public enum PacketType
	{
		Login,
		Register,
		Disconnect,

		Information,

		MultiChat,
		SingleChat
	}

	public enum InformationType
	{
		Server,

		Writing,

		Joining,
		Leaving,
		Contacts
	}

	[Serializable]
	public class Information
	{
		public InformationType Type;
		public string User;
		public string Message;
		public List<string> Contacts;

		public Information(InformationType t, string user, string msg)
		{
			this.Type = t;
			this.User = user;
			this.Message = msg;
		}
		public Information(InformationType t, List<string> contacts)
		{
			this.Type = t;
			this.Contacts = contacts;
		}

		public Information(Information info)
		{
			this.Type = info.Type;
			this.User = info.User;
			this.Message = info.Message;
			this.Contacts = info.Contacts;
		}
	}

	[Serializable]
	public class User
	{
		public int ID;
		public string Username;
		public string Nickname;
		public string Email;
		public int Type;
		public byte[] Password;
		public byte[] Salt;

		public User()
		{

		}

		public User(User u)
		{
			this.Username = u.Username;
			this.Email = u.Email;
			this.Password = u.Password;
			this.Salt = u.Salt;
		}

		public User(string login, byte[] pwd)
		{
			this.Username = login;
			this.Password = pwd;
		}

		public User(string name, string mail, byte[] pwd, byte[] salt)
		{
			this.Username = name;
			this.Email = mail;
			this.Salt = salt;
			this.Password = pwd;
		}

		private DataTable AddUserColumns(DataTable tb)
		{
			tb.Columns.Add("id", typeof(int));
			tb.Columns.Add("username", typeof(string));
			tb.Columns.Add("nickname", typeof(string));
			tb.Columns.Add("email", typeof(string));
			tb.Columns.Add("type", typeof(int));
			tb.Columns.Add("pwd", typeof(byte[]));
			tb.Columns.Add("salt", typeof(byte[]));

			return tb;
		}

		public bool Login()
		{
			DataTable tb = new DataTable("user");

			string precmd = "SELECT * FROM server.users WHERE username = @username";
			bool result;

			using ( Database db = new Database() ) {
				using ( MySqlCommand cmd = new MySqlCommand(precmd, db.Connection) ) {
					cmd.Parameters.Add("@username", MySqlDbType.String);
					cmd.Parameters["@username"].Value = Username;

					tb = Database.Select(cmd);

					if ( tb.Rows.Count <= 0 ) {
						return false;
					}

					DataRow row = tb.Rows[0];

					byte[] pwd = (byte[]) row["pwd"];
					Salt = (byte[]) row["salt"];
					result = ConfirmPassword(pwd);

					if ( result ) {
						ID = (int) row["id"];
						Username = (string) row["username"];
						Nickname = (string) row["nickname"];
						Email = (string) row["email"];
						Type = (int) row["type"];
						Password = (byte[]) row["pwd"];
						Salt = (byte[]) row["salt"];
					}
				}
			}

			return result;
		}

		public bool Register()
		{
			string precmd = "INSERT INTO server.users(id, username, nickname, email, pwd, salt)" +
					"VALUES(null, @username, @username, @email, @pwd, @salt)";
			int ret = -1;
			using ( Database db = new Database() ) {
				using ( MySqlCommand cmd = new MySqlCommand(precmd, db.Connection) ) {
					cmd.Parameters.Add("@username", MySqlDbType.String);
					cmd.Parameters.Add("@email", MySqlDbType.VarChar);
					cmd.Parameters.Add("@pwd", MySqlDbType.VarBinary);
					cmd.Parameters.Add("@salt", MySqlDbType.VarBinary);

					cmd.Parameters["@username"].Value = Username;
					cmd.Parameters["@email"].Value = Email;
					cmd.Parameters["@pwd"].Value = Tools.Hash(Password, Salt);
					cmd.Parameters["@salt"].Value = Salt;

					ret = Database.Insert(cmd);

					Console.WriteLine("[Register] {0}", Username);
				}
			}

			return ret > 0;
		}

		public bool ConfirmPassword(byte[] hashedpwd)
		{
			byte[] pwdHash = Tools.Hash(Password, Salt);

			return hashedpwd.SequenceEqual(pwdHash);
		}
	}
}
