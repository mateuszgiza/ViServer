using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ViCommV2.Classes;

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly DispatcherTimer timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 100)};
        private bool _lock;
        private ClientManager client;
        private int numErrors;

        public RegisterWindow()
        {
            InitializeComponent();
            InitializeEvents();

            DataContext = new RegisterWindowViewModel();

            bt_register.IsEnabled = false;

            var stick = new WindowStickManager(this);
        }

        public bool Registered { get; set; }

        private void InitializeEvents()
        {
            bt_register.Click += (sender, e) => { Register(); };
            tb_name.KeyDown += (sender, e) => {
                if (e.Key == Key.Enter) {
                    Register();
                }
            };
            tb_mail.KeyDown += (sender, e) => {
                if (e.Key == Key.Enter) {
                    Register();
                }
            };
            tb_pwd.KeyDown += (sender, e) => {
                if (e.Key == Key.Enter) {
                    Register();
                }
            };
            tb_repwd.KeyDown += (sender, e) => {
                if (e.Key == Key.Enter) {
                    Register();
                }
            };
        }

        private void Register()
        {
            var LoginName = tb_name.Text;
            var Mail = tb_mail.Text;
            var Password = tb_pwd.Password;
            var RePassword = tb_repwd.Password;

            if (LoginName != "" && Mail != "" && Password != "" && RePassword != "") {
                if (Password.Equals(RePassword)) {
                    client = ClientManager.Instance;

                    client.Connect();
                    client.Register(LoginName, Mail, Encoding.UTF8.GetBytes(Password));

                    Registered = true;
                }
                else {
                    MessageBox.Show("You entered different passwords!", "Register");
                }
            }
            else {
                MessageBox.Show("Enter all credentials!", "Register");
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!Registered) {
                var helper = FormHelper.Instance;
                helper.Login = new LoginWindow();
                helper.Login.Show();
            }
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) {
                numErrors++;
            }
            else {
                numErrors--;
            }

            if (tb_name.Text.Length > 0 && tb_mail.Text.Length > 0 && tb_pwd.Password.Length > 0 &&
                tb_repwd.Password.Length > 0) {
                bt_register.IsEnabled = numErrors == 0;
            }
        }

        private void Password_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (_lock == false) {
                timer.Start();
                _lock = true;
                timer.Tick += (s, ev) => {
                    _lock = false;
                    timer.Stop();
                };
                tb_repwd.GetBindingExpression(PasswordHelper.PasswordProperty).UpdateSource();
            }
        }

        private void RePassword_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (_lock == false) {
                timer.Start();
                _lock = true;
                timer.Tick += (s, ev) => {
                    _lock = false;
                    timer.Stop();
                };
                tb_pwd.GetBindingExpression(PasswordHelper.PasswordProperty).UpdateSource();
            }
        }
    }

    public class RegisterWindowViewModel : IDataErrorInfo
    {
        private readonly Regex EmailRegex =
            new Regex(@"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");

        //property to bind to textbox
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }

        #region IDataErrorInfo

        //In this region we are implementing the properties defined in //the IDataErrorInfo interface in System.ComponentModel

        public string this[string columnName] {
            get {
                if (columnName == "Email") {
                    if (Email != null) {
                        if (!EmailRegex.Match(Email).Success) {
                            return "Not a valid e-mail address!";
                        }
                    }
                }
                else if (columnName == "Password" || columnName == "RePassword") {
                    if (Password != null && RePassword != null) {
                        if (!Password.Equals(RePassword)) {
                            return "Passwords are not the same!";
                        }
                    }
                }

                return "";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error {
            get { throw new NotImplementedException(); }
        }

        #endregion IDataErrorInfo
    }
}