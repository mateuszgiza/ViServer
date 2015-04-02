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
		public Client client;
		public FormLogin login;
		public FormRegister register;

		private FormState state;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			state = FormState.Exit;

			Connect();

			// Display user nickname
			lb_nick.Text = "Nick: " + client.user.Nickname;
		}

		public void Connect()
		{
			if ( client != null ) {
				return;
			}

			client = Client.GetInstance();
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

		private void btn_login_Click(object sender, EventArgs e)
		{
			state = FormState.Logout;

			client.forms.form_login = new FormLogin();
			client.forms.InvokeIfRequired(() => client.forms.form_login.Show());

			this.Close();
		}

		public void CloseWindow()
		{
			state = FormState.Close;
			this.Close();
		}

		public void Exit()
		{
			Logout();
			Application.Exit();
		}

		private void Logout()
		{
			if ( client != null ) {
				client.Disconnect();
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (state == FormState.Logout) {
				Logout();
			}
			else if ( state == FormState.Close ) {
				
			}
			else {
				Exit();	
			}
		}

		private enum FormState
		{
			Exit,
			Close,
			Logout
		}
	}
}
