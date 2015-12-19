using System;
using System.Text;
using System.Xml.Schema;

namespace XSemmel.Schema
{

    public partial class SchemaInfoView
    {

        public SchemaInfoView()
        {
            InitializeComponent();
        }

        public void SetSchemaInfo(IXmlSchemaInfo schemaInfo)
        {
            if (schemaInfo == null)
            {
                _edtText.Text = "";
                return;
            }

            StringBuilder sb = new StringBuilder();

            try
            {
                if (schemaInfo.SchemaElement != null && schemaInfo.SchemaElement.Annotation != null)
                {
                    var annElement = (XmlSchemaDocumentation) schemaInfo.SchemaElement.Annotation.Items[0];
                    append(sb, annElement);
                }
                if (schemaInfo.SchemaType != null && schemaInfo.SchemaType.Annotation != null)
                {
                    var annType = schemaInfo.SchemaType.Annotation.Items[0] as XmlSchemaDocumentation;
                    append(sb, annType);
                }
                if (schemaInfo.SchemaAttribute != null && schemaInfo.SchemaAttribute.Annotation != null)
                {
                    var annAttribute = schemaInfo.SchemaAttribute.Annotation.Items[0] as XmlSchemaDocumentation;
                    append(sb, annAttribute);
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("An error occurred: " + ex.Message);
            }

            if (sb.Length == 0)
            {
                sb.AppendLine("(No annotation found)");
            }

            _edtText.Text = sb.ToString();
        }

        private void append(StringBuilder sb, XmlSchemaDocumentation doc)
        {
            sb.AppendLine(string.Format("(Source: {2} at Ln {0} Col {1})", doc.LineNumber, doc.LinePosition, doc.SourceUri));
            sb.AppendLine();
            sb.Append("     ");
            sb.AppendLine(doc.Markup[0].OuterXml.Trim());
        }

    }
}
