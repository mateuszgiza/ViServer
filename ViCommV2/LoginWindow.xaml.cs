using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ViCommV2.Classes;

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public enum FormState
        {
            Null,
            Logging,
            Logged
        }

        private ClientManager _client;
        public FormState State;

        public LoginWindow()
        {
            InitializeComponent();
            InitializeEvents();

            WindowStickManager.AddWindow(this);
        }

        public void InitializeEvents()
        {
            var helper = FormHelper.Instance;

            bt_Login.Click += (sender, e) => { Login(); };

            lb_Register.MouseLeftButtonUp += (sender, e) => {
                helper.Register = new RegisterWindow();
                helper.Register.Show();
                State = FormState.Logging;
                Close();
            };

            lb_Register.MouseEnter += (sender, e) => { lb_Register.TextDecorations = TextDecorations.Underline; };
            lb_Register.MouseLeave += (sender, e) => { lb_Register.TextDecorations = null; };

            tb_login.KeyDown += (sender, e) => {
                if (e.Key == Key.Enter) {
                    Login();
                }
            };

            tb_pwd.KeyDown += (sender, e) => {
                if (e.Key == Key.Enter) {
                    Login();
                }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            State = FormState.Null;
            tb_login.Focus();
        }

        private void Login()
        {
            if (tb_login.Text != "" && tb_pwd.Password != "") {
                _client = ClientManager.Instance;

                _client.Connect();

                if (!_client.Connected) {
                    return;
                }
                _client.Login(tb_login.Text, Encoding.UTF8.GetBytes(tb_pwd.Password));

                State = FormState.Logging;
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
                tb_login.BorderBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFE3E9EF"));
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
                tb_pwd.BorderBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFE3E9EF"));
                tb_pwd.BorderThickness = new Thickness(1);
            }
            else {
                tb_pwd.BorderBrush = Brushes.OrangeRed;
                tb_pwd.BorderThickness = new Thickness(2);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (State != FormState.Null) {
                return;
            }

            FormHelper.IsClosing = true;
            Environment.Exit(0);
        }
    }
}