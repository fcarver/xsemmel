using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using XSemmel.Editor;
using XSemmel.Schema.Parser;
using System;

namespace XSemmel.Schema
{

    public partial class XsdVisualizer
    {

        public class PaintOptions
        {
            public bool ExpandReferences { get; set; }
            public bool ExpandTypes { get; set; }
            public bool ExpandIncludes { get; set; }
            public bool HideTypes { get; set; }
            public bool HideImportIncludes { get; set; }
            public XsdVisualizer Visualizer { get; set; }
            public HashSet<IXsdNode> DontPaint { get; set; }
        }

        private class HistoryItem
        {
            public IXsdNode node;
            public SchemaParser parser;
        }

        private IXsdNode _root;
        
        private SchemaParser _schema;

        private readonly HashSet<IXsdNode> _dontPaint = new HashSet<IXsdNode>();
        private EditorFrame _editor;

        private readonly Stack<HistoryItem> _history = new Stack<HistoryItem>();

        private int _fontSize = 15;

        private string _currentFile;


        public XsdVisualizer()
        {
            InitializeComponent();
         //   _currentFile = xsdFile;

//            XmlNode dummy = new XmlDocument().CreateNode(XmlNodeType.Element, "element", "");
//            XsdElement root = new XsdElement(dummy);
//            root.AddAtts(new XsdAttribute() {Name = "Attr"});
//            _root = root;
        }

        private IXsdNode Root
        {
            get
            {
                return _root;
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("value is null");
                }
                _root = value;
                _history.Push(new HistoryItem { node = value, parser = Schema });
                Refresh();
            }
        }

        public void SetRoot(IXsdNode node)
        {
            Root = node;
        }

