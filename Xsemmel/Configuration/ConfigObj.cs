using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using PropertyTools.DataAnnotations;

namespace XSemmel.Configuration
{

   

    public class ConfigObj
    {

        public enum OpenType
        {
            Nothing,
            LastOpenedFile,
            XmlInClipboard
        }

        public class XsdMapItem : INotifyPropertyChanged
        {
            public string Name { get; set; }
            public string Mapping { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;
        }

//        private string _fileEncoding = "UTF-8";

        private ushort _fontSize = 13;
        private bool _showSplashScreen = false;
        private bool _watchCurrentFileForChanges = true;
        private bool _alwaysPrettyprintFragments = true;
        private bool _enableCodeCompletion = true;
        private const int MAX_RECENTLY_USED_FILES = 5;
        private readonly List<string> _recentlyUsedFiles = new List<string>(); //cannot use LinkedList due to serialization
        private List<XsdMapItem> _xsdMapping = new List<XsdMapItem>();


        [Category("General|Application")]
        public bool ShowSplashScreen
        {
            get { return _showSplashScreen; }
            set { _showSplashScreen = value; }
        }

        [Category("Schema|XSD mapping")]
        [Column(0, "Name", "Replace", null, "2*", 'L')]
        [Column(1, "Mapping", "by", null, "1*", 'L')]
        [HeaderPlacement(HeaderPlacement.Collapsed)]
        public List<XsdMapItem> XsdMapping
        {
            get { return _xsdMapping; }
            set { _xsdMapping = value; }
        }


        [Category("Schema|XSD mapping")]
        [Comment]
        public string Comment
        {
            get { return "All xsd's with the specified name will be replaced by the path of the mapping"; }
            set { string dummy = value;  }
        }

        [Category("General|Application")]
        public bool WatchCurrentFileForChanges
        {
            get { return _watchCurrentFileForChanges; }
            set { _watchCurrentFileForChanges = value; }
        }

        [Category("General|Application")]
        public bool AlwaysPrettyprintFragments
        {
            get { return _alwaysPrettyprintFragments; }
            set { _alwaysPrettyprintFragments = value; }
        }

        [Category("General|Editor")]
        public ushort FontSize
        {
            get 
            {
                if (_fontSize < 3) return 3;
                return _fontSize;
            }
            set 
            {
                _fontSize = value; 
            }
        }

        [Category("General|Editor")]
        public bool EnableCodeCompletion
        {
            get { return _enableCodeCompletion; }
            set { _enableCodeCompletion = value; }
        }

//        public string FileEncoding
//        {
//            get { return _fileEncoding; }
//            set { _fileEncoding = value; }
//        }

        [Category("General|Editor")]
        public Encoding Encoding
        {
            get { return new UTF8Encoding(false); }
        }

        [Browsable(false)]
        public string LastOpenedFile
        {
            get; set;
        }

        [Category("General|Application")]
        public OpenType OpenAtStartup { get; set; }

        [Browsable(false)]
        public List<string> RecentlyUsedFiles  //cannot use IList due to serialization
        {
            get { return _recentlyUsedFiles; }
        }

        [Browsable(false)]
        public string TcpListenerPort
        {
            get; set;
        }

        [Browsable(false)]
        public string TcpListenerEncoding
        {
            get; set;
        }

        [Browsable(false)]
        public string TcpListenerNic
        {
            get; set;
        }


        public void AddRecentlyUsedFile(string file)
        {
            if (!_recentlyUsedFiles.Contains(file))
            {
                _recentlyUsedFiles.Add(file);
            }
            while (_recentlyUsedFiles.Count > MAX_RECENTLY_USED_FILES)
            {
                _recentlyUsedFiles.RemoveAt(0);
            }
        }


    }
}
