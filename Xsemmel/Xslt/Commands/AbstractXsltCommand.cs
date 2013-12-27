using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Xsl;
using XSemmel.Commands;
using XSemmel.Configuration;
using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.Xslt.Commands
{
    abstract class AbstractXsltCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(ef.Data.XsltData.File))
            {
                return false;
            }
            return true;
        }


        protected override void Execute(EditorFrame ef)
        {
            try
            {
                var encoding = XSConfiguration.Instance.Config.Encoding;

                XmlDocument xml;
                string xslt;

                if (ef.Data.XsltData.XmlInEditor)
                {
                    xml = ef.XmlEditor.Text.ToXmlDocument();
                    xslt = File.ReadAllText(ef.Data.XsltData.File);
                }
                else
                {
                    string x = File.ReadAllText(ef.Data.XsltData.File);
                    xml = x.ToXmlDocument();
                    xslt = ef.XmlEditor.Text;
                }

                var result = new XsltTransformer().Transform(xml, xslt, encoding);
                SetResult(result);
            } 
            catch (Exception ex)
            {
                string exc = ex.Message;
                if (ex.InnerException != null)
                {
                    exc += ex.InnerException.Message;
                }
                MessageBox.Show(Application.Current.MainWindow, "Error: " + exc, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }



        protected abstract void SetResult(string result);

    }
}
