using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ViCommV2.Classes
{
	class WindowStickManager
	{
		Window window;
		Point Screen;

		public int DIFF { get; set; }

		public WindowStickManager(Window window)
		{
			DIFF = 10;

			this.window = window;
			window.LocationChanged += (s, e) => { CheckBoundaries(); };
			Screen = new Point(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);
		}

		public void CheckBoundaries()
		{
			if (window.Top <= DIFF && window.Top >= -DIFF) {
				window.Top = 0;
			}
			else if (window.Top + window.Height >= Screen.Y - DIFF && window.Top + window.Height <= Screen.Y + DIFF) {
				window.Top = Screen.Y - window.Height;
			}

			if (window.Left <= DIFF && window.Left >= -DIFF) {
				window.Left = 0;
			}
			else if (window.Left + window.Width >= Screen.X - DIFF && window.Left + window.Width <= Screen.X + DIFF) {
				window.Left = Screen.X - window.Width;
			}
		}
	}
}
