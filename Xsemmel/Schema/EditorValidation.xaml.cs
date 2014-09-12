using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using XSemmel.Helpers;
using System.Diagnostics;
using XSemmel.Editor;

namespace XSemmel.Schema
{
    public partial class EditorValidation
    {

        private XSDocument _document;
        private EditorFrame _editor;
        private readonly DispatcherTimer _validationUpdateTimer;

        private ValidationData _validationData;

        private MarkBackgroundRenderer _errorMarker;


        public EditorValidation()
        {
            InitializeComponent();

            _validationUpdateTimer = new DispatcherTimer();
            _validationUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            _validationUpdateTimer.Tick += validationUpdateTimer_Tick;
        }

        public EditorFrame Editor
        {
            set
            {
                if (_editor != null)
                {
                    throw new Exception("Editor already set");
                }
                _editor = value;

                _errorMarker = new MarkBackgroundRenderer(_editor.XmlEditor);
                _editor.XmlEditor.TextArea.TextView.BackgroundRenderers.Add(_errorMarker);

                StartValidation();
            }
        }

        public ValidationData ValidationData
        {
            set { _validationData = value; }
        }

        public void StartValidation()
        {
            _validationUpdateTimer.Start();
        }

        public void StopValidation()
        {
            _validationUpdateTimer.Stop();
            _lstErrors.Items.Clear();
        }

        public XSDocument XSDocument
        {
            set
            {
                if (_document != null)
                {
                    throw new Exception("XSDocument already set");
                }
                _document = value;

                string xsd = _document.XsdFile;
                if (xsd != null && File.Exists(xsd))
                {
                    _validationData.Xsd = xsd;
                    _validationData.CheckXsd = true;
                }
                else
                {
                    _validationData.CheckWellformedness = true;
                }
            }
        }

        void validationUpdateTimer_Tick(object sender, EventArgs e)
        {
            Debug.Assert(_editor != null);

            _lstErrors.Items.Clear();
            _errorMarker.ClearMarks();

            try
            {
                if (_validationData.DoNotValidate)
                {
                    var i1 = new ValidationIssue(ValidationIssue.Type.Information, 0, 0, "Validation is turned off.\nTo activate, click in ribbon on 'Validation' tab and select 'Check wellformedness' or 'Validate against schema'.");
                    _lstErrors.Items.Add(i1);
                }
                
                else if (_validationData.CheckWellformedness)
                {

                    try
                    {
                        XmlDocument xmlDoc = _editor.XmlEditor.Text.ToXmlDocument();  //TODO make async
                        var i = new ValidationIssue(ValidationIssue.Type.Information, 0, 0, "XML is wellformed");
                        _lstErrors.Items.Add(i);
                    }
                    catch (XmlException ex)
                    {
                        var i = new ValidationIssue(ValidationIssue.Type.Error, ex.LineNumber, ex.LinePosition,
                                                    ex.Message);
                        _lstErrors.Items.Add(i);
                    }
                }

                else if (_validationData.CheckXsd)
                {
                    string xsdFile = _validationData.Xsd;
                    if (!FileHelper.FileExists(xsdFile))
                    {
                        var i = new ValidationIssue(ValidationIssue.Type.Warning, 0, 0, 
                            string.Format("The file '{0}' does not exist, or the path is invalid", xsdFile));
                        _lstErrors.Items.Add(i);
                        return;
                    }

                    XsdValidationResult result = XsdValidationHelper.Instance.ValidateInstance(
                        xsdFile, _editor.XmlEditor.Text);
                    if (result.Results.Count == 0)
                    {
                        var i = new ValidationIssue(ValidationIssue.Type.Information, 0, 0, "XML is valid");
                        _lstErrors.Items.Add(i);
                    }
                    else
                    {
                        foreach (var x in result.Results)
                        {
                            _lstErrors.Items.Add(x);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var i = new ValidationIssue(ValidationIssue.Type.Error, 0, 0, "Error while validating file: "+ ex.Message);
                _lstErrors.Items.Add(i);
            }

            foreach (var item in _lstErrors.Items)
            {
                var error = item as ValidationIssue;
                if (error != null)
                {
                    if (error.IssueType != ValidationIssue.Type.Information)
                    {
                        int offset = error.Line == 0 ? 0 : _editor.XmlEditor.Document.GetOffset(error.Line, error.Column);
                        _errorMarker.AddOffsetToMark(new MarkBackgroundRenderer.Mark
                        {
                            Offset = offset,
                            Length = 1,
                            Brush = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0, 0))
                        });
                    }
                }
            }
        }

        private void lstErrors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidationIssue vi = _lstErrors.SelectedItem as ValidationIssue;
            if (vi != null && vi.Line != 0) 
            {
                int offset = _editor.XmlEditor.Document.GetOffset(vi.Line, vi.Column);
                _editor.XmlEditor.CaretOffset = offset;
                _editor.XmlEditor.ScrollToLine(vi.Line);
                _editor.XmlEditor.Focus();
            }
        }
    
    }
}
