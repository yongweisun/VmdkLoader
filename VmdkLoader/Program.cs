using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VmdkLoader {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            //TestProcess();
            //Console.Read();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        static void TestProcess() {
            string file = @"s:\exp.vmdk";
            int lineCount = 0;
            StringBuilder output = new StringBuilder();
            
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                // Prepend line numbers to each line of the output.
                if (!String.IsNullOrEmpty(e.Data)) {
                    lineCount++;
                    //output.Append("\n[" + lineCount + "]: " + e.Data);
                    Console.WriteLine("[" + lineCount + "]: " + e.Data);
                }
            });

            process.Start();

            // Asynchronously read the standard output of the spawned process. 
            // This raises OutputDataReceived events for each line of output.
            process.BeginOutputReadLine();
            //process.BeginErrorReadLine();

            string line = "";
            do {
                line=Console.ReadLine();
                process.StandardInput.WriteLine(line);

            } while (line != null && line != string.Empty);
            process.WaitForExit();

            // Write the redirected output to this application's window.
            Console.WriteLine(output);

            process.WaitForExit();
            process.Close();

            Console.WriteLine("\n\nPress any key to exit.");
            Console.ReadLine();
        }
    }
}
