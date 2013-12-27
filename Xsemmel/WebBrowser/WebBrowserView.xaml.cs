using System;
using System.Windows;
using System.Windows.Input;
using XSemmel.Editor;

namespace XSemmel.WebBrowser
{

    public partial class WebBrowserView
    {

        private EditorFrame _editor;

        public WebBrowserView()
        {
            InitializeComponent();
        }

        private void ClickRefresh(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _browser.NavigateToString(_editor.XmlEditor.Text);
            } 
            catch (Exception)
            {
                //
            }
            finally
            {
                Cursor = null;
            }
        }

        public EditorFrame Editor
        {
            set
            {
                if (_editor != null)
                {
                    throw new Exception("Editor already set");
                }
                _editor = value;
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_editor != null && _editor.XmlEditor != null)
            {
                if (true.Equals(e.NewValue))
                {
                    ClickRefresh(sender, null);
                    _editor.XmlEditor.TextChanged += Editor_TextChanged;
                }
                else
                {
                    _editor.XmlEditor.TextChanged -= Editor_TextChanged;
                }
            }
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            ClickRefresh(sender, null);
        }


    }
}
