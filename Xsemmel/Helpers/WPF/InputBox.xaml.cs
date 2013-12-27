using System.Windows;

namespace XSemmel.Helpers.WPF
{

    /// <summary>
    /// An input box
    /// </summary>
    public static class InputBox
    {

        /// <summary>
        /// Shows an input box
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns>The input or null, if the user did not press 'OK'</returns>
        public static string Show(Window owner, string caption, string message)
        {
            InputBoxWnd wnd = new InputBoxWnd(caption, message) {Owner = owner};
            wnd.ShowDialog();
            return wnd.Result;
        }

        /// <summary>
        /// Shows an input box
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="def">Default input text</param>
        /// <returns>The input or null, if the user did not press 'OK'</returns>
        public static string Show(Window owner, string caption, string message, string def)
        {
            InputBoxWnd wnd = new InputBoxWnd(caption, message, def) { Owner = owner };
            wnd.ShowDialog();
            return wnd.Result;
        }
        
    }

    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    internal partial class InputBoxWnd
    {

        public string Result { get; private set; }


        private InputBoxWnd()
        {
            InitializeComponent();
        }

        public InputBoxWnd(string caption, string message) : this()
        {
            InitializeComponent();

            Title = caption;
            lblMessage.Text = message;
            edtInput.Focus();
        }

        public InputBoxWnd(string caption, string message, string def) : this(caption, message)
        {
            edtInput.Text = def;
            edtInput.SelectAll();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = edtInput.Text;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = null;
            Close();
        }

    }
}
