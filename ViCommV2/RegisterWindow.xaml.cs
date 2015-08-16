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
    public partial class RegisterWindow
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 100)};
        private bool _lock;
        private ClientManager _client;
        private int _numErrors;

        public RegisterWindow()
        {
            InitializeComponent();
            InitializeEvents();

            DataContext = new RegisterWindowViewModel();

            bt_register.IsEnabled = false;

            WindowStickManager.AddWindow(this);
        }

        public bool Registered { get; set; }

        private void InitializeEvents()
        {
            bt_register.Click += (sender, e) => { Register(); };
            tb_name.KeyDown += InputBox_KeyDown;
            tb_mail.KeyDown += InputBox_KeyDown;
            tb_pwd.KeyDown += InputBox_KeyDown;
            tb_repwd.KeyDown += InputBox_KeyDown;
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                Register();
            }
        }
        
        private void Register()
        {
            var loginName = tb_name.Text;
            var mail = tb_mail.Text;
            var password = tb_pwd.Password;
            var rePassword = tb_repwd.Password;

            if (loginName != "" && mail != "" && password != "" && rePassword != "") {
                if (password.Equals(rePassword)) {
                    _client = ClientManager.Instance;

                    _client.Connect();
                    _client.Register(loginName, mail, Encoding.UTF8.GetBytes(password));

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
            if (Registered) {
                return;
            }

            var helper = FormHelper.Instance;
            helper.Login = new LoginWindow();
            helper.Login.Show();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) {
                _numErrors++;
            }
            else {
                _numErrors--;
            }

            if (tb_name.Text.Length > 0 && tb_mail.Text.Length > 0 && tb_pwd.Password.Length > 0 &&
                tb_repwd.Password.Length > 0) {
                bt_register.IsEnabled = _numErrors == 0;
            }
        }

        private void Password_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (_lock) {
                return;
            }

            _timer.Start();
            _lock = true;
            _timer.Tick += (s, ev) => {
                _lock = false;
                _timer.Stop();
            };
            tb_repwd.GetBindingExpression(PasswordHelper.PasswordProperty)?.UpdateSource();
        }

        private void RePassword_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (_lock) {
                return;
            }

            _timer.Start();
            _lock = true;
            _timer.Tick += (s, ev) => {
                _lock = false;
                _timer.Stop();
            };
            tb_pwd.GetBindingExpression(PasswordHelper.PasswordProperty)?.UpdateSource();
        }
    }

    public class RegisterWindowViewModel : IDataErrorInfo
    {
        private readonly Regex _emailRegex =
            new Regex(@"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");

        //property to bind to textbox
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }

        #region IDataErrorInfo

        //In this region we are implementing the properties defined in //the IDataErrorInfo interface in System.ComponentModel

        public string this[string columnName] {
            get {
                switch (columnName) {
                    case "Email":
                        if (Email == null) {
                            return "";
                        }

                        if (!_emailRegex.Match(Email).Success) {
                            return "Not a valid e-mail address!";
                        }

                        break;
                    case "Password":
                    case "RePassword":
                        if (Password == null || RePassword == null) {
                            return "";
                        }

                        if (!Password.Equals(RePassword)) {
                            return "Passwords are not the same!";
                        }

                        break;
                }

                return "";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Error {
            get { throw new NotImplementedException(); }
        }

        #endregion IDataErrorInfo
    }
}