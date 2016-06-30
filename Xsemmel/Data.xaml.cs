using XSemmel.Editor;
using XSemmel.Helpers.WPF;

namespace XSemmel
{

    public class ValidationData : PropertyChangeNotifierBase
    {
        private readonly Data _data;

        private bool _checkWellformedness = true;
        private bool _doNotValidate;
        private bool _checkXsd;

        private string _xsdFile;


        public ValidationData(Data data)
        {
            _data = data;
        }

        public bool DoNotValidate
        {
            get
            {
                return _doNotValidate;
            }
            set 
            {
                _doNotValidate = value;
                if (value)
                {
                    CheckWellformedness = false;
                    CheckXsd = false;
                }
                OnPropertyChanged("DoNotValidate");
            }
        }
        public bool CheckWellformedness
        {
            get
            {
                return _checkWellformedness;
            }
            set
            {
                _checkWellformedness = value;
                if (value)
                {
                    DoNotValidate = false;
                    CheckXsd = false;
                }
                OnPropertyChanged("CheckWellformedness");
            }
        }
        public bool CheckXsd
        {
            get
            {
                return _checkXsd;
            }
            set
            { 
                _checkXsd = value;
                if (value)
                {
                    DoNotValidate = false;
                    CheckWellformedness = false;
                }
                OnPropertyChanged("CheckXsd");
            }
        }


        public string Xsd
        {
            get
            {
                return _xsdFile;
            }
            set 
            { 
                if (_data.EditorFrame != null)
                {
                    string filename = value;
                    filename = filename?.Trim('\"');

                    _xsdFile = filename;
                    _data.EditorFrame.SetXsdFile(filename);
                }
                OnPropertyChanged("Xsd");
            }
        }

    }


    public class XsltData : PropertyChangeNotifierBase
    {
        private readonly Data _data;

        private bool _xsltInEditor = true;
        private bool _xmlInEditor;

        private string _file;


        public XsltData(Data data)
        {
            _data = data;
        }

        public bool XmlInEditor
        {
            get
            {
                return _xmlInEditor;
            }
            set
            {
                _xmlInEditor = value;
                if (value)
                {
                    XsltInEditor = false;
                }
                OnPropertyChanged("XmlInEditor");
            }
        }
        public bool XsltInEditor
        {
            get
            {
                return _xsltInEditor;
            }
            set
            {
                _xsltInEditor = value;
                if (value)
                {
                    XmlInEditor = false;
                }
                OnPropertyChanged("XsltInEditor");
            }
        }

        public string File
        {
            get
            {
                return _file;
            }
            set
            {
                if (_data.EditorFrame != null)
                {
                    if (value != null)
                    {
                        _file = value.Trim('\"');
                    }
                    else
                    {
                        _file = value;                        
                    }
                }
                OnPropertyChanged("File");
            }
        }

    }

    public class PrettyPrintData : PropertyChangeNotifierBase
    {
        private readonly Data _data;

        private bool _indent = true;
        private bool _newLineOnAttribute = false;

        public PrettyPrintData(Data data)
        {
            _data = data;
        }

        public bool Indent
        {
            get
            {
                return _indent;
            }
            set 
            {
                _indent = value;
                OnPropertyChanged("Indent");
            }
        }
        public bool NewLineOnAttributes
        {
            get
            {
                return _newLineOnAttribute;
            }
            set
            { 
                _newLineOnAttribute = value;
                OnPropertyChanged("NewLineOnAttributes");
            }
        }

    }


    public class XsdVisualizerData : PropertyChangeNotifierBase
    {
        private readonly Data _data;
        private bool _expandRefs = true;
        private bool _expandTypes;
        private bool _hideTypes;
        private bool _expandIncludes;
        private bool _hideIncludes = true;

        public XsdVisualizerData(Data data)
        {
            _data = data;
        }

        public bool ExpandRefs
        {
            get
            {
                return _expandRefs;
            }
            set
            {
                _expandRefs = value;
                OnPropertyChanged("ExpandRefs");
                _data.EditorFrame?._xsdVisualizer?.Refresh();
            }
        }

