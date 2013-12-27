using System;
using System.Windows;
using System.Windows.Forms;

namespace XSemmel.Helpers
{
#if TODO

    /// <summary>
    /// Helper class to save and load window positions and sizes
    /// </summary>
    public class WindowSaver
    {

        private const string SECTION = "Window";

        private readonly Window _window;

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        private WindowSaver(Window window)
        {
            _window = window;
            loadPosition();
            window.Closing += window_Closed;
        }

        private void window_Closed(object sender, EventArgs e)
        {
            savePosition();
        }

        private void savePosition()
        {
            string name = _window.GetType().Name;

            var rect = _window.RestoreBounds;

            write(name, "left", Math.Max(0, rect.Left));
            write(name, "top", Math.Max(0, rect.Top));
            write(name, "width", rect.Width);
            write(name, "height", rect.Height);
            write(name, "state", (double)_window.WindowState);

            _window.Closing -= window_Closed;
        }


        private void loadPosition()
        {
            string name = _window.GetType().Name;

            double left = read(name, "left");
            double top = read(name, "top");
            double width = read(name, "width");
            double height = read(name, "height");
            double dblState = read(name, "state");

            var screenSize = SystemInformation.VirtualScreen;
            if (left + width > screenSize.Width)
            {
                if (width < screenSize.Width)
                {
                    left = (screenSize.Width - width) / 2.0;
                }
                else
                {
                    left = 0;
                    width = screenSize.Width;
                }
            }
            if (top + height > screenSize.Height)
            {
                if (height < screenSize.Height)
                {
                    top = (screenSize.Height - height) / 2.0;
                }
                else
                {
                    top = 0;
                    height = screenSize.Height;
                }
            }

            if (left >= 0)
            {
                _window.Left = left;
            }
            if (top >= 0)
            {
                _window.Top = top;
            }
            if (width > 0)
            {
                _window.Width = width;
            }
            if (height > 0)
            {
                _window.Height = height;
            }
            if (dblState >= 0)
            {
                WindowState state = (WindowState)dblState;
                _window.WindowState = state;
            }
        }


        private static void write(string windowName, string key, double value)
        {
            string name = windowName + "." + key;
            SessionModel.OverrideOptionWithinSectionInUiConfigurationImmediately(SECTION, name, value.ToString());
        }

        private static double read(string windowName, string key)
        {
            string name = windowName + "." + key;
            string stringValue = SessionModel.GetOptionFromSectionFromUiConfiguration(SECTION, name, "-1");
            double value = Double.Parse(stringValue);
            return value;
        }

        /// <summary>
        /// Registers the specified window to be managed by this WindowSaver
        /// </summary>
        /// <param name="window"></param>
        public static void Register(Window window)
        {
            new WindowSaver(window);
        }
    }

#endif

}
