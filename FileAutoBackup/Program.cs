using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FileAutoBackup
{
    class Program
    {
        public static string directoryToWatchPath = @"C:\Users\JosephM\AppData\Local\Google\Chrome\User Data\Default";
        public static string fileNameToWatchPath = "Bookmarks";
        public static string gitCommitMessage => $"\"Updated : {DateTime.Now.ToString()}\"";

        public static string gitCommand { get; set; }
        static void Main(string[] args)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = directoryToWatchPath;

            watcher.NotifyFilter = 
                NotifyFilters.LastAccess 
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName 
                | NotifyFilters.DirectoryName;

            watcher.Filter = fileNameToWatchPath;

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            new ManualResetEvent(false).WaitOne();
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(60000);
            RunCommitBatchFile();
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Thread.Sleep(60000);
            RunCommitBatchFile();
        }

        private static void RunCommitBatchFile()
        {
            Process cmd = new Process();

            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            cmd.Start();

            cmd.StandardInput.WriteLine($"cd {directoryToWatchPath}");
            cmd.StandardInput.WriteLine($"git add {fileNameToWatchPath}");
            cmd.StandardInput.WriteLine($"git commit -m {gitCommitMessage}");
            cmd.StandardInput.WriteLine($"git push");

            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }
    }
}
