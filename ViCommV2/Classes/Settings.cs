using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ViData;

using Brushes = System.Windows.Media.Brushes;

namespace ViCommV2.Classes
{
    public class Settings : ViewModelBase, IDisposable
    {
        #region General

        // The path to the key where Windows looks for startup applications
        private readonly RegistryKey _regStartup =
            Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private bool _runOnStartup;

        public bool RunOnStartup
        {
            get { return _runOnStartup; }
            set
            {
                _runOnStartup = value;

                if (_runOnStartup) {
                    _regStartup.SetValue("ViComm", Tools.GetStartupPath());
                }
                else {
                    _regStartup.DeleteValue("ViComm", false);
                }

                NotifyPropertyChanged();
            }
        }

        private bool _alwaysOnTop;

        public bool AlwaysOnTop
        {
            get { return _alwaysOnTop; }
            set
            {
                _alwaysOnTop = value;
                NotifyPropertyChanged();
            }
        }

        #endregion General

        #region Fonts

        private Font _messageFont = new Font("Segoe UI", 12);

        public Font MessageFont
        {
            get { return _messageFont; }
            set
            {
                _messageFont = value;
                NotifyPropertyChanged();
            }
        }

        private Font _dateFont = new Font("Segoe UI", 9);

        public Font DateFont
        {
            get { return _dateFont; }
            set
            {
                _dateFont = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Fonts

        #region Colors

        private SolidColorBrush _messageForeground = Brushes.LightGray;

        public SolidColorBrush MessageForeground
        {
            get { return _messageForeground; }
            set
            {
                _messageForeground = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _dateForeground = BrushExtension.FromARGB("#87000000");

        public SolidColorBrush DateForeground
        {
            get { return _dateForeground; }
            set
            {
                _dateForeground = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _bgColor = Brushes.DarkSlateGray;

        public SolidColorBrush BackgroundColor
        {
            get { return _bgColor; }
            set
            {
                _bgColor = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _borderColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF436363");

        public SolidColorBrush BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _rowUserColor = Brushes.CadetBlue;

        public SolidColorBrush RowUserColor
        {
            get { return _rowUserColor; }
            set
            {
                _rowUserColor = value;
                NotifyPropertyChanged();
            }
        }

        private SolidColorBrush _rowSenderColor = Brushes.MediumSeaGreen;

        public SolidColorBrush RowSenderColor
        {
            get { return _rowSenderColor; }
            set
            {
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
    internal sealed class CallerMemberNameAttribute : Attribute { }
}