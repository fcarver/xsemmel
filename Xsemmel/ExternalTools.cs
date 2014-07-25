using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Fluent;
using XSemmel.Configuration;

namespace XSemmel
{
    internal class ExternalTools
    {

        public ExternalTools()
        {
        }

        public bool IsAnySpecified()
        {
            return !string.IsNullOrEmpty(XSConfiguration.Instance.Config.ExternalTool1Filename) ||
                   !string.IsNullOrEmpty(XSConfiguration.Instance.Config.ExternalTool2Filename) ||
                   !string.IsNullOrEmpty(XSConfiguration.Instance.Config.ExternalTool3Filename);
        }

        public void Configure(RibbonGroupBox ribbongroupExternalTools)
        {
            ribbongroupExternalTools.Items.Clear();

            if (!string.IsNullOrEmpty(XSConfiguration.Instance.Config.ExternalTool1Filename))
            {
                string tool = XSConfiguration.Instance.Config.ExternalTool1Filename;
                string args = XSConfiguration.Instance.Config.ExternalTool1Arguments;

                Button btn = new Button();
                btn.Header = "(1) " + getAppname(tool);
                btn.LargeIcon = "/Images/Tools_Hammer_32x32.png";   //getIcon(tool).ToBitmap();
                btn.Click += (sender, e) => run(tool, args);
                ribbongroupExternalTools.Items.Add(btn);
            }

            if (!string.IsNullOrEmpty(XSConfiguration.Instance.Config.ExternalTool2Filename))
            {
                string tool = XSConfiguration.Instance.Config.ExternalTool2Filename;
                string args = XSConfiguration.Instance.Config.ExternalTool2Arguments;

                Button btn = new Button();
                btn.Header = "(2) " + getAppname(tool);
                btn.LargeIcon = "/Images/Tools_Hammer_32x32.png";   //getIcon(tool).ToBitmap();
                btn.Click += (sender, e) => run(tool, args);
                ribbongroupExternalTools.Items.Add(btn);
            }

            if (!string.IsNullOrEmpty(XSConfiguration.Instance.Config.ExternalTool3Filename))
            {
                string tool = XSConfiguration.Instance.Config.ExternalTool3Filename;
                string args = XSConfiguration.Instance.Config.ExternalTool3Arguments;

                Button btn = new Button();
                btn.Header = "(3) " + getAppname(tool);
                btn.LargeIcon = "/Images/Tools_Hammer_32x32.png";   //getIcon(tool).ToBitmap();
                btn.Click += (sender, e) => run(tool, args);
                ribbongroupExternalTools.Items.Add(btn);
            }
        }


        private void run(string tool, string args)
        {
            try
            {
                if (!File.Exists(tool))
                {
                    MessageBox.Show(
                        Application.Current.MainWindow, 
                        "The specified application was not found: '" + tool + "'", 
                        "File not found");
                    return;
                }

                Application.Current.MainWindow.Cursor = Cursors.Wait;

                Process p = new Process();
                p.StartInfo.FileName = tool;
                p.StartInfo.Arguments = expandArguments(args);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                MessageBox.Show(Application.Current.MainWindow, output, "Output");
            }
            catch (Exception ex)
            {
                Application.Current.MainWindow.Cursor = null;
                MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error");
            }
            finally
            {
                Application.Current.MainWindow.Cursor = null;
            }
        }


        private string expandArguments(string args)
        {
            string res = args;

            var editor = ((MainWindow) Application.Current.MainWindow).Data.EditorFrame;

            string item = editor.XSDocument.Filename;

            res = res.Replace("$(ItemPath)", item);
            res = res.Replace("$(ItemDir)", Path.GetDirectoryName(item));
            res = res.Replace("$(ItemFilename)", Path.GetFileName(item));
            res = res.Replace("$(ItemExt)", Path.GetExtension(item));
            res = res.Replace("$(ItemFilenameWithoutExt)", Path.GetFileNameWithoutExtension(item));
            if (res.Contains("$(CurText"))
            {
                res = res.Replace("$(CurText)", editor.XmlEditor.SelectedText);
            }
            if (res.Contains("$(CurLine") || res.Contains("$(CurCol)"))
            {
                var loc = editor.XmlEditor.Document.GetLocation(editor.XmlEditor.CaretOffset);
                res = res.Replace("$(CurLine)", loc.Line.ToString(CultureInfo.InvariantCulture));
                res = res.Replace("$(CurCol)", loc.Column.ToString(CultureInfo.InvariantCulture));
            }

            return res;
        }

        private string getAppname(string filename)
        {
            try
            {
                var fvi = FileVersionInfo.GetVersionInfo(filename);
                string productname = fvi.ProductName;

                if (!string.IsNullOrEmpty(productname))
                {
                    return productname;
                }
                else
                {
                    return Path.GetFileNameWithoutExtension(filename);
                }
            }
            catch (FileNotFoundException)
            {
                return filename + " (not found)";
            }
        }


//        private Icon getIcon(string exename)
//        {
//            try
//            {
//                return Icon.ExtractAssociatedIcon(exename);
//            }
//            catch
//            {
//                return null;
//            }
//        }

    }
}