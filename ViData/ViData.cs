using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace ViData
{
	public class Tools
	{
		private Tools() { }

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
			var path = GetStartupPath() + @"\" + file;
		    if (File.Exists(path)) {
		        return;
		    }

		    File.Create(path).Dispose();
		    TextWriter tw = new StreamWriter(path);
		    tw.WriteLine(line);
		    tw.Close();
		}

		public static string ReadLineTxt(string file)
		{
			var line = "";
			var path = GetStartupPath() + @"\" + file;
		    if (!File.Exists(path)) {
		        return line;
		    }

		    var f = new StreamReader(path);
		    line = f.ReadLine();
		    f.Close();

		    return line;
		}

		public static string GetIp()
		{
		    var ip = ReadLineTxt("ip.txt");

		    return ip.Length > 0 ? ip : (GetIpFromHostname("viserver.noip.pl"));
		}

	    public static string GetIPv4()
		{
			var ips = Dns.GetHostAddresses(Dns.GetHostName());

			foreach (var ip in ips.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)) {
			    return ip.ToString();
			}

			return "127.0.0.1";
		}

		public static string GetIpFromHostname(string host)
		{
			return Dns.GetHostAddresses(host)[0].ToString();
		}

		public static byte[] GenerateSalt()
		{
			var rng = new RNGCryptoServiceProvider();
			var buff = new byte[128 / 8]; // 128 bits
			rng.GetBytes(buff);

			return buff;
		}

		public static byte[] Hash(string value, byte[] salt)
		{
			return Hash(Encoding.UTF8.GetBytes(value), salt);
		}

		public static byte[] Hash(byte[] value, byte[] salt)
		{
			var saltedValue = value.Concat(salt).ToArray();

			return new SHA256Managed().ComputeHash(saltedValue);
		}
	}

	public class Database : IDisposable
	{
	    private MySqlCommand _cmd;
		private MySqlDataReader _reader;
		//MySqlDataAdapter adapter;

		public string Query;

		public Database()
			: this(false)
		{
		}

		public Database(bool start)
		{
		    Connection = new MySqlConnection {
		        ConnectionString = "server=127.0.0.1;uid=ViServer;" +
		                           "pwd=lubiemaslo;database=server;Charset=utf8;"
		    };

		    if (start)
				Connect();
		}

		public void Connect()
		{
			try {
				Connection.Open();
			}
			catch (MySqlException ex) {
				Exception(ex);
			}
		}

		public MySqlConnection Connection { get; private set; }

	    public static DataTable Select(MySqlCommand cmd)
		{
			var tb = new DataTable();
			var adapter = new MySqlDataAdapter(cmd);

			using (cmd) {
				try {
					if (cmd.Connection.State == ConnectionState.Closed) {
						cmd.Connection.Open();
					}

					adapter.Fill(tb);

					cmd.Connection.Close();
				}
				catch (MySqlException ex) {
					Exception(ex);
				}
			}

			return tb;
		}

		public static int Insert(MySqlCommand cmd)
		{
			var ret = -1;
			using (cmd) {
				try {
					if (cmd.Connection.State == ConnectionState.Closed) {
						cmd.Connection.Open();
					}

					ret = cmd.ExecuteNonQuery();

					cmd.Connection.Close();
				}
				catch (MySqlException ex) {
					Exception(ex);
				}
			}

			return ret;
		}

		public static int Exception(MySqlException ex)
		{
			const int id = -1;

			switch (ex.Number) {
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
			Connection.Close();
		}

		public void Dispose()
		{
			if (_reader != null) {
				_reader.Dispose();
				_reader = null;
			}

			if (_cmd != null) {
				_cmd.Dispose();
				_cmd = null;
			}

		    if (Connection == null) {
		        return;
		    }

		    Connection.Dispose();
		    Connection = null;
		}
	}

	[Serializable]
	public class Packet
	{
		public PacketType Type;
		public User User;
		public Information Information;
		public DateTime Date;
		public string Sender;
		public string Receiver;
		public string Message;

		public Packet()
		{
		}

		public Packet(PacketType type)
		{
			Type = type;
		}

		public Packet(PacketType type, string msg)
		{
			Type = type;
			Message = msg;
		}

		public Packet(PacketType type, string sender, string msg)
		{
			Type = type;
			Sender = sender;
			Message = msg;
		}

		public Packet(PacketType type, User u, string msg)
		{
			Type = type;
			User = u;
			Message = msg;
		}

		public Packet(PacketType type, Information i)
		{
			Type = type;
			Information = i;
		}

		public Packet(byte[] buffer)
		{
			var deser = Encoding.UTF8.GetString(buffer);
			//Console.WriteLine(deser);
			var p = JsonConvert.DeserializeObject<Packet>(deser);

			//using (MemoryStream ms = new MemoryStream(buffer)) {
			//	BinaryFormatter bin = new BinaryFormatter();
			//	p = (Packet)bin.Deserialize(ms);
			//}

			Type = p.Type;
			User = p.User;
			Information = p.Information;
			Date = p.Date;
			Sender = p.Sender;
			Receiver = p.Receiver;
			Message = p.Message;
		}

		public byte[] ToBytes()
		{
			//byte[] bytes;

			//using (MemoryStream ms = new MemoryStream()) {
			//	BinaryFormatter bin = new BinaryFormatter();
			//	bin.Serialize(ms, this);
			//	bytes = ms.ToArray();
			//}

			var json = JsonConvert.SerializeObject(this);

			return Encoding.UTF8.GetBytes(json);
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

		public Information()
		{
		}

		public Information(InformationType t, string user, string msg)
		{
			Type = t;
			User = user;
			Message = msg;
		}

		public Information(InformationType t, List<string> contacts)
		{
			Type = t;
			Contacts = contacts;
		}

		public Information(Information info)
		{
			Type = info.Type;
			User = info.User;
			Message = info.Message;
			Contacts = info.Contacts;
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
		public string AvatarUri;
		public string NickColor;

		public User()
		{
		}

		public User(User u)
		{
			Username = u.Username;
			Email = u.Email;
			Password = u.Password;
			Salt = u.Salt;
		}

		public User(string login, byte[] pwd)
		{
			Username = login;
			Password = pwd;
		}

		public User(string name, string mail, byte[] pwd, byte[] salt)
		{
			Username = name;
			Email = mail;
			Salt = salt;
			Password = pwd;
		}

		public User(string avatarUri, string nickColor)
		{
			AvatarUri = avatarUri;
			NickColor = nickColor;
		}

	    public bool Login()
		{
	        const string precmd = "SELECT * FROM server.users WHERE username = @username";

	        using (var db = new Database()) {
				using (var cmd = new MySqlCommand(precmd, db.Connection)) {
					cmd.Parameters.Add("@username", MySqlDbType.String);
					cmd.Parameters["@username"].Value = Username;

					var tb = Database.Select(cmd);

					if (tb.Rows.Count <= 0) {
						return false;
					}

					var row = tb.Rows[0];

					var pwd = (byte[])row["pwd"];
					Salt = (byte[])row["salt"];
					var result = ConfirmPassword(pwd);

				    if (!result) {
				        return false;
				    }

				    ID = (int)row["id"];
				    Username = (string)row["username"];
				    Nickname = (string)row["nickname"];
				    Email = (string)row["email"];
				    Type = (int)row["type"];
				    Password = (byte[])row["pwd"];
				    Salt = (byte[])row["salt"];
				    AvatarUri = (string)row["avatar"];
				    NickColor = (string)row["nick_color"];
				}
			}

			return true;
		}

		public bool Register()
		{
			const string precmd = "INSERT INTO server.users(id, username, nickname, email, pwd, salt)" +
			                      "VALUES(null, @username, @username, @email, @pwd, @salt)";
			int ret;
			using (var db = new Database()) {
				using (var cmd = new MySqlCommand(precmd, db.Connection)) {
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
			var pwdHash = Tools.Hash(Password, Salt);

			return hashedpwd.SequenceEqual(pwdHash);
		}
	}
}