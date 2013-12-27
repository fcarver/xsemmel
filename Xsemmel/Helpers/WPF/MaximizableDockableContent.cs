using System.Windows;
using AvalonDock;

namespace XSemmel.Helpers.WPF
{
    public class MaximizableDockableContent : DockableContent
    {
        public MaximizableDockableContent()
        {
            StateChanged += MaximizableDockableContent_StateChanged;
        }

        private void MaximizableDockableContent_StateChanged(
            object sender, RoutedEventArgs e)
        {
            MaximizableDockableContent mdc = (MaximizableDockableContent)sender;
            if (mdc.State == DockableContentState.DockableWindow)
            {
                FloatingWindowSizeToContent = SizeToContent.WidthAndHeight;
                FloatingDockablePane fdp = (FloatingDockablePane)Parent;
                DockableFloatingWindow dfw = (DockableFloatingWindow)fdp.Parent;

                //dfw.WindowState = WindowState.Maximized;
                dfw.WindowStyle = WindowStyle.ThreeDBorderWindow;
                dfw.ResizeMode = ResizeMode.CanResize;

                //disable minimize button
                ControlBox.SetHasMinimizeButton(dfw, false);
            }
        }
    }
}
