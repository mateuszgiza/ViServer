using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ViData;

namespace ViComm
{
	public class Client
	{
		private Socket socket;
		private IPEndPoint ip;
		private string guid;
		public User user;

		public FormHelper forms = FormHelper.GetInstance();

		private static Client _Instance = null;
		public static Client GetInstance()
		{
			if ( _Instance == null ) {
				_Instance = new Client(Tools.GetIP(), 5555);
			}

			return _Instance;
		}

		public Client(string host, int port)
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.ip = new IPEndPoint(IPAddress.Parse(host), port);
		}

		public void Connect()
		{
			if ( socket.Connected ) {
				return;
			}

			try {
				socket.Connect(ip);

				//form.log.Text = "*Connected!";
				rec = new Thread(Receive);
				rec.Start();
			}
			catch {
				MessageBox.Show("Could not connect to server!", "Connecting error!");
				//form.log.Text = "Could not connect to server!";
				Console.WriteLine("Could not connect to server!");
			}
		}

		public void Login(string login, byte[] pwd)
		{
			Packet p = new Packet(PacketType.Login);
			p.user = new User(login, pwd);
			socket.Send(p.ToBytes());
		}

		public void LoginResult(Packet p)
		{
			string msg = p.message;

			if ( p.user != null ) {
				forms.form = new Form1();

				if ( forms.form_login != null ) {
					forms.InvokeIfRequired(() => forms.form_login.Close());
				}
				guid = p.message;
				user = p.user;

				//form.btn_login.Text = "Logout";
				forms.InvokeIfRequired((value) => forms.form.btn_login.Text = value, "Logout");
				forms.InvokeIfRequired(() => forms.form.Show());
			}
			else {
				MessageBox.Show(msg, "Login error!");
			}
		}

		public void Register(string name, string email, byte[] pwd)
		{
			Packet p = new Packet(PacketType.Register);
			p.user = new User(name, email, pwd, Tools.GenerateSalt());

			socket.Send(p.ToBytes());
		}
		public void RegisterResult(Packet p)
		{
			string result = p.message[0].ToString();
			string msg = p.message.Substring(1);
			// Result of registration
			MessageBox.Show(msg, "Register");
			if ( result == "1") {
				Login(p.user.Username, p.user.Password);
				forms.InvokeIfRequired(() => forms.form_register.Close());
			}
		}

		public void Send(String msg)
		{
			Packet p = new Packet(PacketType.MultiChat);
			p.sender = user.Nickname;
			p.message = msg;

			socket.Send(p.ToBytes());
		}

		Thread rec;
		public void Receive()
		{
			byte[] buffer;
			int readBytes;

			while ( true ) {
				try {
					buffer = new byte[socket.SendBufferSize];
					readBytes = socket.Receive(buffer);

					if ( readBytes > 0 ) {
						Received(new Packet(buffer));
					}
				}
				catch ( SocketException e ) {
					Console.WriteLine("Server lost!");
					Console.WriteLine(e);
				}
			}
		}

		private void Received(Packet p)
		{
			switch ( p.type ) {
				case PacketType.Login:
					LoginResult(p);
					break;
				case PacketType.Disconnect:
					// Reserved
					break;
				case PacketType.Register:
					RegisterResult(p);
					break;
				case PacketType.SingleChat:
					// Reserved
					break;
				case PacketType.MultiChat:
					AppendText(p);
					break;
				default:

					break;
			}
		}

		public delegate void SetTextCallback(Packet p);
		private void AppendText(Packet packet)
		{
			Form1 form = forms.form;

			if ( form.outputBox.InvokeRequired ) {
				SetTextCallback callb = new SetTextCallback(AppendText);
				form.Invoke(callb, new object[] { packet });
			}
			else {
				if ( form.outputBox.TextLength == 0 ) {
					form.outputBox.AppendText(String.Format("{0}: {1}", packet.sender, packet.message));
				}
				else {
					form.outputBox.AppendText(String.Format("\n{0}: {1}", packet.sender, packet.message));
				}
				form.outputBox.ScrollToCaret();
			}
		}

		public void Disconnect()
		{
			if ( socket.Connected ) {
				Packet p = new Packet(PacketType.Disconnect);
				p.message = guid;
				socket.Send(p.ToBytes());

				CloseConnection();
			}
		}

		private void CloseConnection()
		{
			rec.Abort();
			rec = null;
			socket.Close();
			_Instance = null;
		}
	}
}
