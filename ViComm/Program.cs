using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ViComm
{
	static class Program
	{
		[DllImport("kernel32.dll",
	    EntryPoint = "GetStdHandle",
	    SetLastError = true,
	    CharSet = CharSet.Auto,
	    CallingConvention = CallingConvention.StdCall)]
		private static extern IntPtr GetStdHandle(int nStdHandle);
		[DllImport("kernel32.dll",
		    EntryPoint = "AllocConsole",
		    SetLastError = true,
		    CharSet = CharSet.Auto,
		    CallingConvention = CallingConvention.StdCall)]
		private static extern int AllocConsole();
		private const int STD_OUTPUT_HANDLE = -11;
		private const int MY_CODE_PAGE = 437;  

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Console.WriteLine("This text you can see in debug output window.");

			AllocConsole();
			IntPtr stdHandle=GetStdHandle(STD_OUTPUT_HANDLE);
			SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
			FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
			Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
			StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
			standardOutput.AutoFlush = true;
			Console.SetOut(standardOutput);

			Console.WriteLine("## Debug Console ##");  

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
