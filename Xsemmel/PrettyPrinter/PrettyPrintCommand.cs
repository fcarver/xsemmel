using System;
using ICSharpCode.AvalonEdit;
using System.Windows;
using XSemmel.Commands;
using XSemmel.Editor;

namespace XSemmel.PrettyPrinter
{
    class PrettyPrintCommand :  XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef == null || ef.XmlEditor == null)
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            TextEditor te = ef.XmlEditor;

            //TODO undo works this way
//                string vorher = te.Text;
//                te.Document.Remove(0, te.Document.TextLength);
//                te.Document.Insert(0, PrettyPrint.Execute(
//                    vorher,
//                    ef.Data.PrettyPrintData.Indent,
//                    ef.Data.PrettyPrintData.NewLineOnAttributes
//                ));

            if (te.SelectionLength != 0)
            {
                try 
                {
                    te.SelectedText = PrettyPrint.Execute(
                        te.SelectedText, 
                        ef.Data.PrettyPrintData.Indent,
                        ef.Data.PrettyPrintData.NewLineOnAttributes
                    );
                }
                catch (Exception e)
                {
                    MessageBox.Show(Application.Current.MainWindow, "Cannot prettyprint selected text:\n" + e.Message, 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try 
                {
                    te.Text = PrettyPrint.Execute(
                        te.Text,
                        ef.Data.PrettyPrintData.Indent,
                        ef.Data.PrettyPrintData.NewLineOnAttributes
                    );
                }
                catch (Exception e)
                {
                    MessageBox.Show(Application.Current.MainWindow, "Cannot prettyprint text:\n" + e.Message, 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }

}
