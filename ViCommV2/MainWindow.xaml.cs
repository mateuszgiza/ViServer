using System;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

// StickyWindow
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ViData;

namespace ViCommV2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
	public partial class MainWindow : Window
	{
		#region Fields

		private ClientManager _client;
		public ClientManager Client
		{
			get { return _client; }
			set { _client = value; }
		}

		private FormHelper _forms;
		public FormHelper Forms
		{
			get { return _forms; }
			set { _forms = value; }
		}

		private SettingsWindow _settings;
		public SettingsWindow Settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		private FormState _state;
		public FormState State
		{
			get { return _state; }
			set { _state = value; }
		}

		private Brush _color;
		public Brush BackgroundColor
		{
			get { return _color; }
			set { _color = value; }
		}

		public ListBox ListContacts { get { return list_Contacts; } }

		#endregion Fields

		public MainWindow()
		{
			InitializeComponent();

			this.DataContext = SettingsProvider.Instance.settings;
		}

		private void Initialize()
		{
			Sound.AddSound(Sound.SoundType.Available, new Uri(@"Resources\Sounds\mp3_available.mp3", UriKind.Relative));
			Sound.AddSound(Sound.SoundType.Message, new Uri(@"Resources\Sounds\mp3_message.mp3", UriKind.Relative));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Initialize();

			_forms = FormHelper.Instance;
			Connect();

			// Display user nickname
			this.Title = "ViComm | Nick: " + _client.user.Nickname;
			lb_Header.Content = "ViComm | Nick: " + _client.user.Nickname;
			_color = Brushes.CadetBlue;

			_state = FormState.Exit;
			inputBox.Focus();
		}

		private void CM_Clear_Click(object sender, RoutedEventArgs e)
		{
			this.chatBox.Document.Blocks.Clear();
		}

		private void inputBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.Enter)) {
				inputBox.Text += "\r\n";
				inputBox.SelectionStart = inputBox.Text.Length;

				e.Handled = true;
			}
			else if (Keyboard.IsKeyDown(Key.Enter)) {
				if (inputBox.Text.Length > 0) {
					Send(inputBox.Text);
					inputBox.Clear();
				}

				e.Handled = true;
			}
		}

		public void Connect()
		{
			if (_client != null) {
				return;
			}

			_client = ClientManager.GetInstance();
			_client.Connect();
		}

		private void Send(string s)
		{
			_client.Send(s);
		}

		private Table Message(Run date, Run text, RowType rowType)
		{
			BitmapImage bitmap = new BitmapImage(new Uri(@"Resources\Images\avatar.png", UriKind.Relative));
			Rectangle rect = new Rectangle() {
				RadiusX = 10,
				RadiusY = 10,
				Width = 24,
				Height = 24,
				Fill = new ImageBrush(bitmap)
			};
			InlineUIContainer container = new InlineUIContainer(rect);

			Paragraph avatar = new Paragraph(container);

			Paragraph txt = new Paragraph(text) {
				Margin = new Thickness(1)
			};
			txt.DetectURL();
			txt.SetBinding(Run.ForegroundProperty, new Binding("MessageForeground"));

			Paragraph dT = new Paragraph(date) {
				Margin = new Thickness(0, 0, 0, 1)
			};

			var tab = new Table {
				Margin = new Thickness(0, 2, 0, 2)
			};

			var gridLenghtConverter = new GridLengthConverter();

			//1. col
			tab.Columns.Add(new TableColumn {
				Name = "colAvatar",
				Width = (GridLength)gridLenghtConverter.ConvertFromString("28")
			});
			//2.col
			tab.Columns.Add(new TableColumn {
				Name = "colMsg",
				Width = (GridLength)gridLenghtConverter.ConvertFromString("Auto")
			});
			//3.col
			tab.Columns.Add(new TableColumn {
				Name = "colDt",
				Width = (GridLength)gridLenghtConverter.ConvertFromString("50")
			});

			tab.RowGroups.Add(new TableRowGroup());
			tab.RowGroups[0].Rows.Add(new TableRow());

			var tabRow = tab.RowGroups[0].Rows[0];

			if (rowType == RowType.User) {
				tabRow.Style = (Style)this.chatBox.Resources["RowUser"];
			}
			else if (rowType == RowType.Sender) {
				tabRow.Style = (Style)this.chatBox.Resources["RowSender"];
			}

			//1.col - NICK
			tabRow.Cells.Add(new TableCell(avatar));

			//2.col - MESSAGE
			tabRow.Cells.Add(new TableCell(txt));

			//3.col  - DATE TIME
			tabRow.Cells.Add(new TableCell(dT));

			return tab;
		}

		public void AppendText(string sender, string msg, DateTime datetime, RowType rowType)
		{
			Run date = new Run(String.Format("{0:HH:mm:ss}", datetime));
			date.SetBinding(Run.FontFamilyProperty, new Binding("DateFont.Name"));
			date.SetBinding(Run.FontSizeProperty, new Binding("DateFont.Size"));
			date.SetBinding(Run.ForegroundProperty, new Binding("DateForeground"));

			//Run name = new Run(sender + ": ");
			Run text = new Run(sender + ": " + msg);

			this.chatBox.Document.Blocks.Add(Message(date, text, rowType));
			chatBox.ScrollToEnd();
		}

		public void AppendInfoText(string text)
		{
			Paragraph p = new Paragraph();
			p.Background = Brushes.Transparent;

			p.Inlines.Add(new Run(text));

			p.DetectURL();

			this.chatBox.Document.Blocks.Add(p);
			chatBox.ScrollToEnd();
		}

		private void CM_Settings_Click(object sender, RoutedEventArgs e)
		{
			if (_settings == null) {
				_settings = new SettingsWindow();
				_settings.Closed += (s, ev) => {
					_settings = null;
				};

				_settings.Show();
			}

			_settings.Focus();
		}

		private void CM_Logout_Click(object sender, RoutedEventArgs e)
		{
			_state = FormState.Logout;

			_forms.Dispatcher.Invoke(new Action(() => {
				_forms.Login = new LoginWindow();
				_forms.Login.Show();
			}));

			this.Close();
		}

		private void CM_Exit_Click(object sender, RoutedEventArgs e)
		{
			_state = FormState.Exit;

			this.Close();
		}

		private void CM_Minimize_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Minimized;
		}

		private void CM_Maximize_Click(object sender, RoutedEventArgs e)
		{
			if (this.WindowState == System.Windows.WindowState.Maximized) {
				(sender as MenuItem).IsChecked = false;
				this.WindowState = System.Windows.WindowState.Normal;
			}
			else {
				(sender as MenuItem).IsChecked = true;
				this.WindowState = System.Windows.WindowState.Maximized;
			}
		}

		public void CloseWindow()
		{
			_state = FormState.Close;
			this.Close();
		}

		public void Exit()
		{
			Logout();

			_state = FormState.Null;
			System.Environment.Exit(0);
		}

		private void Logout()
		{
			if (_client != null) {
				_client.Disconnect(false);
			}
			FormHelper.isClosing = true;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_state == FormState.Logout) {
				Logout();
			}
			else if (_state == FormState.Close) {
			}
			else if (_state == FormState.Exit) {
				Exit();
			}
		}

		public enum FormState
		{
			Null,
			Exit,
			Close,
			Logout
		}

		public enum RowType
		{
			Information,
			User,
			Sender
		}

		private System.Windows.Threading.DispatcherTimer t_writing;
		private double timeElapsed = 0;
		private bool tStarted = false;
		private bool enterPressed = false;

		private void inputBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (enterPressed == false) {
				timeElapsed = 0;

				if (tStarted == false) {
					tStarted = true;
					t_writing = new System.Windows.Threading.DispatcherTimer();
					t_writing.Interval = new TimeSpan(0, 0, 0, 0, 100);
					t_writing.Tick += t_writing_Tick;
					t_writing.Start();

					Packet p = new Packet(PacketType.Information);
					p.info = new Information(InformationType.Writing, _client.user.Nickname, "started");
					_client.Send(p);
				}
			}
		}

		private void t_writing_Tick(object sender, EventArgs e)
		{
			if ((timeElapsed >= 1000) || enterPressed) {
				t_writing.Stop();
				t_writing = null;
				tStarted = false;

				Packet p = new Packet(PacketType.Information);
				p.info = new Information(InformationType.Writing, _client.user.Nickname, "stopped");
				_client.Send(p);
			}
			else {
				timeElapsed += 100;
			}
		}

		private void list_Contacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBox l = sender as ListBox;
			l.UpdateLayout();
		}

		private void bt_menu_Click(object sender, RoutedEventArgs e)
		{
			(sender as Button).ContextMenu.IsEnabled = true;
			(sender as Button).ContextMenu.PlacementTarget = (sender as Button);
			(sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			(sender as Button).ContextMenu.IsOpen = true;
		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) {
				this.DragMove();
			}
		}
	}
}