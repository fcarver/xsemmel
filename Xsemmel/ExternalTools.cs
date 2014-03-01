using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Fluent;
using XSemmel.Configuration;
using XSemmel.Editor;

namespace XSemmel
{
    internal class ExternalTools
    {

        private readonly EditorFrame _editor;

        public ExternalTools(EditorFrame editor)
        {
            _editor = editor;
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
                Application.Current.MainWindow.Cursor = Cursors.Wait;
                MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error");
            }
            finally
            {
                Application.Current.MainWindow.Cursor = Cursors.Wait;
            }
        }


        private string expandArguments(string args)
        {
            string res = args;

            string item = _editor.XSDocument.Filename;

            res = res.Replace("$(ItemPath)", item);
            res = res.Replace("$(ItemDir)", Path.GetDirectoryName(item));
            res = res.Replace("$(ItemFilename)", Path.GetFileName(item));
            res = res.Replace("$(ItemExt)", Path.GetExtension(item));
            res = res.Replace("$(ItemFilenameWithoutExt)", Path.GetFileNameWithoutExtension(item));
            if (res.Contains("$(CurText"))
            {
                res = res.Replace("$(CurText)", _editor.XmlEditor.SelectedText);
            }
            if (res.Contains("$(CurLine") || res.Contains("$(CurCol)"))
            {
                var loc = _editor.XmlEditor.Document.GetLocation(_editor.XmlEditor.CaretOffset);
                res = res.Replace("$(CurLine)", loc.Line.ToString());
                res = res.Replace("$(CurCol)", loc.Column.ToString());
            }

            return res;
        }

        private string getAppname(string filename)
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