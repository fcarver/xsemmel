using System;
using System.Windows;
using Microsoft.Win32;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class SaveCommand :  XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
            return ef.XmlEditor.IsModified;
        }

        public static void SaveWithoutQuestion(EditorFrame ef)
        {
            if (ef.XSDocument.Filename == null)
            {
                SaveFileDialog dlgSaveFile = new SaveFileDialog();
                dlgSaveFile.Filter = "Xml Files|*.xml|Xsd Files|*.xsd|Xslt Files|*.xslt|All Files|*.*";
                dlgSaveFile.Title = "Select a file to save";
                dlgSaveFile.FileName = ef.XSDocument.Filename;

                if (dlgSaveFile.ShowDialog() == true)
                {
                    try
                    {
                        MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
                        mainWnd.SaveFile(dlgSaveFile.FileName);

                        //TGW
                        if (ef.XSDocument.Filename != null && ef.XSDocument.Filename.ToLower().EndsWith(".xsd"))
                        {
                            Schema.Parser.IXsdNode nd = new Schema.Parser.SchemaParser(ef.XSDocument.Filename, false).GetVirtualRoot();
                            ef._xsdVisualizer.SetRoot(nd);
                            ef._xsdVisualizer.Refresh();
                            ef._xsdVisualizer._lblModified.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error", MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            } 
            else
            {
                try
                {
                    MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
                    mainWnd.SaveFile(ef.XSDocument.Filename);

                    //TGW
                    if (ef.XSDocument.Filename != null && ef.XSDocument.Filename.ToLower().EndsWith(".xsd"))
                    {
                        Schema.Parser.IXsdNode nd = new Schema.Parser.SchemaParser(ef.XSDocument.Filename, false).GetVirtualRoot();
                        ef._xsdVisualizer.SetRoot(nd);
                        ef._xsdVisualizer.Refresh();
                        ef._xsdVisualizer._lblModified.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        protected override void Execute(EditorFrame ef)
        {
            SaveWithoutQuestion(ef);
        }

    }
}
