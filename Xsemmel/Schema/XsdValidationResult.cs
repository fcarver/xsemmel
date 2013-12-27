using System.Text;
using System.Collections.Generic;

namespace XSemmel.Schema
{
    public class XsdValidationResult
    {
        // Fields
        private readonly ICollection<ValidationIssue> _results = new List<ValidationIssue>();

        // Properties
        public ICollection<ValidationIssue> Results
        {
            get
            {
                return _results;
            }
        }

        public ValidationState State { get; set; }
    }

 

}
