using System;
using System.IO;
using System.Windows;
using HtmlAgilityPack;
using XSemmel.Configuration;
using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.Commands
{
    class TidyInvalidXmlCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            return ef.XmlEditor != null;
        }

        protected override void Execute(EditorFrame ef)
        {
            try
            {
                ef.XmlEditor.Text.ToXmlDocument();
                MessageBox.Show(Application.Current.MainWindow, "Document is well-formed. No need to tidy.",
                                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                try
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.OptionOutputAsXml = true;
                    doc.LoadHtml(ef.XmlEditor.Text);

    //                foreach (var s in doc.ParseErrors)
    //                {
    //                    Console.WriteLine(s);
    //                }

                    MemoryStream ms = new MemoryStream();
                    using (TextWriter sw = new StreamWriter(ms, doc.Encoding)) //Set encoding
                    {
                        doc.Save(sw);
                    }

                    var result = XSConfiguration.Instance.Config.Encoding.GetString(ms.ToArray());

                    ef.XmlEditor.Text = result;
                }
                catch (Exception e)
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error: " + e.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

    }
}
