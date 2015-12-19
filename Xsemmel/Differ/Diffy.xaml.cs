using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using ICSharpCode.AvalonEdit;
using Microsoft.Win32;
using XSemmel.Configuration;
using XSemmel.Editor;
using XSemmel.Helpers;
using System;

namespace XSemmel.Differ
{

    public partial class Diffy
    {

        private readonly MarkBackgroundRenderer markerOne;
        private readonly MarkBackgroundRenderer markerTwo;
        private readonly EditorFrame _ef;

        public Diffy(EditorFrame ef)
        {
            _ef = ef;
            InitializeComponent();

            if (_ef != null)
            {
                chkCurrentDocument.IsChecked = true;
                edtXmlFile1.Text = _ef.XSDocument.Filename;
            }
            else
            {
                chkCurrentDocument.IsEnabled = false;
            }

            markerOne = new MarkBackgroundRenderer(edtOne);
            markerTwo = new MarkBackgroundRenderer(edtTwo);
            edtOne.TextArea.TextView.BackgroundRenderers.Add(markerOne);
            edtTwo.TextArea.TextView.BackgroundRenderers.Add(markerTwo);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.HideMinimizeButtons();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "Xml Files (*.xml)|*.xml";
            dlgOpenFile.Title = "Select an Xml File to write";
            if (FileHelper.FileExists(edtXmlFile1.Text))
            {
                dlgOpenFile.FileName = edtXmlFile1.Text;
            }
            if (dlgOpenFile.ShowDialog() == true)
            {
                edtXmlFile1.Text = dlgOpenFile.FileName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "Xml Files (*.xml)|*.xml";
            dlgOpenFile.Title = "Select an Xml File to write";
            if (FileHelper.FileExists(edtXmlFile2.Text))
            {
                dlgOpenFile.FileName = edtXmlFile2.Text;
            }
            if (dlgOpenFile.ShowDialog() == true)
            {
                edtXmlFile2.Text = dlgOpenFile.FileName;
            }
        }

        private static XmlNode createDOM(XmlReader xmlFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            doc.Normalize();

            return doc.DocumentElement;
        }

        private void btnDiff_Click(object sender, RoutedEventArgs e)
        {
            markerOne.ClearMarks();
            markerTwo.ClearMarks();
            if ((string.IsNullOrEmpty(edtXmlFile1.Text) && chkCurrentDocument.IsChecked==false) 
                || (string.IsNullOrEmpty(edtXmlFile2.Text) && chkClipboard.IsChecked==false))
            {
                MessageBox.Show(Application.Current.MainWindow, "Please choose two XML files to diff", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                edtResult.Text = "";
                return;
            }

            try
            {
                string firstXml;
                if (chkCurrentDocument.IsChecked == true)
                {
                    firstXml = _ef.XmlEditor.Text;
                }
                else
                {
                    firstXml = File.ReadAllText(edtXmlFile1.Text);
                }

                string secondXml;
                if (chkClipboard.IsChecked == true)
                {
                    secondXml = Clipboard.GetText();
                }
                else
                {
                    secondXml = File.ReadAllText(edtXmlFile2.Text);
                }

                edtResult.Text = diffWithDiffy(firstXml, secondXml);
                diffWithDiffplex(firstXml, secondXml);
            } 
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error while diffing: " + ex.Message, 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void diffWithDiffplex(string firstXml, string secondXml)
        {
            edtOne.Text = firstXml;
            edtTwo.Text = secondXml;

            var differ = new SideBySideDiffBuilder(new DiffPlex.Differ());
            SideBySideDiffModel diffRes = differ.BuildDiffModel(firstXml, secondXml);

            renderDiff(edtOne, diffRes.OldText.Lines, markerOne);
            renderDiff(edtTwo, diffRes.NewText.Lines, markerTwo);
        }

        private void renderDiff(TextEditor textEditor, IEnumerable<DiffPiece> diff, MarkBackgroundRenderer marker)
        {
            foreach (var line in diff)
            {
                if (line.Position == null)
                {
                    continue;
                }

                int offset = textEditor.Document.GetOffset((int)line.Position, 1);

                switch (line.Type)
                {
                    case ChangeType.Deleted:
                    {
                        marker.AddOffsetToMark(new MarkBackgroundRenderer.Mark
                        {
                            Offset = offset,
                            Length = line.Text.Length,
                            Brush = new SolidColorBrush(Color.FromArgb(255, 255, 200, 100))
                        });
                        break;
                    }
                    case ChangeType.Inserted:
                    {
                        marker.AddOffsetToMark(new MarkBackgroundRenderer.Mark
                        {
                            Offset = offset,
                            Length = line.Text.Length,
                            Brush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0))
                        });
                        break;
                    }
                    case ChangeType.Unchanged:
                    {
                        continue;
                    }
                    case ChangeType.Modified:
                    {
                        marker.AddOffsetToMark(new MarkBackgroundRenderer.Mark
                        {
                            Offset = offset,
                            Length = line.Text.Length,
                            Brush = new SolidColorBrush(Color.FromArgb(255, 220, 220, 128))
                        });
                        break;

                        //foreach (DiffPiece piece in line.SubPieces)
                        //{
                        //    Console.WriteLine();
                        //}
                    }
                    case ChangeType.Imaginary:
                    {
                        //deleted, shown as deleted in other pane
                        //could insert imaginary line, but that's work...
                        break;
                    }
                }
            }
        }

        private string diffWithDiffy(string firstXml, string secondXml)
        {
            try
            {
                XmlNode eines = createDOM(XmlReader.Create(firstXml.ToStream()));
                XmlNode anderes = createDOM(XmlReader.Create(secondXml.ToStream()));

                Knoten ersterKnoten = Knoten.createOf(eines);
                Knoten andererKnoten = Knoten.createOf(anderes);

                DiffEngine diff = new DiffEngine();
                Knoten res = diff.Diff(ersterKnoten, andererKnoten);

                if (res == null)
                {
                    return "XML files are equal";
                }
                else
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = XSConfiguration.Instance.Config.Encoding;

                    XmlDocument result = res.ToString().ToXmlDocument();

                    StringBuilder sb = new StringBuilder();
                    using (XmlWriter w = XmlWriter.Create(sb, settings))
                    {
                        result.WriteTo(w);
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            edtTwo.ScrollToVerticalOffset(edtOne.VerticalOffset);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            edtOne.ScrollToVerticalOffset(edtTwo.VerticalOffset);
        }
    }
}
