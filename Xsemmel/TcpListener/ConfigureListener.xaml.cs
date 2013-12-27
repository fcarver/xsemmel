using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;
using XSemmel.Configuration;

namespace XSemmel.TcpListener
{
    /// <summary>
    /// Interaction logic for ConfigureListener.xaml
    /// </summary>
    public partial class ConfigureListener
    {

        private readonly string[] _encodings = new[]
            {
                "ASCII",
                "UTF8",
                "Unicode",
                "BigEndianUnicode",
                "UTF32",
                "UTF7",
            };

        public class Config
        {
            public int Port { get; set; }
            public Encoding Encoding { get; set; }
            public IPAddress Nic { get; set; }
        }

        private ConfigureListener()
        {
            InitializeComponent();
        }

        private void ConfigureListener_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                foreach (var encoding in _encodings)
                {
                    _cbxEncodings.Items.Add(encoding);
                }
                _cbxEncodings.SelectedIndex = 0;

                _cbxNetworkAdapter.Items.Add("Any");
                try
                {
                    foreach (var i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    {
                        foreach (var ua in i.GetIPProperties().UnicastAddresses)
                        {
                            _cbxNetworkAdapter.Items.Add(ua.Address);
                        }
                    }
                }
                catch
                {
                    //ignore
                }
                _cbxNetworkAdapter.SelectedIndex = 0;


                var storedConfig = XSConfiguration.Instance.Config;
                if (storedConfig.TcpListenerPort != null)
                {
                    _edtPort.Text = storedConfig.TcpListenerPort;
                }
                if (storedConfig.TcpListenerEncoding != null)
                {
                    int idxEncoding;
                    if (int.TryParse(storedConfig.TcpListenerEncoding, out idxEncoding))
                    {
                        _cbxEncodings.SelectedIndex = idxEncoding;
                    }
                }
                if (storedConfig.TcpListenerNic != null)
                {
                    string nic = storedConfig.TcpListenerNic;
                    for (int i = 0; i < _cbxEncodings.Items.Count; i++)
                    {
                        if (_cbxNetworkAdapter.Items[i].ToString() == nic)
                        {
                            _cbxNetworkAdapter.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            finally
            {
                Cursor = null;    
            }
        }

        public IPAddress GetSelectedNetworkAdapter()
        {
            object item = _cbxNetworkAdapter.SelectedItem;
            if (item is IPAddress)
            {
                return (IPAddress) item;
            }
            return IPAddress.Any;
        }

        public Encoding GetSelectedEncoding()
        {
            if (_cbxEncodings.SelectedItem == null)
            {
                return Encoding.ASCII;
            }

            string sel = _cbxEncodings.SelectedItem.ToString();
            switch (sel)
            {
                case "ASCII":
                    return Encoding.ASCII;
                case "BigEndianUnicode":
                    return Encoding.BigEndianUnicode;
                case "Unicode":
                    return Encoding.UTF32;
                case "UTF7":
                    return Encoding.UTF7;
                case "UTF8":
                    return Encoding.UTF8;
                default:
                    return Encoding.ASCII;
            }
        }

        public int GetPort()
        {
            int port;
            return int.TryParse(_edtPort.Text, out port) ? port : 4711;
        }


        public static bool ShowDialog(Window owner, out Config configuration)
        {
            var wmd = new ConfigureListener {Owner = owner};
            wmd.ShowDialog();

            configuration = new Config
                {
                    Port = wmd.GetPort(), 
                    Encoding = wmd.GetSelectedEncoding(),
                    Nic = wmd.GetSelectedNetworkAdapter(),
                };

            if (wmd.OkClicked)
            {
                XSConfiguration.Instance.Config.TcpListenerPort = configuration.Port.ToString();
                XSConfiguration.Instance.Config.TcpListenerEncoding = wmd._cbxEncodings.SelectedIndex.ToString();
                XSConfiguration.Instance.Config.TcpListenerNic = configuration.Nic.ToString();
            }

            return wmd.OkClicked;
        }

        private void _btnOk_OnClick(object sender, RoutedEventArgs e)
        {
            OkClicked = true;
            Close();
        }

        private bool OkClicked
        {
            get; set;
        }

        private void _btnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            OkClicked = false;
            Close();
        }
    }
}
