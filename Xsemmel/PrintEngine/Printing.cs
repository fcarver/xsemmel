using System;
using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PrintEngine;

namespace XSemmel.PrintEngine
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Source: http://community.sharpdevelop.net/forums/t/12012.aspx</remarks>
    public static class Printing
    {

        /// <summary>
        /// Creates a DocumentPaginatorWrapper from TextEditor text to print.
        /// </summary>
        public static DocumentPaginatorWrapper CreateDocumentPaginatorToPrint(TextEditor textEditor, PageSettings pageSettings, PrintTicket printTicket, string title)
        {
            // this baby adds headers and footers
            IDocumentPaginatorSource dps = CreateFlowDocumentToPrint(textEditor, pageSettings, printTicket);
            DocumentPaginatorWrapper dpw = new DocumentPaginatorWrapper(dps.DocumentPaginator, pageSettings, printTicket, textEditor.FontFamily);
            dpw.Title = title;

            return dpw;
        }

        /// <summary>
        /// Creates a FlowDocument from TextEditor text to print.
        /// </summary>
        static FlowDocument CreateFlowDocumentToPrint(TextEditor textEditor, PageSettings pageSettings, PrintTicket printTicket)
        {
            // this baby has all settings to be printed or previewed in the PrintEngine.PrintPreviewDialog
            FlowDocument doc = CreateFlowDocumentForEditor(textEditor);

            doc.ColumnWidth = pageSettings.PrintableArea.Width;
            doc.PageHeight = (pageSettings.Landscape ? (int)printTicket.PageMediaSize.Width : (int)printTicket.PageMediaSize.Height);
            doc.PageWidth = (pageSettings.Landscape ? (int)printTicket.PageMediaSize.Height : (int)printTicket.PageMediaSize.Width);
            doc.PagePadding = ConvertPageMarginsToThickness(pageSettings.Margins);
            doc.FontFamily = textEditor.FontFamily;
            doc.FontSize = textEditor.FontSize;

           return doc;
        }


        /// <summary>
        /// Creates a FlowDocument from TextEditor text.
        /// </summary>
        static FlowDocument CreateFlowDocumentForEditor(TextEditor editor)
        {
            // ref.:  http://community.sharpdevelop.net/forums/t/12012.aspx

            IHighlighter highlighter = editor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;
            FlowDocument doc = new FlowDocument(ConvertTextDocumentToBlock(editor.Document, highlighter));

            return doc;
        }


        /// <summary>
        /// Converts a TextDocument to Block.
        /// </summary>
        static Block ConvertTextDocumentToBlock(TextDocument document, IHighlighter highlighter)
        {
            // ref.:  http://community.sharpdevelop.net/forums/t/12012.aspx

            if (document == null)
                throw new ArgumentNullException("document");

            Paragraph p = new Paragraph();

            foreach (DocumentLine line in document.Lines)
            {
                int lineNumber = line.LineNumber;
                HighlightedInlineBuilder inlineBuilder = new HighlightedInlineBuilder(document.GetText(line));
                if (highlighter != null)
                {
                    HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
                    int lineStartOffset = line.Offset;
                    foreach (HighlightedSection section in highlightedLine.Sections)
                        inlineBuilder.SetHighlighting(section.Offset - lineStartOffset, section.Length, section.Color);
                }

                p.Inlines.AddRange(inlineBuilder.CreateRuns());
                p.Inlines.Add(new LineBreak());
            }

            return p;
        }

        /// <summary>
        /// Converts specified Margins (hundredths of an inch) to Thickness (px).
        /// </summary>
        static Thickness ConvertPageMarginsToThickness(Margins margins)
        {
            Thickness thickness = new Thickness();
            thickness.Left = ConvertToPx(margins.Left);
            thickness.Top = ConvertToPx(margins.Top);
            thickness.Right = ConvertToPx(margins.Right);
            thickness.Bottom = ConvertToPx(margins.Bottom);
            return thickness;
        }

        /// <summary>
        /// Converts specified inch (hundredths of an inch) to pixels (px).
        /// </summary>
        static double ConvertToPx(double inch)
       {
            return inch * 0.96;
        }

    }
}
