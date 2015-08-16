using System.Collections.Generic;
using System.Security.Permissions;
using ViCommV2.Classes;

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for FormHelper.xaml
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class FormHelper
    {
        private static FormHelper _instance;

        public FormHelper()
        {
            InitializeComponent();
            _instance = this;

            SettingsManager = SettingsProvider.Instance;
            SingleChat = new Dictionary<string, SingleChatWindow>();

            Login = new LoginWindow();
            Login.Show();
        }

        public static FormHelper Instance => _instance ?? (_instance = new FormHelper());

        #region Fields

        public static bool IsClosing { get; set; }

        public MainWindow Main { get; set; }

        public Dictionary<string, SingleChatWindow> SingleChat;

        public LoginWindow Login { get; set; }

        public RegisterWindow Register { get; set; }

        public SettingsProvider SettingsManager { get; set; }

        private NotifyWindow _notifyWindow;

        public NotifyWindow Notify {
            get { return _notifyWindow ?? (_notifyWindow = new NotifyWindow()); }
            set { _notifyWindow = value; }
        }

        #endregion Fields
    }
}