        private SchemaParser Schema
        {
            get
            {
                return _schema;
            }
            set
            {
                _schema = value;
                Root = value.GetVirtualRoot();
                //initLstElements();
            }
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
                Debug.Assert(_editor.XSDocument != null);
                _currentFile = _editor.XSDocument.Filename;
            }
        }


        public void SetDescription(IXsdNode o)
        {
            if (_editor == null)
            {
                return;
            }

            _editor._lstXsdSelectedProperties.Items.Clear();
            _editor._lstXsdSelectedProperties.Items.Add(string.Format("{0}", o.GetType().Name));

            PropertyInfo[] props = o.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in props)
            {
                object value = propertyInfo.GetValue(o, null);
                if (value != null)
                {
                    if (value is ICollection)
                    {
                        foreach (object item in (IEnumerable)value)
                        {
                            _editor._lstXsdSelectedProperties.Items.Add(string.Format("{0}: {1}", propertyInfo.Name, item));
                        }
                    }
                    else
                    {
                        _editor._lstXsdSelectedProperties.Items.Add(string.Format("{0}: {1}", propertyInfo.Name, value));
                    }
                }
            }

            //TODO is this neccesary? FragmentView will prettyprint nonetheless...
            XmlWriterSettings settings = new XmlWriterSettings
                                             {
                                                 CheckCharacters = true,
                                                 OmitXmlDeclaration = true,
                                                 Indent = true,
                                                 CloseOutput = true,
                                             };

            StringWriter sw;
            using (sw = new StringWriter())
            using (XmlWriter w = XmlWriter.Create(sw, settings))
            {
                o.XmlNode.WriteTo(w);
            }
            _editor.SetFragmentText(sw.ToString());
        }

        //private void initLstElements()
        //{
        //    lstElements.Items.Clear();
        //    lstTypes.Items.Clear();
        //    var allNodes = Schema.GetAllNodes();
        //    List<object> elements = new List<object>();
        //    List<object> types = new List<object>();
        //    foreach (IXsdNode node in allNodes)
        //    {
        //        if (node is XsdElement)
        //        {
        //            if (!elements.Contains(node))
        //            {
        //                elements.Add(node);
        //            }
        //        }
        //    }
        //    foreach (IXsdNode node in allNodes)
        //    {
        //        if (node is IXsdIsType)
        //        {
        //            if (!types.Contains(node))
        //            {
        //                types.Add(node);    
        //            }
        //        }
        //    }

        //    elements.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
        //    types.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

        //    foreach (object element in elements)
        //    {
        //        lstElements.Items.Add(element);
        //    }
        //    foreach (object type in types)
        //    {
        //        lstTypes.Items.Add(type);
        //    }
        //}

        public void Refresh()
        {
            XsdVisualizerData data = _editor.Data.XsdVisualizerData;
            PaintOptions po = new PaintOptions
            {
                ExpandReferences = data.ExpandRefs,
                HideTypes = data.HideTypes,
                HideImportIncludes = data.HideIncludes,
                ExpandTypes = data.ExpandTypes,
                ExpandIncludes = data.ExpandIncludes,
                Visualizer =  this,
                DontPaint = _dontPaint,
            };

            if (Root != null)
            {
                UIElement pnl = Root.GetPaintComponent(po, _fontSize);
                pnlPaintArea.Children.Clear();
                pnlPaintArea.Children.Add(pnl);
            }
            else
            {
                MessageBox.Show(Application.Current.MainWindow, "No root or root not yet loaded", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public HashSet<IXsdNode> IgnoreNodes
        {
            get { return _dontPaint; }
        }
     
        public void ZoomIn()
        {
            _fontSize++;
            Refresh();
        }

        public void ZoomOut()
        {
            _fontSize--;
            if (_fontSize < 4) _fontSize = 4;
            Refresh();
        }

        public void ShowRoot()
        {
            Root = Schema.GetVirtualRoot();
        }

        public void HistoryBack()
        {
            if (_history.Count > 1)
            {
                _history.Pop();
                HistoryItem item = _history.Pop();
                Schema = item.parser;
                IXsdNode node = item.node;
                Root = node;
            }
            else
            {
                MessageBox.Show(Application.Current.MainWindow, "No item in history", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void Export() 
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "Png Files (*.png)|*.png";
            sfd.Title = "Select an file to save";
            if (sfd.ShowDialog(Application.Current.MainWindow) == true)
            {
                int height = (int)pnlPaintArea.ActualHeight;
                int width = (int)pnlPaintArea.ActualWidth;

                RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(pnlPaintArea);

                string file = sfd.FileName;

                string extension = Path.GetExtension(file);
                if (extension != null)
                {
                    extension = extension.ToLower();
                }

                BitmapEncoder encoder;
                if (extension == ".gif")
                    encoder = new GifBitmapEncoder();
                else if (extension == ".jpg")
                    encoder = new JpegBitmapEncoder();
                else
                    encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using (Stream stm = File.Create(file))
                {
                    encoder.Save(stm);
                }
            }
        }

        //private void lst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    IXsdNode selItem = (IXsdNode)((ListBox)sender).SelectedItem;
        //    if (selItem != null)
        //    {
        //        Root = selItem;
        //    }
        //}

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_currentFile != null && Schema == null)
            {
                new Action(() => 
                    Dispatcher.Invoke(new Action(() =>
                                        {
                                            try
                                            {
                                                Mouse.OverrideCursor = Cursors.Wait;
                                                Schema = new SchemaParser(_currentFile, true);
                                            }
                                            catch
                                            {
                                                //ignore
                                            }
                                            finally
                                            {
                                                Mouse.OverrideCursor = null;
                                            }
                                        }))).BeginInvoke(null, null);
            }
        }

        public void CopyToClipboard()
        {
            int height = (int)pnlPaintArea.ActualHeight;
            int width = (int)pnlPaintArea.ActualWidth;

            RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(pnlPaintArea);

            Clipboard.SetImage(bmp);
        }

        public void UnhideAll()
        {
            _dontPaint.Clear();
            Refresh();
        }


    }
}
