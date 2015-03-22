using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using ViData;
using System.Threading;

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
			this.txt_name.Focus();

			client = new Client(Tools.GetIP(), 5555, this);
			client.Connect();
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
			if ( client != null ) {
				client.Disconnect();
			}
		}

		private void btn_login_Click(object sender, EventArgs e)
		{
			client.Login(this.txt_name.Text);
			this.inputBox.Enabled = true;
			this.inputBox.Focus();
		}

		private void txt_name_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter ) {
				e.SuppressKeyPress = true;
				if ( this.txt_name.Text.Length > 0 ) {
					client.Login(this.txt_name.Text);
					this.inputBox.Enabled = true;
					this.inputBox.Focus();
				}
			}
		}
	}

	class Client
	{
		private Socket socket;
		private IPEndPoint ip;
		private string guid;
		public String Name;

		private Form1 form;

		public Client(string host, int port, Form1 f)
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.ip = new IPEndPoint(IPAddress.Parse(host), port);
			this.form = f;
		}

		public void Login(String name)
		{
			this.Name = name;

			Packet p = new Packet(PacketType.Login);
			p.sender = Name;
			socket.Send(p.ToBytes());
		}

		public void Connect()
		{
			try {
				socket.Connect(ip);

				form.log.Text = "*Connected!";
				rec = new Thread(Receive);
				rec.Start();
			}
			catch {
				form.log.Text = "Could not connect to server!";
				Console.WriteLine("Could not connect to server!");
			}
		}

		public void Send(String msg)
		{
			Packet p = new Packet(PacketType.MultiChat);
			p.sender = Name;
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
					guid = p.message;
					break;
				case PacketType.Disconnect:
					// Reserved
					break;
				case PacketType.Register:
					// Reserved
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
		}
	}
}
