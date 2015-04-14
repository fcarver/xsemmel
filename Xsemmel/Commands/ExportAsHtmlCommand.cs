using System;
using System.IO;
using ICSharpCode.AvalonEdit;
using System.Windows;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class ExportAsHtmlCommand :  XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
//            if (!ef.XmlEditor.TextArea.IsFocused)
//            {
//                return false;
//            }
            if (ef.XSDocument == null)
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            TextEditor textEditor = ef.XmlEditor;

            if (textEditor == null)
            {
                throw new Exception("No XmlEditor");
            }

            SaveFileDialog dlgSaveFile = new SaveFileDialog();
            dlgSaveFile.Filter = "Html Files|*.html|All Files|*.*";
            dlgSaveFile.Title = "Select an Html file to save";
            if (string.IsNullOrWhiteSpace(ef.XSDocument.Filename))
            {
                dlgSaveFile.FileName = "undefined.html";
            }
            else
            {
                dlgSaveFile.FileName = ef.XSDocument.Filename + ".html";    
            }

            if (dlgSaveFile.ShowDialog() == true)
            {
                try
                {
                    IHighlighter highlighter = new DocumentHighlighter(
                        textEditor.Document, textEditor.SyntaxHighlighting.MainRuleSet);
                    string html = HtmlClipboard.CreateHtmlFragment(
                        textEditor.Document, highlighter, null, new HtmlOptions());

                    File.WriteAllText(dlgSaveFile.FileName, html);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

    }
}
