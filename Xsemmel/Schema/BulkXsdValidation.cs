using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using Microsoft.Win32;
using XSemmel.Helpers;

namespace XSemmel.Schema
{
    static class BulkXsdValidation
    {

        private static TextBox edtXsdFile;

        public static void ShowDialog(Window owner)
        {
            var w = new BulkWindow
            {
                Owner = owner,
                Icon = BitmapFrame.Create(
                    new Uri("pack://application:,,,/Images/112_Tick_Green_16x16.png",
                    UriKind.RelativeOrAbsolute)),
                Title = "Bulk XSD Validation",
                OnSearchClicked = onSearchClicked,
                GetUserObject = getUserObject,
                InputControl = getInputBox(),
            };
            
            w.ShowDialog();
        }

        private static object getUserObject()
        {
            return edtXsdFile.Text;
        }

        private static string onSearchClicked(BulkWindow.BulkCommunicator bc, ICollection<string> files, object userObject)
        {
            bc.ReportProgress(string.Format("Found {0} files to scan", files.Count));

            StringBuilder edtResult = new StringBuilder();
            int countFiles = 0, countWarnings = 0, countErrors = 0, countSuccess = 0;

            try
            {
                foreach (string file in files)
                {
                    try
                    {
                        bc.ReportProgress("Scanning file: " + file);
                        edtResult.Append("Result of file: " + file+" ...");

                        try
                        {
                            countFiles++;

                            XmlDocument xmldoc = File.ReadAllText(file).ToXmlDocument();

                            XsdValidationResult result = XsdValidationHelper.Instance.ValidateInstance(
                                userObject.ToString(), File.ReadAllText(file));
                            switch (result.State)
                            {
                                case ValidationState.Success:
                                    edtResult.Append( " valid");
                                    countSuccess++;
                                    break;

                                case ValidationState.Warning:
                                    edtResult.Append( " warning");
                                    countWarnings++;
                                    break;

                                case ValidationState.ValidationError:
                                    edtResult.Append( " error");
                                    countErrors++;
                                    break;

                                case ValidationState.OtherError:
                                    edtResult.Append( " other error");
                                    countErrors++;
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            edtResult.Append( ex.Message);
                            countErrors++;
                        }
                        edtResult.AppendLine();

                    }
                    catch (Exception e)
                    {
                        edtResult.AppendLine("Error while scanning " + file + ": " + e.Message);
                    }
                }

                edtResult.AppendLine();
                edtResult.AppendFormat("{0} files, {1} valid, {2} valid with warnings, {3} errors",
                            countFiles, countSuccess, countWarnings, countErrors);
                edtResult.AppendLine();
                edtResult.AppendLine("Scanning completed");
            }
            catch (Exception e)
            {
                edtResult.AppendLine("Error: " + e.Message);
            }
            return edtResult.ToString();
        }



        private static void btnSelectXsdFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "Xsd Files (*.xsd)|*.xsd";
            dlgOpenFile.Title = "Select an Xsd file to open";
            //dlgOpenFile.InitialDirectory = Path.GetDirectoryName(_mainWindow.CurrentFileName);
            if (FileHelper.FileExists(edtXsdFile.Text))
            {
                dlgOpenFile.FileName = edtXsdFile.Text;
            }
            if ((dlgOpenFile.ShowDialog() == true) && (string.Compare(edtXsdFile.Text, dlgOpenFile.FileName, true) != 0))
            {
                edtXsdFile.Text = dlgOpenFile.FileName;
            }
        }

        private static UIElement getInputBox()
        {
            DockPanel pnl = new DockPanel
                                {
                                    HorizontalAlignment = HorizontalAlignment.Stretch
                                };

            TextBlock lbl = new TextBlock {Text = "Xsd file:", Width = 170};
            DockPanel.SetDock(lbl, Dock.Left);
            pnl.Children.Add(lbl);

            Button btn = new Button {Width = 20, Content = "..."};
            btn.Click += btnSelectXsdFile_Click;
            DockPanel.SetDock(btn, Dock.Right);
            pnl.Children.Add(btn);

            edtXsdFile = new TextBox();
            pnl.Children.Add(edtXsdFile);

            return pnl;
        }

    }
}