        public bool ExpandTypes
        {
            get
            {
                return _expandTypes;
            }
            set
            {
                _expandTypes = value;
                OnPropertyChanged("ExpandTypes");
                _data.EditorFrame?._xsdVisualizer?.Refresh();
            }
        }

        public bool HideTypes
        {
            get
            {
                return _hideTypes;
            }
            set
            {
                _hideTypes = value;
                OnPropertyChanged("HideTypes");
                _data.EditorFrame?._xsdVisualizer?.Refresh();
            }
        }

        public bool ExpandIncludes
        {
            get
            {
                return _expandIncludes;
            }
            set
            {
                _expandIncludes = value;
                OnPropertyChanged("ExpandIncludes");
                _data.EditorFrame?._xsdVisualizer?.Refresh();
            }
        }

        public bool HideIncludes
        {
            get
            {
                return _hideIncludes;
            }
            set
            {
                _hideIncludes = value;
                OnPropertyChanged("HideIncludes");
                _data.EditorFrame?._xsdVisualizer?.Refresh();
            }
        }

    }

    public class Data : PropertyChangeNotifierBase
    {
        private readonly ValidationData _validationData;
        private readonly PrettyPrintData _prettyPrintData;
        private readonly XsdVisualizerData _xsdVisualizerData;
        private readonly XsltData _xsltData;

        public Data()
        {
            _validationData = new ValidationData(this);
            _prettyPrintData = new PrettyPrintData(this);
            _xsdVisualizerData = new XsdVisualizerData(this);
            _xsltData = new XsltData(this);
        }

        public EditorFrame EditorFrame { get; set; }

     
        public ValidationData ValidationData
        {
            get { return _validationData; }
        }

        public PrettyPrintData PrettyPrintData
        {
            get { return _prettyPrintData; }
        }

        public XsdVisualizerData XsdVisualizerData
        {
            get { return _xsdVisualizerData; }
        }

        public XsltData XsltData
        {
            get { return _xsltData; }
        }

        public bool WordWrap
        {
            get
            {
                if (EditorFrame?.XmlEditor != null)
                {
                    return EditorFrame.XmlEditor.WordWrap;
                }
                return false;
            }
            set
            {
                if (EditorFrame?.XmlEditor != null)
                {
                    EditorFrame.XmlEditor.WordWrap = value;
                    OnPropertyChanged("WordWrap");
                }
            }
        }

        public bool ShowSpecialCharacters
        {
            get
            {
                if (EditorFrame?.XmlEditor != null)
                {
                    return EditorFrame.XmlEditor.Options.ShowEndOfLine;
                }
                return false;
            }
            set
            {
                if (EditorFrame?.XmlEditor != null)
                {
                    EditorFrame.XmlEditor.Options.ShowBoxForControlCharacters = value;
                    EditorFrame.XmlEditor.Options.ShowEndOfLine = value;
                    EditorFrame.XmlEditor.Options.ShowSpaces = value;
                    EditorFrame.XmlEditor.Options.ShowTabs = value;
                    OnPropertyChanged("ShowSpecialCharacters");
                }
            }
        }

        public bool ShowFindDialog 
        {
            get
            {
                return false;
            }
            set 
            {
                if (EditorFrame != null && EditorFrame._xPathQueryDockable != null)
                {
                    EditorFrame._xPathQueryDockable.Focus();
                }
                OnPropertyChanged("ShowFindDialog");
            }
        }

        public bool UpdateTreeViewAutomatically
        {
            get
            {
                if (EditorFrame != null && EditorFrame._editorTree != null)
                {
                    return EditorFrame._editorTree.UpdateAutomatically;
                }
                return true;
            }
            set
            {
                if (EditorFrame != null && EditorFrame._editorTree != null)
                {
                    EditorFrame._editorTree.UpdateAutomatically = value;
                }
                OnPropertyChanged("UpdateTreeViewAutomatically");
            }
        }
    }
}