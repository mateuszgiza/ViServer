using System;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using ViData;

namespace ViCommV2
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public sealed partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // create the main window and assign your datacontext
            var main = new FormHelper();
            main.Show();
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception) e.ExceptionObject);
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.Handled = true;
        }

        public static void HandleException(Exception e)
        {
            var dt = DateTime.Now;
            var startdir = Tools.GetStartupPath();
            var dir = startdir + @"\_logs";
            var filename = @"\log - " + dt.Date.ToString("yyyy-MM-dd") + ".txt";

            try {
                if (!File.Exists(dir + filename)) {
                    if (!Directory.Exists(dir)) {
                        Directory.CreateDirectory(dir);
                    }

                    File.Create(dir + filename);
                }

                var time = dt.ToString("HH:mm:ss");

                using (var log = File.AppendText(dir + filename)) {
                    var code = 0;
                    var w32ex = e as Win32Exception;
                    if (w32ex == null) {
                        w32ex = e.InnerException as Win32Exception;
                    }
                    if (w32ex != null) {
                        code = w32ex.ErrorCode;
                    }

                    var sb = new StringBuilder();
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
                    MessageBox.Show(
                        string.Format("Looks like the Unhandled Exception occured!\nSend this file to Autor:\n{0}{1}",
                                      dir, filename),
                        "Unhandled Exception!");
                }
            }
            finally {
                FormHelper.isClosing = true;
                Environment.Exit(0);
            }
        }
    }
}