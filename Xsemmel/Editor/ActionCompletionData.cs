using System;
using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Media;

namespace XSemmel.Editor
{
	/// <summary>
	/// Implements AvalonEdit ICompletionData interface to provide the entries in the completion drop down.
	/// </summary>
	public class ActionCompletionData : ICompletionData
	{

	    private readonly CompleteDelegate _action;

        public delegate void CompleteDelegate(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs);

        public ActionCompletionData(string textToShow, string description, CompleteDelegate action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
            Content = new TextBlock(new Italic(new Run(textToShow)));
            Description = description;
        }

		public ImageSource Image 
        {
			get { return null; }
		}
		
		public string Text 
        { 
            get { return ""; } 
            private set {}
        }
		
		// Use this property if you want to show a fancy UIElement in the drop down list.
        public object Content 
        { 
            get;
            private set;
        }

	    public object Description
        { 
            get;
            private set;
        }
		
		public double Priority { get { return 0; } }
		
		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
            _action(textArea, completionSegment, insertionRequestEventArgs);
		}
	}
}
