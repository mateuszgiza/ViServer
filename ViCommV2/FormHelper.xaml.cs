using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media;

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

		#endregion Fields
	}

	public class Sound
	{
		private static Dictionary<SoundType, MediaPlayer> Sounds = new Dictionary<SoundType, MediaPlayer>();
		private MediaPlayer s;

		public static void AddSound(SoundType key, Uri path)
		{
			if (Sounds.ContainsKey(key) == false) {
				MediaPlayer p = new MediaPlayer();
				p.Open(path);
				Sounds.Add(key, p);
			}
		}

		public void Play(SoundType type)
		{
			s = Sounds[type];

			s.Stop();
			s.Play();
		}

		public enum SoundType
		{
			Available,
			Message
		}
	}
}