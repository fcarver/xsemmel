// ref gac System;
// ref gac System.Xml;

using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace XSemmel
{

    /// <summary>
    /// This class devires from XsltContext and implements the
    /// methods necessary to resolve functions and variables
    /// in a XSLT context.
    /// Source: http://www.codeproject.com/KB/cs/CustomFunctions.aspx
    /// See also http://msdn.microsoft.com/en-us/library/ms950806.aspx
    /// </summary>
    public sealed class ExtendedXPathFunctions: XsltContext
    {
        // Private collection for variables
        private XsltArgumentList m_Args;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExtendedXPathFunctions()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nametable"></param>
        public ExtendedXPathFunctions(NameTable nametable)
            : base(nametable)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nametable"></param>
        /// <param name="Args"></param>
        public ExtendedXPathFunctions(NameTable nametable, XsltArgumentList Args)
            : base(nametable)
        {
            ArgumentList = Args;
        }

        #region XsltContext Implementation

        /// <summary>
        /// Override for XsltContext method used to resolve methods
        /// </summary>
        /// <param name="prefix">Namespace prefix for function</param>
        /// <param name="name">Name of function</param>
        /// <param name="ArgTypes">Array of XPathResultType</param>
        /// <returns>Implementation of IXsltContextFunction</returns>
        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
        {
            IXsltContextFunction func = null;

            switch(name)
            {
                case "compare":
                    func = new CompareFunction();
                    break;
                case "regex":
                    func = new RegexFunction();
                    break;
                default:
                    break;
            }
            return func;
        }

        /// <summary>
        /// Override for XsltContext method used to resolve variables
        /// </summary>
        /// <param name="prefix">Namespace prefix for variable</param>
        /// <param name="name">Name of variable</param>
        /// <returns>CustomVariable</returns>
        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            return new CustomVariable(name);
        }

        /// <summary>
        /// Not used in this example
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="nextbaseUri"></param>
        /// <returns></returns>
        public override int CompareDocument(string baseUri, string nextbaseUri)
        {
            return 0;
        }

        /// <summary>
        /// Not used in this example
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool PreserveWhitespace(XPathNavigator node)
        {
            return true;
        }

        /// <summary>
        /// Not used in this example
        /// </summary>
        public override bool Whitespace
        {
            get { return true; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Argument List
        /// </summary>
        public XsltArgumentList ArgumentList
        {
            get { return m_Args; }
            set { m_Args = value; }
        }

        #endregion
    }

    /// <summary>
    /// Implementation of IXsltContextFunction to be used to compare strings in a case insensitve
    /// manner.
    /// </summary>
    internal sealed class CompareFunction : IXsltContextFunction
    {

        /// <summary>
        /// Perform custom processing for this XsltCustomFunction
        /// </summary>
        /// <param name="xsltContext">XsltContext this function is operating under</param>
        /// <param name="args">Parameters from function</param>
        /// <param name="docContext">XPathNavigator for which function is being applied to</param>
        /// <returns>Returns true if match is found, otherwise false</returns>
        public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
        {
            if(args.Length != 2)
                throw new ApplicationException("Two arguments must be provided to compare function.");

            string arg1 = args[0].ToString();
            string arg2 = args[1].ToString();

            return String.Compare(arg1, arg2, true);
        }

        /// <summary>
        /// Not used
        /// </summary>
        public int Maxargs
        {
            get { return 2; }
        }

        /// <summary>
        /// Not used
        /// </summary>
        public int Minargs
        {
            get { return 2; }
        }

        /// <summary>
        /// Called for each parameter in the function
        /// </summary>
        public XPathResultType ReturnType
        {
            get { return XPathResultType.Number; }
        }

        /// <summary>
        /// Not used
        /// </summary>
        public XPathResultType[] ArgTypes
        {
            get { return new[] { XPathResultType.String, XPathResultType.String }; }
        }

    }

    /// <summary>
    /// Implementation of IXsltContextFunction to be used to match strings to a regular expression.
    /// </summary>
    internal sealed class RegexFunction : IXsltContextFunction
    {

        /// <summary>
        /// Perform custom processing for this XsltCustomFunction
        /// </summary>
        /// <param name="xsltContext">XsltContext this function is operating under</param>
        /// <param name="args">Parameters from function</param>
        /// <param name="docContext">XPathNavigator for which function is being applied to</param>
        /// <returns>Returns true if match is found, otherwise false</returns>
        public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
        {
            if (args.Length != 2)
                throw new ApplicationException("Two arguments must be provided to compare function.");

            string arg1 = args[0].ToString();
            string arg2 = args[1].ToString();

            var regex = new Regex(arg1);
            return regex.IsMatch(arg2);
        }

        /// <summary>
        /// Not used
        /// </summary>
        public int Maxargs
        {
            get { return 2; }
        }

        /// <summary>
        /// Not used
        /// </summary>
        public int Minargs
        {
            get { return 2; }
        }

        /// <summary>
        /// Called for each parameter in the function
        /// </summary>
        public XPathResultType ReturnType
        {
            get { return XPathResultType.Number; }
        }

        /// <summary>
        /// Not used
        /// </summary>
        public XPathResultType[] ArgTypes
        {
            get { return new[] { XPathResultType.String, XPathResultType.String }; }
        }

    }

    
    /// <summary>
    /// Implementation of IXsltContextVariable 
    /// </summary>
    internal sealed class CustomVariable : IXsltContextVariable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Variable name</param>
        internal CustomVariable(string name)
        {
            Name = name;
        }

        #region IXsltContextVariable Members

        /// <summary>
        /// Gets the value of the variable specified
        /// </summary>
        /// <param name="xsltContext">Context in which this variable is used</param>
        /// <returns>Value of the variable</returns>
        public object Evaluate(XsltContext xsltContext)
        {
            XsltArgumentList args = ((ExtendedXPathFunctions)xsltContext).ArgumentList;
            return args.GetParam(Name, "");
        }

        /// <summary>
        /// Not used
        /// </summary>
        public bool IsLocal
        {
            get { return false; }
        }

        /// <summary>
        /// Not used
        /// </summary>
        public bool IsParam
        {
            get { return false; }
        }

        /// <summary>
        /// Not used
        /// </summary>
        public XPathResultType VariableType
        {
            get { return XPathResultType.Any; }
        }

        #endregion


        public string Name { get; set; }

    }

}
