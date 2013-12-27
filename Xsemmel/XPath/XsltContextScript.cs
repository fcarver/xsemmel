using System;
using System.IO;
using System.Reflection;
using System.Xml.Xsl;

namespace XSemmel.XPath
{

    public class XsltContextScript
    {

        /// <summary>
        /// ScriptEngine.FromSCriptFile erzeugt bei jedem Aufruf ein neues Assembly und lädt dieses.
        /// Da Assemblies nicht mehr entladen werden können, müllt der Speicher zu.
        /// 
        /// Diese Variablen sorgen dafür, dass - solange sich die Scriptdatei nicht ändert - immer die bereits 
        /// initialisierte Instanz zurückgegeben wird.
        /// </summary>
        private DateTime _alreadyLoadedWasModifiedAt;
        private XsltContext _alreadyLoaded;


        public static readonly XsltContextScript Instance = new XsltContextScript();

        private XsltContextScript()
        {
        }

        private string getFileName()
        {
            string dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string file = Path.Combine(dir, "XPathFunctions.cs");
            return file;
        }


        private XsltContext getNewContext()
        {
            string file = getFileName();
            if (File.Exists(file))
            {
                return new ScriptEngine().FromScriptFile<XsltContext>(file);
            }
            return null;
        }


        public XsltContext Get()
        {
            string file = getFileName();
            if (!File.Exists(file)) return null;

            if (_alreadyLoaded == null)
            {
                _alreadyLoadedWasModifiedAt = File.GetLastWriteTimeUtc(file);
                _alreadyLoaded = getNewContext();
                return _alreadyLoaded;
            }
            else
            {
                DateTime lastModifiedAt = File.GetLastWriteTimeUtc(file);
                if (lastModifiedAt != _alreadyLoadedWasModifiedAt)
                {
                    _alreadyLoaded = null;
                    return Get();
                }
                else
                {
                    return _alreadyLoaded;
                }
            }
        }

    }
}
