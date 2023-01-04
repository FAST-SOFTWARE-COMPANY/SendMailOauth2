using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SendMailOAuth2
{
    class LogWriter
    {
        static string LogDirectory = Directory.GetCurrentDirectory();
        static string LogFile = "Log.txt";
        public static void Write(string message)
        {
            var LogPath = Path.Combine(LogDirectory, LogFile);
            var fileInfo = new FileInfo(LogPath);
            if (!fileInfo.Exists)
            {
                fileInfo.Create();
            }
            var sw = File.AppendText(LogPath);
            sw.WriteLine("--------------------------------------------------------------------");
            sw.WriteLine(String.Format("Date: {0}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")));
            sw.WriteLine(String.Format("Message: {0}", message));
            sw.WriteLine("--------------------------------------------------------------------");
            sw.WriteLine();
            sw.Close();
        }
    }
}
