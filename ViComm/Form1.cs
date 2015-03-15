using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace ViComm
{
	public partial class Form1 : Form
	{
		Client client;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			client = new Client("127.0.0.1", 5555, this);

			client.Connect();
			this.txt_name.Focus();
		}

		private void inputBox_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter ) {
				e.SuppressKeyPress = true;
				if ( this.inputBox.Text.Length > 0 ) {
					string msg = this.inputBox.Text;
					this.inputBox.Text = "";
					client.Send(msg);
				}
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if ( client.GetStream != null && !client.GetClient.Connected )
				client.Disconnect();
		}

		private void btn_login_Click(object sender, EventArgs e)
		{
			client.Login(this.txt_name.Text);
			this.inputBox.Enabled = true;
			this.inputBox.Focus();
		}
	}

	struct Packet
	{
		public PacketStruct receive;

		public Packet(PacketStruct p)
		{
			receive = p;
		}

		public string PacketToString()
		{
			return JsonConvert.SerializeObject(receive, Formatting.Indented);
		}

		public PacketStruct ToPacket(string p)
		{
			receive = JsonConvert.DeserializeObject<PacketStruct>(p);
			return receive;
		}
	}

#pragma warning disable 0649
	public struct PacketStruct
	{
		public Int16 type;
		public String receiver;
		public String sender;
		public String message;
	}
#pragma warning restore 0649

	class Client
	{
		TcpClient client;
		NetworkStream stream;
		public String Name;
		private Int32 port;
		private String host;
		private string password;

		private Form1 form;

		Queue<PacketStruct> packets = new Queue<PacketStruct>();
		Byte[] data = new Byte[1024];

		public Client(string host, int port, Form1 f, string pwd = null)
		{
			this.host = host;
			this.port = port;
			this.password = pwd;
			this.form = f;

			BackgroundWorker bg = new BackgroundWorker();
			bg.DoWork += bg_DoWork;
			bg.RunWorkerAsync();
		}

		public NetworkStream GetStream
		{
			get
			{
				return this.stream;
			}
		}
		public TcpClient GetClient
		{
			get
			{
				return this.client;
			}
		}

		void bg_DoWork(object sender, DoWorkEventArgs e)
		{
			while ( true ) {
				Receive();
			}
		}

		public void Login(String name)
		{
			PacketStruct packet = new PacketStruct();
			this.Name = name;
			packet.type = 0;
			packet.sender = Name;
			packet.message = "login";

			data = Encoding.UTF8.GetBytes(new Packet(packet).PacketToString());
			stream.Write(data, 0, data.Length);
		}

		public void Connect()
		{
			try {
				client = new TcpClient(host, port);

				stream = client.GetStream();
				form.log.Text = "*Connected!";
				Receive();
			}
			catch ( ArgumentNullException e ) {
				// TODO: Save to file
				form.log.Text = String.Format("|#| ArgumentNullException: {0}", e);
			}
			catch ( SocketException e ) {
				// TODO: Save to file
				form.log.Text = String.Format("|#| SocketException: {0}", e);
			}
		}

		public void Send(String msg)
		{
			PacketStruct packet = new PacketStruct();
			packet.type = 1;
			packet.receiver = form.txt_receiver.Text;
			packet.sender = Name;
			packet.message = msg;

			// Send Parsed Packet
			data = Encoding.UTF8.GetBytes(new Packet(packet).PacketToString());
			stream.BeginWrite(data, 0, data.Length, new AsyncCallback(OnSend), stream);
			Console.WriteLine("Sending: {0} B to {1}", data.Length, packet.receiver);

			AppendText(packet);
		}

		private void OnSend(IAsyncResult result)
		{
			NetworkStream stream = (NetworkStream) result.AsyncState;

			try {
				stream.EndWrite(result);
			}
			catch ( SocketException socketException ) {
				if ( socketException.ErrorCode == 10054 ||
					( ( socketException.ErrorCode != 10004 ) &&
					( socketException.ErrorCode != 10053 ) ) ) {
					String remoteIP = ( (IPEndPoint) client.Client.RemoteEndPoint ).Address.ToString();
					String remotePort = ( (IPEndPoint) client.Client.RemoteEndPoint ).Port.ToString();
					CloseConnection();
				}
			}
			catch ( Exception exception ) {
				MessageBox.Show(exception.Message + "\n" + exception.StackTrace, Name);
				CloseConnection();
			}
		}

		public void Receive()
		{
			stream = client.GetStream();

			// Read Packet
			data = new Byte[1024];
			ContinueReceiving(stream, data, data.Length, 0);
		}

		private void ContinueReceiving(NetworkStream stream, Byte[] data, int full, int received)
		{
			stream.BeginRead(data, received, full - received, (asyncResult) => {
				try {
					full = stream.EndRead(asyncResult);
				}
				catch ( SocketException socketException ) {
					if ( socketException.ErrorCode == 10054 ||
						( ( socketException.ErrorCode != 10004 ) &&
						( socketException.ErrorCode != 10053 ) ) ) {
						String remoteIP = ( (IPEndPoint) client.Client.RemoteEndPoint ).Address.ToString();
						String remotePort = ( (IPEndPoint) client.Client.RemoteEndPoint ).Port.ToString();
						CloseConnection();
					}
				}
				catch ( Exception exception ) {
					MessageBox.Show(exception.Message + "\n" + exception.StackTrace, Name);
					CloseConnection();
				}

				if ( full == 0 ) {
					return;
				}

				received += full;
				Console.WriteLine("Size: {0} B\tReceived: {1} B", full, received);

				if ( full <= received ) {
					Received(data, full);
				}
				else {
					ContinueReceiving(stream, data, full, received);
				}
			}, null);
		}

		private void Received(Byte[] data, int size)
		{
			if ( size != 0 ) {
				PacketStruct packet = new Packet().ToPacket(Encoding.UTF8.GetString(data, 0, size));

				if ( packet.type > 0 ) {
					AppendText(packet);
				}
			}

			ContinueReceiving(stream, new Byte[1024], 1024, 0);
		}

		public delegate void SetTextCallback(PacketStruct p);
		private void AppendText(PacketStruct packet)
		{
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
			PacketStruct packet = new PacketStruct();
			packet.type = 0;
			packet.message = "disconnect";

			data = Encoding.UTF8.GetBytes(new Packet(packet).PacketToString());
			stream.Write(data, 0, data.Length);
			stream.Flush();

			CloseConnection();
		}

		private void CloseConnection()
		{
			client.Client.Disconnect(true);
			stream.Close();
			stream = null;
			client.Close();
		}
	}
}
