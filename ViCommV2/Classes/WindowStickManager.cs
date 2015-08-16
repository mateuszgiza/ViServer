using System.Collections.Generic;
using System.Windows;

namespace ViCommV2.Classes
{
    public static class WindowStickManager
    {
        // ReSharper disable once CollectionNeverQueried.Local
        private static readonly List<Window> Windows = new List<Window>();
        private const int StickSize = 10;
        private static Point _screen = new Point(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);
        
        public static void AddWindow(Window window)
        {
            Windows.Add(window);

            window.LocationChanged += (s, e) => CheckBoundaries(window);
        }

        private static void CheckBoundaries(Window window)
        {
            if (window.Top <= StickSize && window.Top >= -StickSize) {
                window.Top = 0;
            }
            else if (window.Top + window.Height >= _screen.Y - StickSize && window.Top + window.Height <= _screen.Y + StickSize) {
                window.Top = _screen.Y - window.Height;
            }

            if (window.Left <= StickSize && window.Left >= -StickSize) {
                window.Left = 0;
            }
            else if (window.Left + window.Width >= _screen.X - StickSize && window.Left + window.Width <= _screen.X + StickSize) {
                window.Left = _screen.X - window.Width;
            }
        }
    }
}