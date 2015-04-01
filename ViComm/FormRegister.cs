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
	public partial class FormRegister : Form
	{
		private Client client;

		public FormRegister()
		{
			InitializeComponent();
		}

		private void FormRegister_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			string name = tb_login.Text;
			string mail = tb_mail.Text;
			string pwd = tb_pwd.Text;
			string repwd = tb_repwd.Text;

			if ( name != null && mail != null && pwd != null && repwd != null ) {
				if ( pwd.Equals(repwd) ) {
					client = Client.GetInstance();

					client.Connect();
					client.Register(name, mail, Encoding.UTF8.GetBytes(pwd));
				}
				else {
					MessageBox.Show("You entered different passwords!", "Register");
				}
			}
			else {
				MessageBox.Show("Enter all credentials!", "Register");
			}
		}

		private void FormRegister_FormClosing(object sender, FormClosingEventArgs e)
		{

		}
	}
}
