using System.Security.Permissions;
using System.Windows;

namespace ViCommV2
{
	/// <summary>
	/// Interaction logic for FormHelper.xaml
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
	public partial class FormHelper : Window
	{
		public FormHelper()
		{
			InitializeComponent();
			_instance = this;

			_settingsManager = SettingsProvider.Instance;

			_login = new LoginWindow();
			_login.Show();
		}

		private static FormHelper _instance = null;

		public static FormHelper Instance
		{
			get
			{
				if (_instance == null) {
					_instance = new FormHelper();
				}

				return _instance;
			}
		}

		#region Fields

		public static bool isClosing { get; set; }

		private MainWindow _main;
		public MainWindow Main
		{
			get { return _main; }
			set { _main = value; }
		}

		private LoginWindow _login;
		public LoginWindow Login
		{
			get { return _login; }
			set { _login = value; }
		}

		private RegisterWindow _register;
		public RegisterWindow Register
		{
			get { return _register; }
			set { _register = value; }
		}

		private SettingsProvider _settingsManager;
		public SettingsProvider SettingsManager
		{
			get { return _settingsManager; }
			set { _settingsManager = value; }
		}

		private NotifyWindow _notifyWindow;
		public NotifyWindow Notify
		{
			get
			{
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