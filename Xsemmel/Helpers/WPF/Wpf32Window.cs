using System;
using System.Windows;
using System.Windows.Interop;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace XSemmel.Helpers.WPF
{
    public class Wpf32Window : IWin32Window
    {
        public IntPtr Handle { get; private set; }

        public Wpf32Window(Window wpfWindow)
        {
            Handle = new WindowInteropHelper(wpfWindow).Handle;
        }
    }
}
