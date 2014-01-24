using System.IO;
using System.Windows;
using XSemmel.Configuration;

namespace XSemmel
{
    public partial class ApplicationMenu
    {
        
        public ApplicationMenu()
        {
            InitializeComponent();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            new ConfigurationWnd {Owner = Application.Current.MainWindow}.ShowDialog();
        }

        private void ApplicationMenu_OnIsOpenChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (true.Equals(e.NewValue))
            {
                _mnuOpen.Children.Clear();

                var files = XSConfiguration.Instance.Config.RecentlyUsedFiles;
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.Exists)
                    {
                        var item = new Fluent.Button
                            {
                                Header = fi.Name,
                                ToolTip = fi.FullName,
                                Tag = file,
                            };

                        item.Click += (x, args) => 
                            ((MainWindow)Application.Current.MainWindow).OpenFile(item.Tag.ToString());
                        _mnuOpen.Children.Add(item);
                    }
                }                
            }
        }

    }
}
