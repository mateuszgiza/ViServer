using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ViComm
{
	public static class FontHelper
	{
		public static PrivateFontCollection fonts = new PrivateFontCollection();

		public static void AddFontFromResource(string fontResourceName)
		{
			Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fontResourceName);
			System.IntPtr data = Marshal.AllocCoTaskMem((int) fontStream.Length);
			byte[] fontdata = new byte[fontStream.Length];

			fontStream.Read(fontdata, 0, (int) fontStream.Length);
			Marshal.Copy(fontdata, 0, data, (int) fontStream.Length);
			fonts.AddMemoryFont(data, (int) fontStream.Length);

			fontStream.Close();
			Marshal.FreeCoTaskMem(data);
		}
	}

	public class Sound : IDisposable
	{
		private static Dictionary<SoundType, Stream> Sounds = new Dictionary<SoundType, Stream>();
		private Stream str;
		private SoundPlayer s;

		public static void AddSound(SoundType key, Stream path)
		{
			if ( Sounds.ContainsKey(key) == false ) {
				Sounds.Add(key, path);
			}
		}

		public void Play(SoundType type)
		{
			str = Sounds[type];

			s = new SoundPlayer(str);
			s.Stream.Position = 0;
			s.Play();
		}

		public enum SoundType
		{
			Available,
			Message
		}

		public void Dispose()
		{
			str.Dispose();
			str = null;
			s.Dispose();
			s = null;

			GC.SuppressFinalize(this); 
		}
	}

	public static class ControlExtensions
	{
		public static void InvokeIfRequired(this Control control, Action action)
		{
			if ( control.InvokeRequired )
				control.Invoke(action);
			else
				action();
		}

		public static void InvokeIfRequired<T>(this Control control, Action<T> action, T parameter)
		{
			if ( control.InvokeRequired )
				control.Invoke(action, parameter);
			else
				action(parameter);
		}
	}

	public partial class FormHelper : Form
	{
		public Form1 form;
		public FormLogin form_login;
		public FormRegister form_register;

		private FormHelper()
		{
			InitializeComponent();
		}

		private static FormHelper _Instance = null;
		public static FormHelper GetInstance()
		{
			if ( _Instance == null ) {
				_Instance = new FormHelper();
			}

			return _Instance;
		}

		private void FormHelper_Load(object sender, EventArgs e)
		{
			form_login = new FormLogin();
			form_login.Show();
		}
	}
}
