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

		public FormState state;

		public Form1()
		{
			InitializeComponent();
			Initialize();
		}

		private void Initialize()
		{
			// Load fonts
			FontHelper.AddFontFromResource("ViComm.Resources.Fonts.segoeui.ttf");
			FontHelper.AddFontFromResource("ViComm.Resources.Fonts.segoeuil.ttf");

			// Use fonts
			//outputBox.Font = new System.Drawing.Font(FontHelper.fonts.Families[0], 12, System.Drawing.FontStyle.Regular);
			lb_nick.Font = new System.Drawing.Font(FontHelper.fonts.Families[1], 12, System.Drawing.FontStyle.Regular);

			this.chatBox1.SetFont = new System.Drawing.Font(FontHelper.fonts.Families[0], 12, System.Drawing.FontStyle.Regular);

			// Load sounds
			Sound.AddSound(Sound.SoundType.Available, ViResource.s_available);
			Sound.AddSound(Sound.SoundType.Message, ViResource.s_message);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Connect();

			// Display user nickname
			lb_nick.Text = "Nick: " + client.user.Nickname;

			state = FormState.Exit;
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
				enterPressed = true;
				e.SuppressKeyPress = true;
				if ( this.inputBox.Text.Length > 0 ) {
					string msg = this.inputBox.Text;
					this.inputBox.Text = "";
					client.Send(msg);
				}
			}
			else {
				enterPressed = false;
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

			state = FormState.Null;
			Application.Exit();
		}

		private void Logout()
		{
			if ( client != null ) {
				client.Disconnect(false);
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if ( state == FormState.Logout ) {
				Logout();
			}
			else if ( state == FormState.Close ) {

			}
			else if ( state == FormState.Exit ) {
				Exit();
			}
		}

		public enum FormState
		{
			Null,
			Exit,
			Close,
			Logout
		}

		System.Windows.Forms.Timer t_writing;
		private double timeElapsed = 0;
		private bool tStarted = false;
		private bool enterPressed = false;
		private void inputBox_TextChanged(object sender, EventArgs e)
		{
			if ( enterPressed == false ) {
				timeElapsed = 0;

				if ( tStarted == false ) {
					tStarted = true;
					t_writing = new System.Windows.Forms.Timer();
					t_writing.Interval = 100;
					t_writing.Tick += t_writing_Tick;
					t_writing.Start();

					Packet p = new Packet(PacketType.Information);
					p.info = new Information(InformationType.Writing, client.user.Nickname, "started");
					client.Send(p);
				}
			}
		}

		void t_writing_Tick(object sender, EventArgs e)
		{
			if ( ( timeElapsed >= 1000 ) || enterPressed ) {
				t_writing.Stop();
				t_writing.Dispose();
				t_writing = null;
				tStarted = false;

				Packet p = new Packet(PacketType.Information);
				p.info = new Information(InformationType.Writing, client.user.Nickname, "stopped");
				client.Send(p);
			}
			else {
				timeElapsed += 100;
			}
		}

	}
}
