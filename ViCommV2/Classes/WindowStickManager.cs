using System.Windows;

namespace ViCommV2.Classes
{
    internal class WindowStickManager
    {
        private readonly Window _window;
        private readonly int DIFF;
        private Point _screen;

        public WindowStickManager(Window window) : this(window, 10) {}

        public WindowStickManager(Window window, int diff)
        {
            _window = window;
            DIFF = diff;

            window.LocationChanged += (s, e) => { CheckBoundaries(); };
            _screen = new Point(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);
        }

        public void CheckBoundaries()
        {
            if (_window.Top <= DIFF && _window.Top >= -DIFF) {
                _window.Top = 0;
            }
            else if (_window.Top + _window.Height >= _screen.Y - DIFF && _window.Top + _window.Height <= _screen.Y + DIFF) {
                _window.Top = _screen.Y - _window.Height;
            }

            if (_window.Left <= DIFF && _window.Left >= -DIFF) {
                _window.Left = 0;
            }
            else if (_window.Left + _window.Width >= _screen.X - DIFF && _window.Left + _window.Width <= _screen.X + DIFF) {
                _window.Left = _screen.X - _window.Width;
            }
        }
    }
}