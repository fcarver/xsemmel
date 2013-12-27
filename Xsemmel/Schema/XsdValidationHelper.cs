using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using XSemmel.Configuration;
using XSemmel.Helpers;

namespace XSemmel.Schema
{
    public class XsdValidationHelper
    {
        private static readonly XsdValidationHelper _instance = new XsdValidationHelper();
        private XsdValidationResult _result;

        private void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            var ex = e.Exception as XmlSchemaValidationException;
            if (ex != null)
            {
                if (string.IsNullOrEmpty(ex.SourceUri))
                {
                    var vi = new ValidationIssue(ValidationIssue.Type.Error, ex.LineNumber, ex.LinePosition, e.Message);
                    _result.Results.Add(vi);
                }
                else
                {
                    var vi = new ValidationIssue(ValidationIssue.Type.Error,  ex.LineNumber, ex.LinePosition, ex.SourceUri, e.Message);
                    _result.Results.Add(vi);
                }
            }
            else
            {
                var vi = new ValidationIssue(ValidationIssue.Type.Error, 0, 0, e.Message);
                _result.Results.Add(vi);
            }
            _result.State = ValidationState.ValidationError;
        }

        public XsdValidationResult ValidateInstance(string xsdFilePath, string instanceDoc)
        {
            if (string.IsNullOrEmpty(xsdFilePath))
            {
                throw new ArgumentNullException("xsdFilePath", "An Xsd File Path must be given");
            }
            if (instanceDoc == null)
            {
                throw new ArgumentNullException("instanceDoc", "A valid instance XmlDocument must be supplied");
            }
            if (!FileHelper.FileExists(xsdFilePath))
            {
                throw new ArgumentException(string.Format("The Xsd file '{0}' dooes not exist. Please specify a valid file.", xsdFilePath));
            }
            
            FileInfo info = new FileInfo(xsdFilePath);
            Environment.CurrentDirectory = info.DirectoryName;
            StreamReader reader = new StreamReader(xsdFilePath);
            XmlTextReader reader2 = new XmlTextReader(reader.BaseStream);
            XsdValidationResult result = ValidateInstance(reader2, instanceDoc);
            Environment.CurrentDirectory = Environment.CurrentDirectory;
            return result;
        }

        public XsdValidationResult ValidateInstance(XmlTextReader xsdReader, string instanceDoc)
        {
            _result = new XsdValidationResult();
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                //settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation;
                try
                {
                    settings.Schemas.Add(null, xsdReader);
                }
                catch (Exception ex)
                {
                    throw new Exception("Invalid schema file: " + ex.Message, ex);
                }
                settings.ValidationEventHandler += settings_ValidationEventHandler;
                List<string> list = new List<string>();
                foreach (XmlSchema schema in settings.Schemas.Schemas())
                {
                    if (!string.IsNullOrEmpty(schema.TargetNamespace))
                    {
                        list.Add(schema.TargetNamespace);
                    }
                }

                XmlReader reader2 = XmlReader.Create(instanceDoc.ToStream(), settings);
                while (reader2.Read())
                {
                }
                if (_result.State == ValidationState.Success)
                {
                    XmlDocument xd = instanceDoc.ToXmlDocument();
                    if (!list.Contains(xd.DocumentElement.NamespaceURI))
                    {
                        StringBuilder builder = new StringBuilder();

                        builder.AppendLine("The namespace of the current document does not match any of the target namespaces for the loaded schemas.");
                        builder.AppendLine("Have you specified the correct schema for the current document?");
                        builder.AppendLine("Document Namespace:\t");
                        builder.AppendLine(xd.DocumentElement.NamespaceURI);
                        builder.AppendLine("Schema Target Namespace(s):");
                        
                        if (list.Count == 0)
                        {
                            builder.Append("\t(None)");
                        }
                        else
                        {
                            foreach (string t in list)
                            {
                                builder.Append("\t");
                                builder.AppendLine(t);
                            }
                        }
                        var vi = new ValidationIssue(ValidationIssue.Type.Information, 0, 0, builder.ToString());
                        _result.Results.Add(vi);

                        _result.State = ValidationState.Warning;
                    }
                }
            }
            catch (XmlSchemaException exception)
            {
                string text = string.Format("Invalid xsd file ({0}:{1}): {2}", exception.LineNumber, exception.LinePosition, exception.Message);
                var vi = new ValidationIssue(ValidationIssue.Type.Error, 0,0, text);
                _result.Results.Add(vi);
                _result.State = ValidationState.OtherError;
            }
            catch (XmlException exception)
            {
                var vi = new ValidationIssue(ValidationIssue.Type.Error, exception.LineNumber, exception.LinePosition, exception.Message);
                _result.Results.Add(vi);
                _result.State = ValidationState.OtherError;
            }
            catch (Exception exception)
            {
                var vi = new ValidationIssue(ValidationIssue.Type.Error, 0, 0, exception.Message);
                _result.Results.Add(vi);
                _result.State = ValidationState.OtherError;
            }
            finally
            {
                if (xsdReader != null)
                {
                    xsdReader.Close();
                }
            }
            return _result;
        }

        public static XsdValidationHelper Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Gets the filename of the declared XSD
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="currentDirectory"></param>
        /// <returns></returns>
        public static string GetXsdFile(XmlDocument xmldoc, string currentDirectory)
        {
            var doc = xmldoc.DocumentElement;
            if (doc != null)
            {
                foreach (XmlAttribute attr in doc.Attributes)
                {
                    string value = attr.Value;
                    if (value.ToLower().EndsWith(".xsd"))
                    {
                        List<ConfigObj.XsdMapItem> items = XSConfiguration.Instance.Config.XsdMapping;
                        foreach (ConfigObj.XsdMapItem xsdMapItem in items)
                        {
                            if (string.Compare(xsdMapItem.Name, value, true) == 0)
                            {
                                value = xsdMapItem.Mapping;
                                break;
                            }
                        }

                        value = Path.Combine(currentDirectory, value);
                        return value;
                    }
                }
            }
            return null;
        }

    }

 

}
