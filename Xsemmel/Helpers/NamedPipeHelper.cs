using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace XSemmel.Helpers
{
    static class NamedPipeHelper
    {
        private const string NAMEDPIPENAME = "XSemmelNamedPipe";

        public static string Read(string pipeName)
        {
            string lineEndChar = Environment.NewLine;
            StringBuilder sb = new StringBuilder();
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(NAMEDPIPENAME+pipeName))
            {
                pipeClient.Connect();
                using (StreamReader sr = new StreamReader(pipeClient))
                {
                    string temp;
                    while ((temp = sr.ReadLine()) != null)
                    {
                        sb.Append(temp);
                        sb.Append(lineEndChar);
                    }
                }
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - lineEndChar.Length, lineEndChar.Length);
            }
            return sb.ToString();
        }


        public static void Write(string pipename, string text)
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(NAMEDPIPENAME+pipename))
            {
                pipeServer.WaitForConnection();
                using (StreamWriter sw = new StreamWriter(pipeServer))
                {
                    sw.AutoFlush = true;
                    sw.Write(text);
                }
                pipeServer.Close();
            }
        }

        private static int _pipeIndex = 1;

        public static string StartNewListeningXSemmel()
        {
            string pipeName = _pipeIndex++.ToString();
            string exePath = Environment.GetCommandLineArgs()[0];
            Process.Start(exePath, "/p="+pipeName);
            return pipeName;
        }


    }
}
