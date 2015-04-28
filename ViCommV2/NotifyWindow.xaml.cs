using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ViCommV2
{
	/// <summary>
	/// Interaction logic for NotifyWindow.xaml
	/// </summary>
	public partial class NotifyWindow : Window
	{
		public NotifyWindow()
		{
			InitializeComponent();

			this.DataContext = SettingsProvider.Instance.settings;

			var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
			this.Left = desktopWorkingArea.Right - this.Width - 5;
			this.Top = desktopWorkingArea.Bottom - this.Height - 5;
		}

		public void ShowMessage(BitmapImage avatar, String text)
		{
			img_avatar.Source = avatar;
			tb_text.Text = text;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			MainWindow Main = FormHelper.Instance.Main;
			Main.WindowState = System.Windows.WindowState.Normal;
			Main.Focus();
			this.Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Closing -= Window_Closing;
			e.Cancel = true;

			var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromMilliseconds(500));
			anim.Completed += (s, _) => {
				FormHelper.Instance.Notify = null;
				this.Close();
			};
			this.BeginAnimation(UIElement.OpacityProperty, anim);
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}