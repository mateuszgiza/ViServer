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
			client = new Client("127.0.0.1", 5555);
			client.SetAccesibles(ref this.log, ref this.inputBox, ref this.outputBox);
		}

		private void inputBox_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter ) {
				e.SuppressKeyPress = true;
				if ( this.inputBox.Text.Length > 0 ) {
					string msg = this.inputBox.Text;
					this.inputBox.Text = "";
					client.Connect(msg);
				}
			}
		}
	}

	class Client
	{
		TcpClient client;
		private Int32 port;
		private String host;
		private string password;

		private Label log;
		private TextBox inputBox;
		private RichTextBox outputBox;

		

		public Client(string host, int port, string pwd = null)
		{
			this.host = host;
			this.port = port;
			this.password = pwd;
		}

		public void Connect(string msg)
		{
			try {
				client = new TcpClient(host, port);

				Byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);

				NetworkStream stream = client.GetStream();
				log.Text = "*Connected!";

				stream.Write(data, 0, data.Length);

				outputBox.AppendText(String.Format("* Sent: {0}\n", msg));
				outputBox.ScrollToCaret();

				data = new Byte[256];

				String responseData = String.Empty;

				Int32 bytes = stream.Read(data, 0, data.Length);
				responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
				outputBox.AppendText(String.Format("* Received: {0}\n", responseData));
				outputBox.ScrollToCaret();

				stream.Close();
				client.Close();
			}
			catch (ArgumentNullException e) {
				log.Text = String.Format("|#| ArgumentNullException: {0}", e);
			}
			catch (SocketException e) {
				log.Text = String.Format("|#| SocketException: {0}", e);
			}
		}

		public void SetAccesibles(ref Label l, ref TextBox i, ref RichTextBox o)
		{
			this.log = l;
			this.inputBox = i;
			this.outputBox = o;
		}
	}
}
