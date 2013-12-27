using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Windows.Controls.Ribbon;
using XSemmel.Configuration;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace XSemmel
{
    public partial class ApplicationMenu
    {
        
        public ApplicationMenu()
        {
            InitializeComponent();

            FormattedText atoz = new FormattedText("File",
                CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyles.Normal, FontWeights.Normal, new FontStretch()),
                12.0, Brushes.White);
            Geometry geo = atoz.BuildGeometry(new Point(0, 0));
            _fileIcon.Geometry = geo;
            _fileIcon.Brush = Brushes.White;
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            new ConfigurationWnd {Owner = Application.Current.MainWindow}.ShowDialog();
        }

        private void RibbonApplicationMenu_DropDownOpened(object sender, EventArgs e)
        {
            while (_mnuOpen.Items.Count > 3)
            {
                _mnuOpen.Items.RemoveAt(3);
            }

            var files = XSConfiguration.Instance.Config.RecentlyUsedFiles;
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Exists)
                {
                    var item = new RibbonApplicationMenuItem
                                   {
                                       Header = fi.Name, 
                                       ToolTip = fi.FullName, 
                                       Tag = file,
                                   };
                    item.Click += (x, args) => 
                        ((MainWindow)Application.Current.MainWindow).OpenFile(item.Tag.ToString());
                    _mnuOpen.Items.Add(item);
                }
            }
            _mnuOpen.IsSubmenuOpen = true;
        }
    }
}
