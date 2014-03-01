using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Fluent;
using ICSharpCode.AvalonEdit;
using TaskDialogInterop;
using XSemmel.Commands;
using XSemmel.Configuration;
using XSemmel.Differ;
using XSemmel.Editor;
using XSemmel.Generator;
using XSemmel.Helpers;
using XSemmel.PrettyPrinter;
using System.Windows.Input;

namespace XSemmel
{
    public partial class MainWindow
    {
        private EditorFrame Editor { get; set; }
        public const string CLIPBOARD = ":Clipboard";
        public const string NAMEDPIPE = ":XSemmelNamedPipe";
        public const string NEW = ":New";

        private FileSystemWatcher _watcher = new FileSystemWatcher();

        private const int MAX_FILESIZE_IN_BYTES = 2 * 1024 * 1024; //2MB
        
        private readonly Data _data = new Data();

        /// <summary>
        /// Never bind to null, this is why we need to have this property that encapsulates all dynamic values
        /// </summary>
        public Data Data
        {
            get { return _data; }
        }

        private void setFilename()
        {
            if (Editor != null && Editor.XSDocument != null && Editor.XSDocument.Filename != null)
            {
                _lblCurrentFileName.Text = Editor.XSDocument.Filename;
                try
                {
                    Title = Path.GetFileName(Editor.XSDocument.Filename) + " - Xsemmel";
                }
                catch
                {
                    Title = "Xsemmel";
                }
            }
            else
            {
                _lblCurrentFileName.Text = "";
                Title = "Xsemmel";
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            createTaskPanes(null);

            Application.Current.SessionEnding += (sender, y) => { y.Cancel = !allowToClose(); };

            Closing += (sender, y) => { y.Cancel = !allowToClose(); };
        }


        private void setupExternalTools()
        {
            var et = new ExternalTools(Editor);
            _ribbongroupExternalTools.Visibility = et.IsAnySpecified() ? Visibility.Visible : Visibility.Collapsed;
            et.Configure(_ribbongroupExternalTools);
        }


        private void mnuXmlGenerator_Click(object sender, RoutedEventArgs e)
        {
            new XmlGenerator { Owner = this }.ShowDialog();
        }

        private void mnuDiff_Click(object sender, RoutedEventArgs e)
        {
            new Diffy(Editor) { Owner = this }.ShowDialog();
        }


        private bool allowToClose()
        {
            if (Editor != null)
            {
                return Editor.AllowToClose();
            }
            return true;
        }

        public void OpenFile(string filePath)
        {
            if (!allowToClose())
            {
                return;
            }
            if (   !NEW.Equals(filePath) 
                && !filePath.StartsWith(NAMEDPIPE)
                && !CLIPBOARD.Equals(filePath) 
                && !FileHelper.FileExists(filePath))
            {
                MessageBox.Show(this, string.Format("The file '{0}' does not exist", filePath), "File does not exist", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                _watcher.EnableRaisingEvents = false;
                Mouse.OverrideCursor = Cursors.Wait;

                XSDocument doc;
                if (filePath.StartsWith(NAMEDPIPE))
                {
                    string pipeName = filePath.Substring(NAMEDPIPE.Length);
                    string clip = NamedPipeHelper.Read(pipeName);
                    if (clip.Length > MAX_FILESIZE_IN_BYTES)
                    {
                        if (!openTooLargeFiles())
                        {
                            return;
                        }
                    }
                    doc = new XSDocument(clip);
                }
                else if (CLIPBOARD.Equals(filePath))
                {
                    Debug.Assert(Clipboard.ContainsText(), "Already checked");
                    string clip = Clipboard.GetText();
                    if (clip.Length > MAX_FILESIZE_IN_BYTES)
                    {
                        if (!openTooLargeFiles())
                        {
                            return;
                        }
                    }
                    doc = new XSDocument(clip);
                }
                else if (NEW.Equals(filePath))
                {
                    doc = new XSDocument(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                }
                else
                {
                    if (new FileInfo(filePath).Length > MAX_FILESIZE_IN_BYTES)
                    {
                        if (!openTooLargeFiles())
                        {
                            return;
                        }
                    }

                    //open file, even if it is locked by another process
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        string xml = streamReader.ReadToEnd();
                        doc = new XSDocument(xml, null, filePath);
                    }

                    XSConfiguration.Instance.Config.AddRecentlyUsedFile(filePath);

                    if (XSConfiguration.Instance.Config.WatchCurrentFileForChanges)
                    {
                        _watcher.Dispose();
                        _watcher = new FileSystemWatcher
                                       {
                                           Path = Path.GetDirectoryName(filePath),
                                           NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                                           Filter = Path.GetFileName(filePath),
                                           IncludeSubdirectories = false
                                       };
                        _watcher.Changed += OnChanged;
                        _watcher.EnableRaisingEvents = true;
                    }
                }
                createTaskPanes(doc);
                setFilename();

                setupExternalTools();

                //http://stackoverflow.com/questions/3109080/focus-on-textbox-when-usercontrol-change-visibility
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Keyboard.Focus(Editor.XmlEditor);
                    bool success = Editor.XmlEditor.Focus();
//                    Debug.Assert(success);
                }, DispatcherPriority.Render);
            }
            catch (Exception e)
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show(this, "Cannot open file: " + e.Message, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        public void SaveFile(string filename)
        {
            bool wasWatcherEnabled = _watcher.EnableRaisingEvents;
            try
            {
                _watcher.EnableRaisingEvents = false;
                if (Editor.XSDocument.Filename == null || Editor.XSDocument.Filename != filename)
                {
                    //save as...
                    TextEditor textEditor = Editor.XmlEditor;

                    bool isModified = textEditor.IsModified;
                    Debug.Assert(textEditor.Encoding == null ||
                                 Equals(textEditor.Encoding, XSConfiguration.Instance.Config.Encoding));
                    checkForReadonly(filename);
                    textEditor.Save(filename);
                    textEditor.IsModified = isModified;
                    XSConfiguration.Instance.Config.AddRecentlyUsedFile(filename);
                    if (Editor.XSDocument.Filename == null)
                    {
                        Editor.XSDocument.Filename = filename;
                    }
                }
                else
                {
                    //save...
                    Debug.Assert(Editor.XmlEditor.Encoding == null ||
                                 Equals(Editor.XmlEditor.Encoding, XSConfiguration.Instance.Config.Encoding));
                    Debug.Assert(Editor.XSDocument.Filename == filename);
                    checkForReadonly(filename);
                    Editor.XmlEditor.Save(filename);
                    Editor.XSDocument.Xml = Editor.XmlEditor.Text;
                }
            }
            finally
            {
                _watcher.EnableRaisingEvents = wasWatcherEnabled;
                setFilename();
            }
        }

        private void checkForReadonly(string filename)
        {
            if (File.Exists(filename))
            {
                var attributes = File.GetAttributes(filename);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    TaskDialogResult res = TaskDialog.Show(
                                        new TaskDialogOptions
                                        {
                                            AllowDialogCancellation = true,
                                            Owner = Application.Current.MainWindow,
                                            Title = "Save of Read-Only File",
                                            MainInstruction = "The file " + Path.GetFileName(filename) + " cannot be saved because it is write-protected.",
                                            Content = "Xsemmel can attempt to remove the write-protection and overwrite the file",
                                            CommandButtons = new[] { "Overwrite", "Cancel" },
                                            MainIcon = VistaTaskDialogIcon.Information,
                                            ExpandedInfo = "Path: " + filename
                                        });

                    switch (res.CommandButtonResult)
                    {
                        case 0: //overwrite
                            attributes = attributes & ~FileAttributes.ReadOnly;
                            File.SetAttributes(filename, attributes);
                            break;
                        case 1: //cancel
                        case null: //cancel
                            throw new Exception("File was not saved.");
                        default:
                            Debug.Fail("");
                            break;
                    }
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var res = MessageBox.Show(this, "Current file was changed by external process. Do you want to reload?",
                    "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    OpenFile(e.FullPath);
                }
            }));
            _watcher.EnableRaisingEvents = false;
        }


