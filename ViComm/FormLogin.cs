using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ViComm
{
	public partial class FormLogin : Form
	{
		private Client client;

		private bool _logged = false;

		public FormLogin()
		{
			InitializeComponent();
		}

		private void FormLogin_Load(object sender, EventArgs e)
		{
			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string name = tb_login.Text;
			string pwd = tb_pwd.Text;

			if ( name != "" && pwd != "" ) {
				client = Client.GetInstance();

				client.Connect();
				client.Login(name, Encoding.UTF8.GetBytes(pwd));

				_logged = true;
			}
			else {
				MessageBox.Show("Enter all credentials!", "Login");
			}
		}

		private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
		{
			if ( !_logged ) {
				Application.Exit();
			}
		}

		private void tb_pwd_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter ) {
				e.SuppressKeyPress = true;
				button1.PerformClick();
			}
		}

		private void lb_register_Click(object sender, EventArgs e)
		{
			client = Client.GetInstance();
			client.forms.form_register = new FormRegister();
			client.forms.InvokeIfRequired(() => client.forms.form_register.Show());

			_logged = true;

			this.Close();
		}

		private void lb_register_MouseEnter(object sender, EventArgs e)
		{
			lb_register.Font = new Font(lb_register.Font, FontStyle.Underline);
		}

		private void lb_register_MouseLeave(object sender, EventArgs e)
		{
			lb_register.Font = new Font(lb_register.Font, FontStyle.Regular);
		}
	}
}
