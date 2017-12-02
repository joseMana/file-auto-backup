using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileAutoBackup
{
    class Program
    {
        public static string directoryToWatchPath = @"C:\DirectoryToWatch";
        public static string fileNameToWatchPath = "MyTextDocument.txt";
        public static string gitCommitMessage => "\"Updated : {DateTime.Now.ToString()}\"";


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
            RunCommitBatchFile();
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
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

            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }
    }
}