        private bool openTooLargeFiles()
        {
            var result = MessageBox.Show(this,
                "The data you selected to open is very large. Xsemmel may not be able to handle it. Do you want to try it anyway?",
                "Too much data", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }


        private void createTaskPanes(XSDocument Doc)
        {
            _tabCtrl.Content = null;
            
            if (Doc != null)
            {
                Editor = new EditorFrame(Doc, Data);
                _tabCtrl.Content = Editor;
                
                if (Doc.Filename != null && Doc.Filename.ToLower().EndsWith(".xsd"))
                {
                    tabXsd.Visibility = Visibility.Visible;
                }
                else
                {
                    tabXsd.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void RibbonWindow_Closed(object sender, EventArgs e)
        {
            if (Editor != null && Editor.XSDocument != null && Editor.XSDocument.Filename != null)
            {
                XSConfiguration.Instance.Config.LastOpenedFile = Editor.XSDocument.Filename;
            }
            else
            {
                XSConfiguration.Instance.Config.LastOpenedFile = null;
            }
            new QuitCommand().Execute(null);
        }

        private void mnuBulkPrettyPrint_Click(object sender, RoutedEventArgs e)
        {
            BulkPrettyPrint.ShowDialog(this);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var qai = Ribbon.QuickAccessItems;
            foreach (var quickAccessMenuItem in qai)
            {
                Ribbon.AddToQuickAccessToolBar(quickAccessMenuItem);
            }
        }

        private void _tabCtrl_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                if (files != null && files.Length > 0)
                {
                    OpenFile(files[0]);        
                }
            }
        }

        private void insertEntity_OnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as Fluent.MenuItem;
            if (menuItem != null && Editor != null && Editor.XmlEditor != null)
            {
                try
                {
                    string stringToInsert = string.Format("&{0};", menuItem.Header);
                    Editor.XmlEditor.SelectedText = stringToInsert;
                } 
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error: " + ex.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            else
            {
                Debug.Fail("");
            }
        }

    }
}
