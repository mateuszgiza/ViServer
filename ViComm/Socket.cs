using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViData;

namespace ViComm
{
	public class Client
	{
		private Socket socket;
		private IPEndPoint ip;
		private string guid;
		private bool _connected;

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
				_connected = true;
				return;
			}

			try {
				socket.Connect(ip);
				_connected = true;
				rec = new Thread(Receive);
				rec.Start();
			}
			catch {
				_connected = false;
				MessageBox.Show("Could not connect to server!", "Connecting error!");
				Console.WriteLine("Could not connect to server!");
			}
		}

		public bool Connected
		{
			get
			{
				return _connected;
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

				string[] contacts = p.info.Contacts.ToArray();

				forms.InvokeIfRequired((value) => forms.form.list_contacts.Items.AddRange(value), contacts);
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
			if ( result == "1" ) {
				Login(p.user.Username, p.user.Password);
				forms.InvokeIfRequired((value) => forms.form_register._registered = value, true);
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

		public void Send(Packet p)
		{
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
					Disconnect(p);
					break;
				case PacketType.Register:
					RegisterResult(p);
					break;

				case PacketType.Information:
					Information(p);
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

		private void Information(Packet p)
		{
			Form1 form = forms.form;

			if ( p.info.Type == InformationType.Joining ) {
				AddItemToListBox(forms.form.list_contacts, p.info.User);
				AppendText(p);
			}
			else if ( p.info.Type == InformationType.Leaving ) {
				RemoveItemFromListBox(forms.form.list_contacts, p.info.User);
				AppendText(p);
			}
			else if ( p.info.Type == InformationType.Writing ) {
				if ( p.info.Message == "started" ) {
					ChangeItemInListBox(forms.form.list_contacts, p.info.User, "[*]" + p.info.User);
				}
				else {
					ChangeItemInListBox(forms.form.list_contacts, "[*]" + p.info.User, p.info.User);
				}
			}
		}

		public void AddItemToListBox(ListBox list, object o)
		{
			forms.InvokeIfRequired((value) => list.Items.Add(value), o);
		}

		public void RemoveItemFromListBox(ListBox list, object o)
		{
			if ( list.Items.Contains(o) ) {
				forms.InvokeIfRequired((value) => list.Items.Remove(value), o);
			}
		}

		public void ChangeItemInListBox(ListBox list, object o, object changed)
		{
			if ( list.Items.Contains(o) ) {
				int i = list.Items.IndexOf(o);
				forms.InvokeIfRequired((value) => list.Items[i] = value, changed);
			}
		}

		public delegate void SetTextCallback(Packet p);
		private void AppendText(Packet packet)
		{
			Form1 form = forms.form;
			Sound sound = new Sound();

			if ( form.chatBox1.InvokeRequired ) {
				SetTextCallback callb = new SetTextCallback(AppendText);
				form.Invoke(callb, new object[] { packet });
			}
			else {
				string format;
				string sender;
				string msg;

				if ( packet.type == PacketType.Information ) {
					format = "{0} {1}";
					sender = "*";
					msg = packet.info.User + " " + packet.info.Message;

					sound.Play(Sound.SoundType.Available);
				}
				else {
					format = "{0}: {1}";
					sender = packet.sender;
					msg = packet.message;

					if ( sender != user.Nickname ) {
						sound.Play(Sound.SoundType.Message);
						form.chatBox1.backColor = Color.FromArgb(50, Color.Black);
					}
					else {
						form.chatBox1.backColor = Color.FromArgb(50, Color.Gray);
					}
				}

				form.chatBox1.Add(String.Format(format, sender, msg));
				form.chatBox1.ScrollToCarret();
				form.chatBox1.backColor = Color.Transparent;
			}
		}

		public void Disconnect(Packet p)
		{
			MessageBox.Show(p.message, p.sender);

			forms.form_login = new FormLogin();
			forms.InvokeIfRequired(() => forms.form_login.Show());

			if ( forms.form != null ) {
				forms.InvokeIfRequired((value) => forms.form.state = value, ViComm.Form1.FormState.Logout);
				forms.InvokeIfRequired(() => forms.form.CloseWindow());
			}

			Disconnect(true);
		}

		public void Disconnect(bool server)
		{
			if ( server == false ) {
				if ( socket.Connected ) {
					Packet p = new Packet(PacketType.Disconnect);
					p.message = guid;
					socket.Send(p.ToBytes());
				}
			}

			CloseConnection();
		}

		private void CloseConnection()
		{
			if ( rec != null ) {
				rec.Abort();
				rec = null;
			}

			if ( socket != null ) {
				socket.Dispose();
				socket = null;
			}

			_Instance = null;
		}
	}
}
