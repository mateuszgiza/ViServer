using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Xml;
using ViData;

using Brushes = System.Windows.Media.Brushes;
using FontStyle = System.Drawing.FontStyle;

namespace ViCommV2.Classes
{
    public class SettingsProvider : IDisposable
    {
        private static SettingsProvider _instance;
        public string CurrentFile = Tools.GetStartupPath() + @"\settings.xml";
        public Settings Settings;

        private SettingsProvider()
        {
            Settings = new Settings();
            LoadSettings();
        }

        public static SettingsProvider Instance
        {
            get
            {
                if (_instance == null) {
                    _instance = new SettingsProvider();
                }

                return _instance;
            }
        }

        public void LoadDefault()
        {
            // General
            Settings.RunOnStartup = false;
            Settings.AlwaysOnTop = false;

            // Fonts
            Settings.MessageFont = new Font("Segoe UI", 12);
            Settings.DateFont = new Font("Segoe UI", 9);

            // Colors
            Settings.MessageForeground = Brushes.LightGray;
            Settings.DateForeground = Brushes.CadetBlue;
            Settings.BackgroundColor = Brushes.DarkSlateGray;
            Settings.BorderColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF436363");
            Settings.RowUserColor = Brushes.CadetBlue;
            Settings.RowSenderColor = Brushes.MediumSeaGreen;
        }

        public void LoadSettings()
        {
            var path = CurrentFile;
            LoadSettings(path);
        }

        public void LoadSettings(string path)
        {
            CurrentFile = path;

            if (File.Exists(path)) {
                ReadXml(path);
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
                writer.WriteAttributeString("Value", Settings.RunOnStartup.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("AlwaysOnTop");
                writer.WriteAttributeString("Value", Settings.AlwaysOnTop.ToString());
                writer.WriteEndElement();

                // Message Font
                writer.WriteStartElement("MessageFont");
                writer.WriteAttributeString("FontFamily", Settings.MessageFont.Name);
                writer.WriteAttributeString("FontSize", Settings.MessageFont.Size.ToString());
                writer.WriteAttributeString("FontStyle", Settings.MessageFont.Style.ToString());
                writer.WriteAttributeString("MessageForeground", Settings.MessageForeground.ToARGB());
                writer.WriteEndElement();

                // Message Font
                writer.WriteStartElement("DateFont");
                writer.WriteAttributeString("FontFamily", Settings.DateFont.Name);
                writer.WriteAttributeString("FontSize", Settings.DateFont.Size.ToString());
                writer.WriteAttributeString("FontStyle", Settings.DateFont.Style.ToString());
                writer.WriteAttributeString("DateForeground", Settings.DateForeground.ToARGB());
                writer.WriteEndElement();

                // Background Color
                writer.WriteStartElement("BackgroundColor");
                writer.WriteAttributeString("Value", Settings.BackgroundColor.ToARGB());
                writer.WriteEndElement();

                // Border Color
                writer.WriteStartElement("BorderColor");
                writer.WriteAttributeString("Value", Settings.BorderColor.ToARGB());
                writer.WriteEndElement();

                // Row User Color
                writer.WriteStartElement("RowUserColor");
                writer.WriteAttributeString("Value", Settings.RowUserColor.ToARGB());
                writer.WriteEndElement();

                // Row Sender Color
                writer.WriteStartElement("RowSenderColor");
                writer.WriteAttributeString("Value", Settings.RowSenderColor.ToARGB());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void ReadXml(string path)
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
                    if (!reader.IsStartElement()) {
                        continue;
                    }

                    switch (reader.Name) {
                        // General
                        case "RunOnStartup":
                            bValue = reader.GetAttribute("Value").ToBoolean() ?? false;
                            Settings.RunOnStartup = bValue;
                            break;

                        case "AlwaysOnTop":
                            bValue = reader.GetAttribute("Value").ToBoolean() ?? false;
                            Settings.AlwaysOnTop = bValue;
                            break;

                        // Fonts
                        case "MessageFont":
                            fontFamily = reader.GetAttribute("FontFamily") ?? "Segoe UI";
                            fontSize = reader.GetAttribute("FontSize").ToFloat() ?? 12;
                            fontStyle =
                                Extensions.ParseEnum<FontStyle>(reader.GetAttribute("FontStyle") ?? "Regular");
                            foreground = reader.GetAttribute("MessageForeground") ?? "#FFD3D3D3";

                            Settings.MessageFont = new Font(fontFamily, fontSize, fontStyle);
                            Settings.MessageForeground = BrushExtension.FromARGB(foreground);
                            break;

                        case "DateFont":
                            fontFamily = reader.GetAttribute("FontFamily") ?? "Segoe UI";
                            fontSize = reader.GetAttribute("FontSize").ToFloat() ?? 9;
                            fontStyle =
                                Extensions.ParseEnum<FontStyle>(reader.GetAttribute("FontStyle") ?? "Regular");
                            foreground = reader.GetAttribute("DateForeground") ?? "#87000000";

                            Settings.DateFont = new Font(fontFamily, fontSize, fontStyle);
                            Settings.DateForeground = BrushExtension.FromARGB(foreground);
                            break;

                        // Colors
                        case "BackgroundColor":
                            color = reader.GetAttribute("Value") ?? "#FF2F4F4F";

                            Settings.BackgroundColor = BrushExtension.FromARGB(color);
                            break;

                        case "BorderColor":
                            color = reader.GetAttribute("Value") ?? "#FF436363";

                            Settings.BorderColor = BrushExtension.FromARGB(color);
                            break;

                        case "RowUserColor":
                            color = reader.GetAttribute("Value") ?? "#FF5F9EA0";

                            Settings.RowUserColor = BrushExtension.FromARGB(color);
                            break;

                        case "RowSenderColor":
                            color = reader.GetAttribute("Value") ?? "#FF3CB371";

                            Settings.RowSenderColor = BrushExtension.FromARGB(color);
                            break;
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
                Settings.Dispose();
            }
            // Free any unmanaged objects here.

            _disposed = true;
        }

        #endregion Dispose Implementation
    }
}
