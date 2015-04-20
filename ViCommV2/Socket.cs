using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using ViData;

namespace ViCommV2
{
	public class ClientManager : IDisposable
	{
		private Socket socket;
		private IPEndPoint ip;
		private string guid;
		private bool _connected;

		public User user;

		public FormHelper forms = FormHelper.GetInstance();

		private static ClientManager _Instance = null;

		public static ClientManager GetInstance()
		{
			if (_Instance == null) {
				_Instance = new ClientManager(Tools.GetIP(), 5555);
			}

			return _Instance;
		}

		public ClientManager(string host, int port)
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.ip = new IPEndPoint(IPAddress.Parse(host), port);
		}

		public void Connect()
		{
			if (socket.Connected) {
				_connected = true;
				return;
			}

			try {
				socket.Connect(ip);
				_connected = true;
				StartReceiving();
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
			Send(p);
		}

		public void LoginResult(Packet p)
		{
			string msg = p.message;

			if (p.user != null) {
				guid = p.message;
				user = p.user;
				string[] contacts = p.info.Contacts.ToArray();

				forms.Dispatcher.Invoke(new Action(() => {
					forms.Main = new MainWindow();

					if (forms.Login != null) {
						forms.Login.state = LoginWindow.FormState.Logged;
						forms.Login.Close();
					}

					foreach (string c in contacts) {
						forms.Main.ListContacts.Items.Add(c);
					}

					forms.Main.Show();
				}));
			}
			else {
				MessageBox.Show(msg, "Login error!");
			}
		}

		public void Register(string name, string email, byte[] pwd)
		{
			Packet p = new Packet(PacketType.Register);
			p.user = new User(name, email, pwd, Tools.GenerateSalt());

			Send(p);
		}

		public void RegisterResult(Packet p)
		{
			string result = p.message[0].ToString();
			string msg = p.message.Substring(1);

			// Result of registration
			MessageBox.Show(msg, "Register");
			if (result == "1") {
				Login(p.user.Username, p.user.Password);
				forms.Dispatcher.Invoke(new Action(() => {
					forms.Register._registered = true;
					forms.Register.Close();
				}));
			}
		}

		public void Send(String msg)
		{
			Packet p = new Packet(PacketType.MultiChat);
			p.date = DateTime.Now;
			p.sender = user.Nickname;
			p.message = msg;

			Send(p);
		}

		public void Send(Packet p)
		{
			try {
				socket.Send(p.ToBytes());
			}
			catch (SocketException e) {
				App.HandleException(e);
			}
			catch (Exception e) {
				App.HandleException(e);
			}
		}

		private byte[] buffer;

		public void StartReceiving()
		{
			FormHelper.isClosing = false;
			buffer = new byte[8192];

			socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
		}

		private void OnReceive(IAsyncResult ar)
		{
			try {
				if (FormHelper.isClosing) {
					return;
				}

				int readBytes = socket.EndReceive(ar);

				if (readBytes > 0) {
					Received(new Packet(buffer));
				}

				buffer = new byte[8192];
				socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
			}
			catch (ObjectDisposedException) {
			}
			catch (SocketException e) {
				App.HandleException(e);
			}
			catch (Exception e) {
				App.HandleException(e);
			}
		}

		private void Received(Packet p)
		{
			switch (p.type) {
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
			MainWindow main = forms.Main;

			if (p.info.Type == InformationType.Joining) {
				AddItemToListBox(main.ListContacts, p.info.User);
				AppendText(p);
			}
			else if (p.info.Type == InformationType.Leaving) {
				RemoveItemFromListBox(main.ListContacts, p.info.User);
				AppendText(p);
			}
			else if (p.info.Type == InformationType.Writing) {
				if (p.info.Message == "started") {
					ChangeItemInListBox(main.ListContacts, p.info.User, "*| " + p.info.User);
				}
				else {
					ChangeItemInListBox(main.ListContacts, "*| " + p.info.User, p.info.User);
				}
			}
		}

		public void AddItemToListBox(ListBox list, object o)
		{
			forms.Dispatcher.Invoke(new Action(() => {
				list.Items.Add(o);
			}));
		}

		public void RemoveItemFromListBox(ListBox list, object o)
		{
			forms.Dispatcher.Invoke(new Action(() => {
				if (list.Items.Contains(o)) {
					list.Items.Remove(o);
				}
			}));
		}

		public void ChangeItemInListBox(ListBox list, object o, object changed)
		{
			forms.Dispatcher.Invoke(new Action(() => {
				if (list.Items.Contains(o)) {
					int i = list.Items.IndexOf(o);
					list.Items[i] = changed;
				}
			}));
		}

		private void AppendText(Packet packet)
		{
			forms.Dispatcher.Invoke(new Action(() => {
				MainWindow main = forms.Main;
				Sound sound = new Sound();

				string sender;
				string msg;
				DateTime date = DateTime.Now;

				if (packet.type == PacketType.Information) {
					sound.Play(Sound.SoundType.Available);

					msg = packet.info.User + " " + packet.info.Message;
					main.AppendInfoText(String.Format("* {0}", msg));
				}
				else {
					sender = packet.sender;
					msg = packet.message;
					date = packet.date.ToLocalTime();
					MainWindow.RowType rowType;

					if (sender != user.Nickname) {
						rowType = MainWindow.RowType.Sender;
						sound.Play(Sound.SoundType.Message);
					}
					else {
						rowType = MainWindow.RowType.User;
					}

					main.AppendText(sender, msg, date, rowType);
				}
			}));
		}

		public void Disconnect(Packet p)
		{
			MessageBox.Show(p.message, p.sender);

			forms.Dispatcher.Invoke(new Action(() => {
				forms.Login = new LoginWindow();
				forms.Login.Show();

				if (forms.Main != null) {
					forms.Main.State = ViCommV2.MainWindow.FormState.Close;
					forms.Main.CloseWindow();
				}
			}));

			Disconnect(true);
		}

		public void Disconnect(bool server)
		{
			if (server == false) {
				if (socket.Connected) {
					Packet p = new Packet(PacketType.Disconnect);
					p.message = guid;
					socket.Send(p.ToBytes());
				}
			}

			CloseConnection();
		}

		private void CloseConnection()
		{
			if (socket != null) {
				socket.Close();
			}

			_Instance = null;
		}

		#region Dispose Implementation

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ClientManager()
		{
			Dispose(false);
		}

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing) {
				// Free any other managed objects here.
				socket.Dispose();
			}
			// Free any unmanaged objects here.

			_disposed = true;
		}

		#endregion Dispose Implementation
	}
}