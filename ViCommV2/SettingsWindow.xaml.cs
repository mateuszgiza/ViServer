using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using ViCommV2.Classes;
using ViData;
using Color = System.Windows.Media.Color;
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

            _settings = _manager.Settings;
            DataContext = _settings;

            Buttons_Clicks();

            WindowStickManager.AddWindow(this);
        }

        private void Buttons_Clicks()
        {
            #region Font Buttons

            bt_MessageFont.Click += (sender, e) => {
                var dialog = new FontDialog {Font = _settings.MessageFont};

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
                var dialog = new FontDialog {Font = _settings.DateFont};

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

                dialog.FileOk += (s, e) => {
                    _manager.LoadSettings(dialog.FileName);
                };
                dialog.ShowDialog();
            }
        }

        private void bt_Save_Click(object sender, RoutedEventArgs e)
        {
            _manager.SaveSettings(_manager.CurrentFile);
        }

        private void bt_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _manager.SaveSettings(_manager.CurrentFile);
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
}
