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
	public static class ControlExtensions
	{
		public static void InvokeIfRequired(this Control control, Action action)
		{
			if ( control.InvokeRequired )
				control.Invoke(action);
			else
				action();
		}
		public static void InvokeIfRequired<T>(this Control control, Action<T> action, T parameter)
		{
			if ( control.InvokeRequired )
				control.Invoke(action, parameter);
			else
				action(parameter);
		}
	}

	public partial class FormHelper : Form
	{
		public Form1 form;
		public FormLogin form_login;
		public FormRegister form_register;

		private FormHelper()
		{
			InitializeComponent();
		}

		private static FormHelper _Instance = null;
		public static FormHelper GetInstance()
		{
			if ( _Instance == null ) {
				_Instance = new FormHelper();
			}

			return _Instance;
		}

		private void FormHelper_Load(object sender, EventArgs e)
		{
			form_login = new FormLogin();
			form_login.Show();
		}
	}
}
