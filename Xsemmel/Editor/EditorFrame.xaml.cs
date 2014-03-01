using System;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using ICSharpCode.AvalonEdit;
using XSemmel.Commands;

namespace XSemmel.Editor
{

    public partial class EditorFrame
    {

        public XSDocument XSDocument { get; private set; }

        public Data Data { get; private set; }


        public EditorFrame() : this(new XSDocument(""), new Data())
        {
        }

        public EditorFrame(XSDocument doc, Data data)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            InitializeComponent();

            Data = data;
            Data.EditorFrame = this;
            XSDocument = doc;
            
            try
            {
                if (doc.XsdFile != null)
                {
                    Data.ValidationData.Xsd = doc.XsdFile;
                }    
            }
            catch (Exception)
            {
                Data.ValidationData.Xsd = null;
            }
            

            if (!(doc.Filename != null && doc.Filename.ToLower().EndsWith(".xsd")))
            {
                _xsdPropertiesDockable.Visibility = Visibility.Collapsed;
                _xsdVisualizerDockable.Visibility = Visibility.Collapsed;
            }

            _xmlEditor.Editor = this;
            _editorValidation.ValidationData = data.ValidationData;
            _editorValidation.XSDocument = doc;
            
            _gridView.Editor = this;
            _editorValidation.Editor = this;
            _editorTree.Editor = this;
            _webBrowser.Editor = this;
            _xPathQuery.Editor = this;
            _xPathSearchAndReplace.Editor = this;
            _xsdVisualizer.Editor = this;
//            _schemaInfo.Editor = this;

            _xPathSearchAndReplaceDockable.Visibility = Visibility.Collapsed;

            _editorTreeDockable.Activate();
            _editorValidationDockable.Activate();
            _xmlEditorDockable.Activate();
        }

        public void SetFragmentText(string fragment, bool showDockableIfHidden = false)
        {
            if (showDockableIfHidden)
            {
                _fragmentDockable.Show();
                _fragmentDockable.Visibility = Visibility.Visible;
            }
            _fragment.SetText(fragment);
        }

        public TextEditor XmlEditor
        {
            get 
            {
                return _xmlEditor.TextEditor;
            }
        }

        public void StartSearch()
        {
            ((FindString) _xmlEditor._presenterFindString.Content).StartSearch();
        }

        public void SetSchemaInfo(IXmlSchemaInfo schemaInfo)
        {
            _schemaInfoDockable.Show();
            _schemaInfoDockable.Visibility = Visibility.Visible;
            _schemaInfo.SetSchemaInfo(schemaInfo);
        }

        internal void SetXsdFile(string xsdFile)
        {
            XSDocument.XsdFile = xsdFile;
            _xmlEditor.SetXsdFile(xsdFile);
            _editorTree.Update();
        }

        internal bool AllowToClose()
        {
            if (_xmlEditor.TextEditor.IsModified)
            {
                var result = MessageBox.Show(Application.Current.MainWindow, "Save changes before closing?",
                    "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveCommand.SaveWithoutQuestion(this);
                }
                if (result == MessageBoxResult.No)
                {
                    return true;
                }
                if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private void dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            //(De-)activate ContextualRibbonTabs
            var actContent = dockManager.ActiveContent;
            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            if (actContent == _editorTreeDockable)
            {
                wnd.contextribTree.Visibility = Visibility.Visible;
                wnd.Ribbon.SelectedTabItem = wnd.ribbonTabTreeView;
            }
            else
            {
                wnd.contextribTree.Visibility = Visibility.Collapsed;
                if (wnd.Ribbon.SelectedTabItem == wnd.ribbonTabTreeView)
                {
                    wnd.Ribbon.SelectedTabItem = null;
                }
            }
            //            wnd.contextribGrid.Visibility = (actContent == gridViewDockable) ? Visibility.Visible : Visibility.Collapsed;
        }

     }
}
