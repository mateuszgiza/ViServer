using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using ViCommV2.Classes;

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for NotifyWindow.xaml
    /// </summary>
    public partial class NotifyWindow : Window
    {
        public NotifyWindow()
        {
            InitializeComponent();

            DataContext = SettingsProvider.Instance.Settings;

            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 5;
            Top = desktopWorkingArea.Bottom - Height - 5;
        }

        public void ShowMessage(BitmapImage avatar, string text)
        {
            img_avatar.Source = avatar;
            tb_text.Text = text;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var main = FormHelper.Instance.Main;
            main.WindowState = WindowState.Normal;
            main.Focus();
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;

            var anim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(500));
            anim.Completed += (s, _) => {
                FormHelper.Instance.Notify = null;
                Close();
            };
            BeginAnimation(OpacityProperty, anim);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}