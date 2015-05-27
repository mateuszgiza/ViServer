using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml;
using Microsoft.Win32;
using ViCommV2.Classes;
using ViData;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using FontStyle = System.Drawing.FontStyle;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for SettingsWindow.xaml
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class SettingsWindow : Window
    {
        private readonly SettingsProvider _manager;
        private readonly Settings _settings;

        public SettingsWindow()
        {
            _manager = FormHelper.Instance.SettingsManager;
            _settings = new Settings();

            InitializeComponent();

            _settings = _manager.settings;
            DataContext = _settings;

            Buttons_Clicks();

            var stick = new WindowStickManager(this);
        }

        private void Buttons_Clicks()
        {
            #region Font Buttons

            bt_MessageFont.Click += (sender, e) => {
                var dialog = new FontDialog();
                dialog.Font = _settings.MessageFont;

                try {
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                        _settings.MessageFont = dialog.Font;
                    }
                }
                catch (ArgumentException ex) {
                    MessageBox.Show(ex.Message, "Error");
                }
            };

            bt_DateFont.Click += (sender, e) => {
                var dialog = new FontDialog();
                dialog.Font = _settings.DateFont;

                try {
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                        _settings.DateFont = dialog.Font;
                    }
                }
                catch (ArgumentException ex) {
                    MessageBox.Show(ex.Message, "Error");
                }
            };

            #endregion Font Buttons
        }

        private void bt_Load_Click(object sender, RoutedEventArgs ex)
        {
            using (var dialog = new OpenFileDialog()) {
                dialog.AddExtension = true;
                dialog.Multiselect = false;
                dialog.CheckFileExists = true;
                dialog.SupportMultiDottedExtensions = true;
                dialog.InitialDirectory = Tools.GetStartupPath();
                dialog.Title = "Choose Settings file";
                dialog.Filter = "Settings Files (*.xml)|*.xml";
                dialog.FilterIndex = 1;

                dialog.FileOk += (s, e) => { _manager.LoadSettings(dialog.FileName); };
                dialog.ShowDialog();
            }
        }

        private void bt_Save_Click(object sender, RoutedEventArgs e)
        {
            _manager.SaveSettings(_manager.currentFile);
        }

        private void bt_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _manager.SaveSettings(_manager.currentFile);
        }

        #region Color Pickers Changed events

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (_settings.BackgroundColor.Color != ClrPcker_Background.SelectedColor) {
                _settings.BackgroundColor = new SolidColorBrush(ClrPcker_Background.SelectedColor);
            }
        }

        private void ClrPcker_Border_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (_settings.BorderColor.Color != ClrPcker_Border.SelectedColor) {
                _settings.BorderColor = new SolidColorBrush(ClrPcker_Border.SelectedColor);
            }
        }

        private void ClrPcker_RowUser_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (_settings.RowUserColor.Color != ClrPcker_RowUser.SelectedColor) {
                _settings.RowUserColor = new SolidColorBrush(ClrPcker_RowUser.SelectedColor);
            }
        }

        private void ClrPcker_RowSender_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (_settings.RowSenderColor.Color != ClrPcker_RowSender.SelectedColor) {
                _settings.RowSenderColor = new SolidColorBrush(ClrPcker_RowSender.SelectedColor);
            }
        }

        private void ClrPcker_MessageForeground_SelectedColorChanged(object sender,
                                                                     RoutedPropertyChangedEventArgs<Color> e)
        {
            if (_settings.MessageForeground.Color != ClrPcker_MessageForeground.SelectedColor) {
                _settings.MessageForeground = new SolidColorBrush(ClrPcker_MessageForeground.SelectedColor);
            }
        }

        private void ClrPcker_DateForeground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (_settings.DateForeground.Color != ClrPcker_DateForeground.SelectedColor) {
                _settings.DateForeground = new SolidColorBrush(ClrPcker_DateForeground.SelectedColor);
            }
        }

        #endregion Color Pickers Changed events
    }

    public class Settings : ViewModelBase, IDisposable
    {
        #region General

        // The path to the key where Windows looks for startup applications
        private readonly RegistryKey regStartup =
            Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private bool _runOnStartup;

        public bool RunOnStartup {
            get { return _runOnStartup; }
            set {
                _runOnStartup = value;

                if (_runOnStartup) {
                    regStartup.SetValue("ViComm", Tools.GetStartupPath());
                }
                else {
                    regStartup.DeleteValue("ViComm", false);
                }

                NotifyPropertyChanged();
            }
        }

        private bool _alwaysOnTop;

        public bool AlwaysOnTop {
            get { return _alwaysOnTop; }
            set {
                _alwaysOnTop = value;
                NotifyPropertyChanged();
            }
        }

        #endregion General

        #region Fonts

        private Font _messageFont = new Font("Segoe UI", 12);

        public Font MessageFont {
            get { return _messageFont; }
            set {
                _messageFont = value;
                NotifyPropertyChanged();
            }
        }

        private Font _dateFont = new Font("Segoe UI", 9);

        public Font DateFont {
            get { return _dateFont; }
            set {
                _dateFont = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Fonts

        #region Colors

        private SolidColorBrush _messageForeground = Brushes.LightGray;

        public SolidColorBrush MessageForeground {
            get { return _messageForeground; }
            set {
                _messageForeground = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _dateForeground = BrushExtension.FromARGB("#87000000");

        public SolidColorBrush DateForeground {
            get { return _dateForeground; }
            set {
                _dateForeground = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _bgColor = Brushes.DarkSlateGray;

        public SolidColorBrush BackgroundColor {
            get { return _bgColor; }
            set {
                _bgColor = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _borderColor = (SolidColorBrush) new BrushConverter().ConvertFrom("#FF436363");

        public SolidColorBrush BorderColor {
            get { return _borderColor; }
            set {
                _borderColor = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _rowUserColor = Brushes.CadetBlue;

        public SolidColorBrush RowUserColor {
            get { return _rowUserColor; }
            set {
                _rowUserColor = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _rowSenderColor = Brushes.MediumSeaGreen;

        public SolidColorBrush RowSenderColor {
            get { return _rowSenderColor; }
            set {
                _rowSenderColor = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Colors

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

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) {
                return;
            }

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
        private static SettingsProvider _instance;
        public string currentFile = Tools.GetStartupPath() + @"\settings.xml";
        public Settings settings;

        private SettingsProvider()
        {
            settings = new Settings();
            LoadSettings();
        }

        public static SettingsProvider Instance {
            get {
                if (_instance == null) {
                    _instance = new SettingsProvider();
                }

                return _instance;
            }
        }

        public void LoadDefault()
        {
            // General
            settings.RunOnStartup = false;
            settings.AlwaysOnTop = false;

            // Fonts
            settings.MessageFont = new Font("Segoe UI", 12);
            settings.DateFont = new Font("Segoe UI", 9);

            // Colors
            settings.MessageForeground = Brushes.LightGray;
            settings.DateForeground = Brushes.CadetBlue;
            settings.BackgroundColor = Brushes.DarkSlateGray;
            settings.BorderColor = (SolidColorBrush) new BrushConverter().ConvertFrom("#FF436363");
            settings.RowUserColor = Brushes.CadetBlue;
            settings.RowSenderColor = Brushes.MediumSeaGreen;
        }

        public void LoadSettings()
        {
            var path = currentFile;
            LoadSettings(path);
        }

        public void LoadSettings(string path)
        {
            currentFile = path;

            if (File.Exists(path)) {
                ReadXML(path);
            }
            else {
                SaveSettings(path);
            }
        }

        public void SaveSettings(string path)
        {
            var xmlWriterSettings = new XmlWriterSettings {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true
            };

            using (var writer = XmlWriter.Create(path, xmlWriterSettings)) {
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");

                // General
                writer.WriteStartElement("RunOnStartup");
                writer.WriteAttributeString("Value", settings.RunOnStartup.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("AlwaysOnTop");
                writer.WriteAttributeString("Value", settings.AlwaysOnTop.ToString());
                writer.WriteEndElement();

                // Message Font
                writer.WriteStartElement("MessageFont");
                writer.WriteAttributeString("FontFamily", settings.MessageFont.Name);
                writer.WriteAttributeString("FontSize", settings.MessageFont.Size.ToString());
                writer.WriteAttributeString("FontStyle", settings.MessageFont.Style.ToString());
                writer.WriteAttributeString("MessageForeground", settings.MessageForeground.ToARGB());
                writer.WriteEndElement();

                // Message Font
                writer.WriteStartElement("DateFont");
                writer.WriteAttributeString("FontFamily", settings.DateFont.Name);
                writer.WriteAttributeString("FontSize", settings.DateFont.Size.ToString());
                writer.WriteAttributeString("FontStyle", settings.DateFont.Style.ToString());
                writer.WriteAttributeString("DateForeground", settings.DateForeground.ToARGB());
                writer.WriteEndElement();

                // Background Color
                writer.WriteStartElement("BackgroundColor");
                writer.WriteAttributeString("Value", settings.BackgroundColor.ToARGB());
                writer.WriteEndElement();

                // Border Color
                writer.WriteStartElement("BorderColor");
                writer.WriteAttributeString("Value", settings.BorderColor.ToARGB());
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
            bool bValue;
            string color;
            string foreground;
            string fontFamily;
            float fontSize;
            FontStyle fontStyle;

            LoadDefault();

            using (var reader = XmlReader.Create(path)) {
                while (reader.Read()) {
                    if (reader.IsStartElement()) {
                        switch (reader.Name) {
                            // General
                            case "RunOnStartup":
                                bValue = reader.GetAttribute("Value").ToBoolean() ?? false;
                                settings.RunOnStartup = bValue;
                                break;

                            case "AlwaysOnTop":
                                bValue = reader.GetAttribute("Value").ToBoolean() ?? false;
                                settings.AlwaysOnTop = bValue;
                                break;

                            // Fonts
                            case "MessageFont":
                                fontFamily = reader.GetAttribute("FontFamily") ?? "Segoe UI";
                                fontSize = reader.GetAttribute("FontSize").ToFloat() ?? 12;
                                fontStyle =
                                    Extensions.ParseEnum<FontStyle>(reader.GetAttribute("FontStyle") ?? "Regular");
                                foreground = reader.GetAttribute("MessageForeground") ?? "#FFD3D3D3";

                                settings.MessageFont = new Font(fontFamily, fontSize, fontStyle);
                                settings.MessageForeground = BrushExtension.FromARGB(foreground);
                                break;

                            case "DateFont":
                                fontFamily = reader.GetAttribute("FontFamily") ?? "Segoe UI";
                                fontSize = reader.GetAttribute("FontSize").ToFloat() ?? 9;
                                fontStyle =
                                    Extensions.ParseEnum<FontStyle>(reader.GetAttribute("FontStyle") ?? "Regular");
                                foreground = reader.GetAttribute("DateForeground") ?? "#87000000";

                                settings.DateFont = new Font(fontFamily, fontSize, fontStyle);
                                settings.DateForeground = BrushExtension.FromARGB(foreground);
                                break;

                            // Colors
                            case "BackgroundColor":
                                color = reader.GetAttribute("Value") ?? "#FF2F4F4F";

                                settings.BackgroundColor = BrushExtension.FromARGB(color);
                                break;

                            case "BorderColor":
                                color = reader.GetAttribute("Value") ?? "#FF436363";

                                settings.BorderColor = BrushExtension.FromARGB(color);
                                break;

                            case "RowUserColor":
                                color = reader.GetAttribute("Value") ?? "#FF5F9EA0";

                                settings.RowUserColor = BrushExtension.FromARGB(color);
                                break;

                            case "RowSenderColor":
                                color = reader.GetAttribute("Value") ?? "#FF3CB371";

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

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) {
                return;
            }

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
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

namespace System.Runtime.CompilerServices
{
    internal sealed class CallerMemberNameAttribute : Attribute {}
}