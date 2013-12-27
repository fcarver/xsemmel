using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace XSemmel.Helpers
{
    public partial class BulkWindow
    {
        private readonly BackgroundWorker _bw = new BackgroundWorker();

        private struct WorkInfo
        {
            public string FolderReferences;
            public string PatternReferences;
            public bool ReferencesRecursive;

            public Func<BulkCommunicator, ICollection<string>, object, string> Work;
            public object UserObject;
        }

        private Func<BulkCommunicator, ICollection<string>, object, string> _work;

        private Func<object> _getUserObject;

        public BulkWindow()
        {
            InitializeComponent();

            _bw.WorkerReportsProgress = true;

            _bw.DoWork += search;
            _bw.ProgressChanged += reportProgress;
            _bw.RunWorkerCompleted += searchCompleted;
        }

        public UIElement InputControl
        {
            set { _input.Content = value; }
        }


        private void btnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _edtFolderReferences.Text = dialog.SelectedPath;
            }
        }

        public Func<BulkCommunicator, ICollection<string>, object, string> OnSearchClicked
        {
            set { _work = value; }
        }


        public Func<object> GetUserObject
        {
            set { _getUserObject = value; }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (_bw.IsBusy)
            {
                MessageBox.Show("Search is already running");
            }

            _btnSearch.IsEnabled = false;
            _edtResult.Text = "";
            _edtProgress.Text = "";

            object userObj = null;
            if (_getUserObject != null)
            {
                userObj = _getUserObject();
            }
            
            WorkInfo wi = new WorkInfo
                              {
                                  FolderReferences = _edtFolderReferences.Text,
                                  PatternReferences = _edtPatternReferences.Text,
                                  ReferencesRecursive = _chkReferencesRecursive.IsChecked == true,
                                  Work = _work,
                                  UserObject = userObj,
                              };

            _bw.RunWorkerAsync(wi);
            _tabCtrl.SelectedIndex = 1;
            _tabResult.IsEnabled = true;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        private void searchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Mouse.OverrideCursor = null;
            _btnSearch.IsEnabled = true;
            _edtResult.Text = e.Result.ToString();
            _edtResult.ScrollToEnd();
        }

        private void reportProgress(object sender, ProgressChangedEventArgs e)
        {
            _edtProgress.Text += e.UserState+"\n";
            _edtProgress.ScrollToEnd();
        }

        private void search(object sender, DoWorkEventArgs e)
        {
            WorkInfo wi = (WorkInfo)e.Argument;

            BulkCommunicator bc = new BulkCommunicator(_bw);

            bc.ReportProgress("Searching files...");

            ICollection<string> filesToSearchIn = getFilesToSearchIn(wi);

            var work = wi.Work;
            e.Result = work(bc, filesToSearchIn, wi.UserObject);
        }


        private ICollection<string> getFilesToSearchIn(WorkInfo wi)
        {
            try
            {
                SearchOption searchOption;
                if (wi.ReferencesRecursive)
                {
                    searchOption = SearchOption.AllDirectories;
                }
                else
                {
                    searchOption = SearchOption.TopDirectoryOnly;
                }

                string[] patterns = wi.PatternReferences.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                HashSet<string> files = new HashSet<string>();
                foreach (string pattern in patterns)
                {
                    if (Directory.Exists(wi.FolderReferences))
                    {
                        var res = Directory.GetFiles(wi.FolderReferences, pattern, searchOption);
                        foreach (string file in res)
                        {
                            files.Add(file);
                        }
                    }
                }
                return files;
            }
            catch
            {
                //Can't show messagebox, because we're in a background thread
                return new HashSet<string>();
            }
        }

        public class BulkCommunicator
        {
            private readonly BackgroundWorker _bw;

            internal BulkCommunicator(BackgroundWorker bw)
            {
                _bw = bw;
            }

            public void ReportProgress(string progress)
            {
                _bw.ReportProgress(0, progress);
            }
        }


        private void btnOpenInNewXsemmel_Click(object sender, RoutedEventArgs e)
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
            }) { IsBackground = true }.Start(_edtResult.Text);
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(_edtResult.Text);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlgSaveFile = new SaveFileDialog();
            dlgSaveFile.Filter = "All Files|*.*";
            dlgSaveFile.Title = "Select a file to save";

            if (dlgSaveFile.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dlgSaveFile.FileName, _edtResult.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }
    }



}
