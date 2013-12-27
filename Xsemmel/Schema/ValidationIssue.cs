namespace XSemmel.Schema
{
    public class ValidationIssue
    {

        public enum Type
        {
            Error,
            Warning,
            Information
        }

        private readonly object _sourceUri;

        public ValidationIssue(Type type, int line, int col, string message)
        {
            Message = message;
            Line = line;
            Column = col;
            IssueType = type;
        }

        public ValidationIssue(Type type, int line, int col, object sourceUri, string message)
        {
            Message = message;
            Line = line;
            Column = col;
            _sourceUri = sourceUri;
            IssueType = type;
        }

        public override string ToString()
        {
            string position = "";
            if (Line != 0 && Column != 0)
            {
                if (_sourceUri != null)
                {
                    position = string.Format("({0}:{1}, {2}): ", Line, Column, _sourceUri);
                }
                else
                {
                    position = string.Format("({0}:{1}): ", Line, Column);
                }
            }

            return string.Format("{0}{2}, {1}", position, Message, IssueType);
        }


        public int Line
        {
            get;
            private set;
        }

        public int Column
        {
            get;
            private set;
        }

        public Type IssueType
        {
            get; 
            private set;
        }

        public string Message
        {
            get; 
            private set;
        }

    }
}
