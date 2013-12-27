using System;
using System.Windows;
using Microsoft.Xml.XQuery;

namespace XSemmel.XQuery
{
    /// <summary>
    /// Interaction logic for XQuery.xaml
    /// </summary>
    public partial class XQuery
    {
        private readonly XSDocument _document;

        public XQuery(XSDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            _document = doc;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XQueryNavigatorCollection col = new XQueryNavigatorCollection();
                col.AddNavigator(_document.Navigator, "doc");
                string query = _edtXQuery.Text;
                XQueryExpression xepr = new XQueryExpression(query);
                _edtResult.Text = xepr.Execute(col).ToXml();
            }
            catch (Exception ex)
            {
                _edtResult.Text = ex.ToString();
                _edtResult.Text += "\n\n";
                _edtResult.Text += _edtXQuery.Text;
            }
        }
    }
}
