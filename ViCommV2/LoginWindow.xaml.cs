using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ViCommV2
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		public FormState state;
		private ClientManager client;

		public LoginWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			state = FormState.Null;

			tb_login.Focus();
		}

		private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				Login();
			}
		}

		private void Login_Click(object sender, RoutedEventArgs e)
		{
			Login();
		}

		private void Login()
		{
			if (tb_login.Text != "" && tb_pwd.Password != "") {
				client = ClientManager.Instance;

				client.Connect();

				if (client.Connected) {
					client.Login(tb_login.Text, Encoding.UTF8.GetBytes(tb_pwd.Password));

					state = FormState.Logging;
				}
			}
			else {
				if (tb_login.Text == "") {
					tb_login.BorderBrush = Brushes.OrangeRed;
					tb_login.BorderThickness = new Thickness(2);
				}

				if (tb_pwd.Password == "") {
					tb_pwd.BorderBrush = Brushes.OrangeRed;
					tb_pwd.BorderThickness = new Thickness(2);
				}
			}
		}

		private void tb_login_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (tb_login.Text != "") {
				tb_login.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE3E9EF"));
				tb_login.BorderThickness = new Thickness(1);
			}
			else {
				tb_login.BorderBrush = Brushes.OrangeRed;
				tb_login.BorderThickness = new Thickness(2);
			}
		}

		private void tb_pwd_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (tb_pwd.Password != "") {
				tb_pwd.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE3E9EF"));
				tb_pwd.BorderThickness = new Thickness(1);
			}
			else {
				tb_pwd.BorderBrush = Brushes.OrangeRed;
				tb_pwd.BorderThickness = new Thickness(2);
			}
		}

		public enum FormState
		{
			Null,
			Logging,
			Logged
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (state == FormState.Null) {
				FormHelper.isClosing = true;
				System.Environment.Exit(0);
			}
		}
	}
}