using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using XSemmel.Editor;
using XSemmel.Helpers;
using XSemmel.PrettyPrinter;

namespace XSemmel.TcpListener
{
    /// <summary>
    /// Interaction logic for TcpListeningWindow.xaml
    /// </summary>
    public partial class TcpListeningWindow
    {

        private System.Net.Sockets.TcpListener _listener;
        private readonly object _listenerLock = new object();
        private TcpClient _client;
        private readonly ConfigureListener.Config _configuration;
        private readonly EditorFrame _editor;


        public TcpListeningWindow(ConfigureListener.Config configuration, EditorFrame editorFrame)
        {
            InitializeComponent();
            Closed += OnClosed;

            _editor = editorFrame;
            _configuration = configuration;

            _edtResult.Options.ShowBoxForControlCharacters = true;
            _edtResult.Options.ShowEndOfLine = true;
            _edtResult.Options.ShowSpaces = true;
            _edtResult.Options.ShowTabs = true;
        }

        private void stopListener()
        {
            lock (_listenerLock)
            {
                if (_listener != null)
                {
                    _listener.Stop();
                    _listener.Server.Close();
                    if (_client != null)
                    {
                        _client.Close();
                    }

                    _listener = null;
                }
            }
        }
         
        private void OnClosed(object sender, EventArgs eventArgs)
        {
            stopListener();
        }

        private void setStatus(string message)
        {
            Dispatcher.Invoke(new Action(() => { _lblStatus.Text = message; }));
        }

        private void threadMethod(object threadStart)
        {
            ConfigureListener.Config config = (ConfigureListener.Config)threadStart;
            try 
            {
                lock (_listenerLock)
                {
                    _listener = new System.Net.Sockets.TcpListener(config.Nic, config.Port);
                    _listener.Start();
                }
            }
            catch (Exception se)
            {
                setStatus("Error: " + se.Message);
                stopListener();
            }

            byte[] bytes = new byte[256];
            while (true)
            {
                try
                {
                    if (_listener == null)
                    {
                        return;
                    }

                    if (!_listener.Pending())
                    {
                        setStatus("Waiting for a connection... on " + _listener.LocalEndpoint);
                        Thread.Sleep(500);
                        continue;
                    }

                    using (_client = _listener.AcceptTcpClient())
                    using (NetworkStream stream = _client.GetStream())
                    {
                        setStatus(string.Format("Connected, {1} -> {0}", _client.Client.LocalEndPoint,
                                                _client.Client.RemoteEndPoint));

                        int i;
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            string data = config.Encoding.GetString(bytes, 0, i);

                            Dispatcher.Invoke(new Action(() => showData(data)));

                            // data = data.ToUpper();
                            // byte[] msg = Encoding.ASCII.GetBytes(data);
                            // stream.Write(msg, 0, msg.Length);
                            // Console.WriteLine("Sent: {0}", data);
                        }
                    }
                }
                catch (Exception se)
                {
                    setStatus("Error: " + se.Message);
                }
            }

//            finally
//            {
//                stopListener();
//            }
        }

        private void showData(string message)
        {
            _edtResult.Text = _edtResult.Text + message;
            _lblLastReceived.Text = string.Format("Last received: {0:d} {0:T}", DateTime.Now);
        }

        public void Start()
        {
            stopListener();

            Thread thread = new Thread(threadMethod) { IsBackground = true };
            thread.Start(_configuration);            
        }

        private void btnClear_OnClick(object sender, RoutedEventArgs e)
        {
            _edtResult.Clear();
        }

        private void mnuCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(getStringInContext());
        }

        private string getStringInContext()
        {
            if (_edtResult.SelectionLength == 0)
            {
                return _edtResult.Text;
            }
            else
            {
                return _edtResult.SelectedText;
            }
        }

        private void mnuOpenInNewXSemmel_Click(object sender, RoutedEventArgs e)
        {
            string pipename = NamedPipeHelper.StartNewListeningXSemmel();
            new Thread(text =>
            {
                try
                {
                    NamedPipeHelper.Write(pipename, text.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }) { IsBackground = true }.Start(getStringInContext());
        }

        private void mnuAppendToEditor_Click(object sender, RoutedEventArgs e)
        {
            _editor.XmlEditor.AppendText(getStringInContext());
        }

           /// <summary>
        /// see string.Trim
        /// </summary>
        private static readonly char[] WhitespaceChars =
            { (char) 0x9, (char) 0xA, (char) 0xB, (char) 0xC, (char) 0xD, (char) 0x20,   (char) 0x85, 
              (char) 0xA0, (char)0x1680,
              (char) 0x2000, (char) 0x2001, (char) 0x2002, (char) 0x2003, (char) 0x2004, (char) 0x2005,
              (char) 0x2006, (char) 0x2007, (char) 0x2008, (char) 0x2009, (char) 0x200A, (char) 0x200B,
              (char) 0x2028, (char) 0x2029, 
              (char) 0x3000, (char) 0xFEFF,
              (char) 0x1C  //eigene Erweiterung
            };


        private void mnuAppendToEditorAndPrettyprint_Click(object sender, RoutedEventArgs e)
        {
            string text = getStringInContext().Trim(WhitespaceChars);
            try
            {
                text = PrettyPrint.Execute(text, true, false, true);
            }
            catch
            {
//                text = getStringInContext();
            }

            _editor.XmlEditor.AppendText(text);
        }

        private void chkShowSpecialCharacters_Click(object sender, RoutedEventArgs e)
        {
            bool value = ((CheckBox) sender).IsChecked == true;

            if (_edtResult != null)
            {
                _edtResult.Options.ShowBoxForControlCharacters = value;
                _edtResult.Options.ShowEndOfLine = value;
                _edtResult.Options.ShowSpaces = value;
                _edtResult.Options.ShowTabs = value;
            }
        }

        private void mnuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlgSaveFile = new SaveFileDialog();
            dlgSaveFile.Filter = "Text Files|*.txt|All Files|*.*";
            dlgSaveFile.Title = "Select a file to save";
            dlgSaveFile.FileName = string.Format("TCP_{0}.txt", DateTime.Now.Ticks);

            if (dlgSaveFile.ShowDialog(this) == true)
            {
                try
                {
                    _edtResult.Save(dlgSaveFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
    }
}
