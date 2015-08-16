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
    public sealed partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Handle Exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            // create the main window and assign your datacontext
            var main = new FormHelper();
            main.Show();
        }

        private static void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.Handled = true;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception) e.ExceptionObject);
        }

        private static void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.Handled = true;
        }

        public static bool IsFileReady(string sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try {
                using (var inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None)) {
                    return inputStream.Length > 0;

                }
            }
            catch (Exception) {
                return false;
            }
        }

        private static readonly object Lock = new object();
        public static void HandleException(Exception e)
        {
            var dt = DateTime.Now;
            var startdir = Tools.GetStartupPath();
            var dir = startdir + @"\_logs";
            var filename = @"\log - " + dt.Date.ToString("yyyy-MM-dd") + ".txt";

            while (!IsFileReady(filename)) {}

            try {
                lock (Lock) {
                    if (!File.Exists(dir + filename)) {
                        if (!Directory.Exists(dir)) {
                            Directory.CreateDirectory(dir);
                        }

                        File.Create(dir + filename);
                    }

                    var time = dt.ToString("HH:mm:ss");

                    using (var log = File.AppendText(dir + filename)) {
                        var code = 0;
                        var w32Ex = e as Win32Exception ?? e.InnerException as Win32Exception;
                        if (w32Ex != null) {
                            code = w32Ex.ErrorCode;
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
                            $"Looks like the Unhandled Exception occured!\nSend this file to Autor:\n{dir}{filename}",
                            "Unhandled Exception!");
                    }
                }
            }
            finally {
                FormHelper.IsClosing = true;
                Environment.Exit(0);
            }
        }
    }
}