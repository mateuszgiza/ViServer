using System.Security.Permissions;
using System.Windows;

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for FormHelper.xaml
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class FormHelper : Window
    {
        private static FormHelper _instance;

        public FormHelper()
        {
            InitializeComponent();
            _instance = this;

            SettingsManager = SettingsProvider.Instance;

            Login = new LoginWindow();
            Login.Show();
        }

        public static FormHelper Instance {
            get {
                if (_instance == null) {
                    _instance = new FormHelper();
                }

                return _instance;
            }
        }

        #region Fields

        public static bool isClosing { get; set; }

        public MainWindow Main { get; set; }

        public LoginWindow Login { get; set; }

        public RegisterWindow Register { get; set; }

        public SettingsProvider SettingsManager { get; set; }

        private NotifyWindow _notifyWindow;

        public NotifyWindow Notify {
            get {
                if (_notifyWindow == null) {
                    _notifyWindow = new NotifyWindow();
                }
                return _notifyWindow;
            }
            set { _notifyWindow = value; }
        }

        #endregion Fields
    }
}