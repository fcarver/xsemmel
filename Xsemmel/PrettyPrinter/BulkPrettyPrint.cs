using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using XSemmel.Helpers;

namespace XSemmel.PrettyPrinter
{
    static class BulkPrettyPrint
    {

        public static void ShowDialog(Window owner)
        {
            var w = new BulkWindow
            {
                Owner = owner,
                Icon = BitmapFrame.Create(
                    new Uri("pack://application:,,,/Images/Prettyprint_16x16.png",
                    UriKind.RelativeOrAbsolute)),
                Title = "Bulk pretty print files",
                OnSearchClicked = onSearchClicked
            };

//            w.InputControl = new TextBox {Height = 50};
            w.ShowDialog();
        }

        private static string onSearchClicked(BulkWindow.BulkCommunicator bc, ICollection<string> files, object userObject)
        {
            bc.ReportProgress(string.Format("Found {0} files to pretty print", files.Count));

            StringBuilder errorlog = new StringBuilder();

            foreach (string file in files)
            {
                try
                {
                    string newFileName = Path.ChangeExtension(file, ".pretty.xml");
                    bc.ReportProgress("Prettyprinting file: " + file);
                    string xml = File.ReadAllText(file);
                    string res = PrettyPrint.Execute(xml, true, false);
                    File.WriteAllText(newFileName, res);
                }
                catch (Exception e)
                {
                    errorlog.AppendLine("Error while prettyprinting " + file + ": " + e.Message);
                }
            }

            errorlog.AppendLine("Prettyprinting completed");
            return errorlog.ToString();
        }

    }
}
