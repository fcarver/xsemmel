using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace XSemmel.Configuration
{
    class XSConfiguration
    {

        public static readonly XSConfiguration Instance = new XSConfiguration();

        private readonly string _configFile;
        private readonly ConfigObj _config;

        private XSConfiguration()
        {
            _configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "xsemmel.config");

            _config = new ConfigObj();

            if (File.Exists(_configFile))
            {
                try
                {
                    using (Stream s = File.OpenRead(_configFile))
                    {
                        _config = deserialize(s);
                    }
                }
                catch (Exception e)
                {
                    Debug.Fail(e.Message);
                    File.Delete(_configFile);
                }
            }
        }

        public void Save()
        {
            using (Stream s = File.Create(_configFile))
            {
                serialize(s);
            }
        }

        public ConfigObj Config
        {
            get { return _config; }
        }

        private void serialize(Stream outStream)
        {
            lock (this)
            {
                XmlSerializer xs = new XmlSerializer(typeof (ConfigObj));
                xs.Serialize(outStream, _config);
            }
        }

        private ConfigObj deserialize(Stream inStream)
        {
            lock (this)
            {
                XmlSerializer xs = new XmlSerializer(typeof (ConfigObj));
                return (ConfigObj) xs.Deserialize(inStream);
            }
        }


    }
}
