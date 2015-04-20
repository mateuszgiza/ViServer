using System;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Windows;

namespace ViCommV2
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
	public partial class App : Application
	{
		public App()
			: base()
		{
			App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
			Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
			App.Current.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			App.Current.Run(new FormHelper());
		}

		private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			HandleException(e.Exception);
			e.Handled = true;
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			HandleException((Exception)e.ExceptionObject);
		}

		private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			HandleException(e.Exception);
			e.Handled = true;
		}

		public static void HandleException(Exception e)
		{
			DateTime dt = DateTime.Now;
			string startdir = ViData.Tools.GetStartupPath();
			string dir = startdir + @"\_logs";
			string filename = @"\log - " + dt.Date.ToString("yyyy-MM-dd") + ".txt";

			try {
				if (!File.Exists(dir + filename)) {
					if (!Directory.Exists(dir)) {
						Directory.CreateDirectory(dir);
					}

					File.Create(dir + filename);
				}

				string time = dt.ToString("HH:mm:ss");

				using (StreamWriter log = File.AppendText(dir + filename)) {
					int code = 0;
					var w32ex = e as Win32Exception;
					if (w32ex == null) {
						w32ex = e.InnerException as Win32Exception;
					}
					if (w32ex != null) {
						code = w32ex.ErrorCode;
					}

					StringBuilder sb = new StringBuilder();
					sb.AppendFormat("////////////////////////////////////////////");
					sb.AppendFormat("\n///// Exception Occured!\t| {0} /////", time);
					sb.AppendFormat("\n////////////////////////////////////////////");

					sb.AppendFormat("\n\tCode: {0}", code);
					sb.AppendFormat("\n\tType: {0}", e.GetType().FullName);
					sb.AppendFormat("\n\tMessage: {0}", e.Message);
					sb.AppendFormat("\n\tSource: {0}", e.Source);
					sb.AppendFormat("\n\tStacktrace: {0}", e.StackTrace);

					if (e.InnerException != null) {
						sb.AppendFormat("\n////////////////////////");
						sb.AppendFormat("\n///// Inner Exception!");
						sb.AppendFormat("\n////////////////////////");

						sb.AppendFormat("\n\tType: {0}", e.InnerException.GetType().FullName);
						sb.AppendFormat("\n\tMessage: {0}", e.InnerException.Message);
						sb.AppendFormat("\n\tSource: {0}", e.InnerException.Source);
						sb.AppendFormat("\n\tStacktrace: {0}", e.InnerException.StackTrace);
					}

					sb.AppendFormat("\n////////////////////////////////////////////");
					sb.AppendFormat("\n///// End of Exception!     | {0} /////", time);
					sb.AppendFormat("\n////////////////////////////////////////////\n");

					log.WriteLine(sb.ToString());
					MessageBox.Show(String.Format("Looks like the Unhandled Exception occured!\nSend this file to Autor:\n{0}{1}", dir, filename),
							"Unhandled Exception!");
				}
			}
			finally {
				FormHelper.isClosing = true;
				System.Environment.Exit(0);
			}
		}
	}
}