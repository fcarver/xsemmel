using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.XPath;
using XSemmel.Helpers;

namespace XSemmel.XPath
{
    static class BulkXPath
    {

        private static BulkXPathInputBox _input;

        public static void ShowDialog(Window owner)
        {
            var w = new BulkWindow
            {
                Owner = owner,
                Icon = BitmapFrame.Create(
                    new Uri("pack://application:,,,/Images/Binoculars_16x16.png",
                    UriKind.RelativeOrAbsolute)),
                Title = "Bulk XPath scanning",
                OnSearchClicked = onSearchClicked,
                GetUserObject = getUserObject,
            };

            _input = new BulkXPathInputBox();

            w.InputControl = _input;
            w.ShowDialog();
        }

        private static object getUserObject()
        {
            return _input.EdtXPath.Text;
        }

        private static string onSearchClicked(BulkWindow.BulkCommunicator bc, ICollection<string> files, object userObject)
        {
            bc.ReportProgress(string.Format("Found {0} files to scan", files.Count));

            StringBuilder edtResult = new StringBuilder();

            try
            {
                XPathExpression expr = XPathExpression.Compile(userObject.ToString());

                foreach (string file in files)
                {
                    try
                    {
                        bc.ReportProgress("Scanning file: " + file);
                        edtResult.AppendLine("Result of file: " + file);

                        XPathDocument doc = new XPathDocument(file);
                        var nav = doc.CreateNavigator();

                        var result = nav.Evaluate(expr);
                        if (result is Boolean || result is Double || result is string)
                        {
                            edtResult.AppendFormat("{0}\n", result);
                        }
                        else if (result is XPathNodeIterator)
                        {
                            DisplayNavigator((XPathNodeIterator) result, edtResult);
                        }
                        else
                        {
                            Debug.Fail("result is " + result.GetType());
                        }
                    }
                    catch (Exception e)
                    {
                        edtResult.AppendLine("Error while scanning " + file + ": " + e.Message);
                    }
                }

                edtResult.AppendLine("Scanning completed");
            }
            catch (Exception e)
            {
                edtResult.AppendLine("Error: " + e.Message);
            }
            return edtResult.ToString();
        }


        private static void DisplayNavigator(XPathNodeIterator xpi, StringBuilder edtResult)
        {
            if ((xpi != null) && (xpi.Count > 0))
            {
                for (bool hasNext = xpi.MoveNext(); hasNext; hasNext = xpi.MoveNext())
                {
                    edtResult.AppendLine(xpi.Current.OuterXml);
                }
            }

            edtResult.AppendLine();
        }


    }
}
