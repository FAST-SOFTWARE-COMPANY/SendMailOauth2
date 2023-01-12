using SendMailOauth2.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SendMailOAuth2
{
    class LogWriter
    {
        static string LogDirectory = "Logs";
        static int ApproximateSize = 5; // đv: MB,  5MB
        public static void Write(string message)
        {
            var LogPath = CheckLogDirectory();
            var fileInfo = new FileInfo(LogPath);
            if (!fileInfo.Exists)
            {
                fileInfo.Create();
            }
            using (var sw = File.AppendText(LogPath))
            {
                sw.WriteLine(String.Format("Date: {0}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")));
                sw.WriteLine(String.Format("Message: {0}", message));
                sw.WriteLine("--------------------------------------------------------------------");
                sw.WriteLine();
                sw.Close();
            }
        }

        public static void WriteException(Exception ex, string message)
        {
            var LogPath = CheckLogDirectory();
            var fileInfo = new FileInfo(LogPath);
            if (!fileInfo.Exists)
            {
                fileInfo.Create();
            }
            using (var sw = File.AppendText(LogPath))
            {
                sw.WriteLine(String.Format("Date: {0}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")));
                sw.WriteLine(String.Format("Message: {0}", message));
                sw.WriteLine(String.Format("Exception: {0}", ex.Message));
                if(ex.InnerException != null)
                {
                    sw.WriteLine(ex.InnerException.Message);
                }
                sw.WriteLine("--------------------------------------------------------------------");
                sw.WriteLine();
                sw.Close();
            }
        }

        private static string CheckLogDirectory()
        {
            var absoluteLogDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), LogDirectory);
            var absoluteLogFilePath = Path.Combine(absoluteLogDirectoryPath, "Logs.txt");
            var directoryInfo = new DirectoryInfo(absoluteLogDirectoryPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
                File.Create(absoluteLogFilePath);
            }
            else
            {
                var latestLogFile = new FileInfo(absoluteLogFilePath);
                if (latestLogFile.Exists)
                {
                    var sizeInMB = latestLogFile.Length / (1024 * 1024);
                    if (sizeInMB > ApproximateSize)
                    {
                        var newName = "Log" + DateTime.Now.ToString() + ".bk";
                        latestLogFile.Rename(newName);
                        File.Create(absoluteLogFilePath);
                    }
                }
                else
                {
                    latestLogFile.Create();
                }
            }
            return absoluteLogFilePath;
        }
    }
}
