using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using ViData;
using Media = System.Windows.Media;

namespace ViCommV2
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
	public partial class SettingsWindow : Window
	{
		private SettingsProvider manager;
		private Settings settings;

		public SettingsWindow()
		{
			manager = FormHelper.GetInstance().SettingsManager;
			settings = new Settings();

			InitializeComponent();

			settings = manager.settings;
			this.DataContext = settings;

			Buttons_Clicks();
		}

		private void Buttons_Clicks()
		{
			bt_MessageFont.Click += (sender, e) => {
				FontDialog dialog = new FontDialog();
				dialog.Font = settings.MessageFont;

				try {
					if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
						settings.MessageFont = dialog.Font;
					}
				}
				catch (ArgumentException ex) {
					System.Windows.MessageBox.Show(ex.Message, "Error");
				}
			};

			bt_DateFont.Click += (sender, e) => {
				FontDialog dialog = new FontDialog();
				dialog.Font = settings.DateFont;

				try {
					if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
						settings.DateFont = dialog.Font;
					}
				}
				catch (ArgumentException ex) {
					System.Windows.MessageBox.Show(ex.Message, "Error");
				}
			};
		}

		private void bt_Load_Click(object sender, RoutedEventArgs ex)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.AddExtension = true;
			dialog.Multiselect = false;
			dialog.CheckFileExists = true;
			dialog.SupportMultiDottedExtensions = true;
			dialog.InitialDirectory = Tools.GetStartupPath();
			dialog.Title = "Choose Settings file";
			dialog.Filter = "Settings Files (*.xml)|*.xml";
			dialog.FilterIndex = 1;

			dialog.FileOk += (s, e) => { manager.LoadSettings(dialog.FileName); };
			dialog.ShowDialog();
		}

		private void bt_Save_Click(object sender, RoutedEventArgs e)
		{
			manager.SaveSettings();
		}

		private void bt_Close_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Media.Color> e)
		{
			if (settings.BackgroundColor.Color != ClrPcker_Background.SelectedColor) {
				settings.BackgroundColor = new Media.SolidColorBrush(ClrPcker_Background.SelectedColor);
			}
		}

		private void ClrPcker_Border_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Media.Color> e)
		{
			if (settings.BorderColor.Color != ClrPcker_Border.SelectedColor) {
				settings.BorderColor = new Media.SolidColorBrush(ClrPcker_Border.SelectedColor);
			}
		}

		private void ClrPcker_RowUser_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Media.Color> e)
		{
			if (settings.RowUserColor.Color != ClrPcker_RowUser.SelectedColor) {
				settings.RowUserColor = new Media.SolidColorBrush(ClrPcker_RowUser.SelectedColor);
			}
		}

		private void ClrPcker_RowSender_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Media.Color> e)
		{
			if (settings.RowSenderColor.Color != ClrPcker_RowSender.SelectedColor) {
				settings.RowSenderColor = new Media.SolidColorBrush(ClrPcker_RowSender.SelectedColor);
			}
		}

		private void ClrPcker_MessageForeground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Media.Color> e)
		{
			if (settings.MessageForeground.Color != ClrPcker_MessageForeground.SelectedColor) {
				settings.MessageForeground = new Media.SolidColorBrush(ClrPcker_MessageForeground.SelectedColor);
			}
		}

		private void ClrPcker_DateForeground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Media.Color> e)
		{
			if (settings.DateForeground.Color != ClrPcker_DateForeground.SelectedColor) {
				settings.DateForeground = new Media.SolidColorBrush(ClrPcker_DateForeground.SelectedColor);
			}
		}
	}

	public class Settings : ViewModelBase, IDisposable
	{
		private Font _messageFont = new Font("Segoe UI", 12);
		public Font MessageFont
		{
			get
			{
				return this._messageFont;
			}
			set
			{
				this._messageFont = value;
				NotifyPropertyChanged();
			}
		}

		private Font _dateFont = new Font("Segoe UI", 9);
		public Font DateFont
		{
			get
			{
				return this._dateFont;
			}
			set
			{
				this._dateFont = value;
				NotifyPropertyChanged();
			}
		}

		private Media.SolidColorBrush _messageForeground = Media.Brushes.LightGray;
		public Media.SolidColorBrush MessageForeground
		{
			get
			{
				return this._messageForeground;
			}
			set
			{
				this._messageForeground = value;
				NotifyPropertyChanged();
			}
		}

		private Media.SolidColorBrush _dateForeground = BrushExtension.FromARGB("#87000000");
		public Media.SolidColorBrush DateForeground
		{
			get
			{
				return this._dateForeground;
			}
			set
			{
				this._dateForeground = value;
				NotifyPropertyChanged();
			}
		}

		private Media.SolidColorBrush _bgColor = Media.Brushes.DarkSlateGray;
		public Media.SolidColorBrush BackgroundColor
		{
			get
			{
				return this._bgColor;
			}
			set
			{
				this._bgColor = value;
				NotifyPropertyChanged();
			}
		}

		private Media.SolidColorBrush _borderColor = (Media.SolidColorBrush)new Media.BrushConverter().ConvertFrom("#FF436363");
		public Media.SolidColorBrush BorderColor
		{
			get
			{
				return this._borderColor;
			}
			set
			{
				this._borderColor = value;
				NotifyPropertyChanged();
			}
		}

		private Media.SolidColorBrush _rowUserColor = Media.Brushes.CadetBlue;
		public Media.SolidColorBrush RowUserColor
		{
			get
			{
				return this._rowUserColor;
			}
			set
			{
				this._rowUserColor = value;
				NotifyPropertyChanged();
			}
		}

		private Media.SolidColorBrush _rowSenderColor = Media.Brushes.MediumSeaGreen;
		public Media.SolidColorBrush RowSenderColor
		{
			get
			{
				return this._rowSenderColor;
			}
			set
			{
				this._rowSenderColor = value;
				NotifyPropertyChanged();
			}
		}

		#region Dispose Implementation

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Settings()
		{
			Dispose(false);
		}

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing) {
				// Free any other managed objects here.
				_messageFont.Dispose();
				_dateFont.Dispose();
			}
			// Free any unmanaged objects here.

			_disposed = true;
		}

		#endregion Dispose Implementation
	}

	public class SettingsProvider : IDisposable
	{
		public Settings settings;

		private static SettingsProvider _Instance = null;

		public static SettingsProvider GetInstance()
		{
			if (_Instance == null) {
				_Instance = new SettingsProvider();
			}

			return _Instance;
		}

		private SettingsProvider()
		{
			settings = new Settings();

			//Load();

			LoadSettings();
		}

		public void LoadDefault()
		{
			settings.MessageFont = new Font("Segoe UI", 12);
			settings.MessageForeground = Media.Brushes.LightGray;
			settings.DateFont = new Font("Segoe UI", 9);
			settings.DateForeground = Media.Brushes.CadetBlue;
			settings.BackgroundColor = Media.Brushes.DarkSlateGray;
			settings.BorderColor = (Media.SolidColorBrush)new Media.BrushConverter().ConvertFrom("#FF436363");
			settings.RowUserColor = Media.Brushes.CadetBlue;
			settings.RowSenderColor = Media.Brushes.MediumSeaGreen;
		}

		public void LoadSettings()
		{
			string path = Tools.GetStartupPath() + @"\settings.xml";
			LoadSettings(path);
		}

		public void LoadSettings(string path)
		{
			if (File.Exists(path)) {
				ReadXML(path);
			}
			else {
				SaveSettings();
			}
		}

		public void SaveSettings()
		{
			string path = Tools.GetStartupPath() + @"\settings.xml";

			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() {
				Indent = true,
				IndentChars = "\t",
				NewLineOnAttributes = true
			};

			using (XmlWriter writer = XmlWriter.Create(path, xmlWriterSettings)) {
				writer.WriteStartDocument();
				writer.WriteStartElement("Settings");

				// Background Color
				writer.WriteStartElement("BackgroundColor");
				writer.WriteAttributeString("Value", settings.BackgroundColor.ToARGB());
				writer.WriteEndElement();

				// Border Color
				writer.WriteStartElement("BorderColor");
				writer.WriteAttributeString("Value", settings.BorderColor.ToARGB());
				writer.WriteEndElement();

				// Message Font
				writer.WriteStartElement("MessageFont");
				writer.WriteAttributeString("FontFamily", settings.MessageFont.Name.ToString());
				writer.WriteAttributeString("FontSize", settings.MessageFont.Size.ToString());
				writer.WriteAttributeString("FontStyle", settings.MessageFont.Style.ToString());
				writer.WriteAttributeString("MessageForeground", settings.MessageForeground.ToARGB());
				writer.WriteEndElement();

				// Message Font
				writer.WriteStartElement("DateFont");
				writer.WriteAttributeString("FontFamily", settings.DateFont.Name.ToString());
				writer.WriteAttributeString("FontSize", settings.DateFont.Size.ToString());
				writer.WriteAttributeString("FontStyle", settings.DateFont.Style.ToString());
				writer.WriteAttributeString("DateForeground", settings.DateForeground.ToARGB());
				writer.WriteEndElement();

				// Row User Color
				writer.WriteStartElement("RowUserColor");
				writer.WriteAttributeString("Value", settings.RowUserColor.ToARGB());
				writer.WriteEndElement();

				// Row Sender Color
				writer.WriteStartElement("RowSenderColor");
				writer.WriteAttributeString("Value", settings.RowSenderColor.ToARGB());
				writer.WriteEndElement();

				writer.WriteEndElement();
				writer.WriteEndDocument();
			}

			xmlWriterSettings = null;
		}

		private void ReadXML(string path)
		{
			string color;
			string foreground;
			string fontFamily;
			float fontSize;
			System.Drawing.FontStyle fontStyle;

			using (XmlReader reader = XmlReader.Create(path)) {
				while (reader.Read()) {
					if (reader.IsStartElement()) {
						switch (reader.Name) {
							case "BackgroundColor":
								color = reader["Value"];

								settings.BackgroundColor = BrushExtension.FromARGB(color);
								break;

							case "BorderColor":
								color = reader["Value"];

								settings.BorderColor = BrushExtension.FromARGB(color);
								break;

							case "MessageFont":
								fontFamily = reader["FontFamily"];
								fontSize = reader["FontSize"].ToFloat();
								fontStyle = Extensions.ParseEnum<System.Drawing.FontStyle>(reader["FontStyle"]);
								foreground = reader["MessageForeground"];

								settings.MessageFont = new Font(fontFamily, fontSize, fontStyle);
								settings.MessageForeground = BrushExtension.FromARGB(foreground);
								break;

							case "DateFont":
								fontFamily = reader["FontFamily"];
								fontSize = reader["FontSize"].ToFloat();
								fontStyle = Extensions.ParseEnum<System.Drawing.FontStyle>(reader["FontStyle"]);
								foreground = reader["DateForeground"];

								settings.DateFont = new Font(fontFamily, fontSize, fontStyle);
								settings.DateForeground = BrushExtension.FromARGB(foreground);
								break;

							case "RowUserColor":
								color = reader["Value"];

								settings.RowUserColor = BrushExtension.FromARGB(color);
								break;

							case "RowSenderColor":
								color = reader["Value"];

								settings.RowSenderColor = BrushExtension.FromARGB(color);
								break;
						}
					}
				}
			}
		}

		#region Dispose Implementation

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SettingsProvider()
		{
			Dispose(false);
		}

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing) {
				// Free any other managed objects here.
				settings.Dispose();
			}
			// Free any unmanaged objects here.

			_disposed = true;
		}

		#endregion Dispose Implementation
	}

	[Serializable]
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		//make it protected, so it is accessible from Child classes
		protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}

namespace System.Runtime.CompilerServices
{
	internal sealed class CallerMemberNameAttribute : Attribute
	{
	}
}