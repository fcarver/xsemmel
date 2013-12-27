using System.Windows;

namespace XSemmel.Configuration
{
    public partial class ConfigurationWnd
    {
        public ConfigurationWnd()
        {
            InitializeComponent();

            _propertyControl.CurrentObject = XSConfiguration.Instance.Config;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            XSConfiguration.Instance.Save();
            Close();
        }
    }
}
