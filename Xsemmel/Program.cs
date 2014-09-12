using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Shell;
using XSemmel.Configuration;
using XSemmel.Helpers;

namespace XSemmel
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (x, exargs) =>
            {
                Trace.WriteLine(exargs.ExceptionObject.ToString());

//                EventLog m_EventLog = new EventLog("");
//                m_EventLog.Source = "Xsemmel";
//                m_EventLog.WriteEntry(exargs.ExceptionObject.ToString(),
//                    EventLogEntryType.FailureAudit);

                MessageBox.Show(exargs.ExceptionObject.ToString(), "Error");
            };

            if (XSConfiguration.Instance.Config.ShowSplashScreen)
            {
                SplashScreen splashScreen = new SplashScreen("Images\\Splash.png");
                splashScreen.Show(true);
            }
         
            System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
            System.Windows.Forms.Application.EnableVisualStyles();

            Application app = new Application();

            app.Resources.BeginInit();
//            app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2013/Generic.xaml") });
            app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Generic.xaml") });
            app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Silver.xaml") });
            app.Resources.EndInit();

            app.Startup += OnStartup;

            MainWindow main = new MainWindow();
            if (args.Length == 1)
            {
                if (args[0].StartsWith("/p="))
                {
                    //named pipe
                    main.OpenFile(MainWindow.NAMEDPIPE+args[0].Substring(3));
                }
                else
                {
                    main.OpenFile(args[0]);
                }
            }
            else
            {
                var type = XSConfiguration.Instance.Config.OpenAtStartup;
                switch (type)
                {
                    case ConfigObj.OpenType.Nothing:
                        break;
                    case ConfigObj.OpenType.LastOpenedFile:
                        if (XSConfiguration.Instance.Config.LastOpenedFile != null)
                        {
                            string lastOpenedFile = XSConfiguration.Instance.Config.LastOpenedFile;
                            if (FileHelper.FileExists(lastOpenedFile))
                            {
                                main.OpenFile(lastOpenedFile);
                            }
                        }
                        break;
                    case ConfigObj.OpenType.XmlInClipboard:
                        if (Clipboard.ContainsText())
                        {
                            string clip = Clipboard.GetText();
                            if (!clip.StartsWith("<") && File.Exists(clip))
                            {
                                //Clipboard contains filename
                                main.OpenFile(clip);
                            }
                            else
                            {
                                main.OpenFile(MainWindow.CLIPBOARD);
                            }
                        }
                        break;
                    default:
                        Debug.Fail(type + " not implemented");
                        break;
                }
            }
            app.Run(main);
        }

        private static void OnStartup(object sender, StartupEventArgs e)
        {
//            JumpTask task = new JumpTask
//            {
//                Title = "Check for Updates",
//                Arguments = "/update",
//                Description = "Cheks for Software Updates",
//                CustomCategory = "Actions",
//                IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
//                ApplicationPath = Assembly.GetEntryAssembly().CodeBase
//            };

//            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;  

            JumpList jumpList = new JumpList();
//            jumpList.JumpItems.Add(task);
            jumpList.ShowFrequentCategory = false;
            jumpList.ShowRecentCategory = false;

            JumpList.SetJumpList(Application.Current, jumpList);
        }
    }
}
