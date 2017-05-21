using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace VmdkLoader {
    class DiskpartProxy :IDisposable{
        Process process;
        public DiskpartProxy() {
            process = new Process();
            var dir = Utility.GetSystemDir();
            string filename = $@"diskpart.exe";
            process.StartInfo.FileName = filename;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
        public event DataReceivedEventHandler DataReceived;
        StringBuilder output = new StringBuilder();
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            if (this.DataReceived != null) {
                DataReceived(this, e);
            }
        }

        void CreateVmdk(string filename) {
            CreateVmdk(filename, 1000);
        }

        public void CreateVmdk(string filename, int size) {
            string command = $"create vdisk file=\"{filename}\" type=expandable maximum={size}";
            ExecuteCommand(command, process);
            Thread.Sleep(2000);
        }

        public void AttachVmdk() {
            string command="Attach vdisk";
            ExecuteCommand(command, process);
            Thread.Sleep(1000);
        }

        static void ExecuteCommand(string command, Process process) {
            process.StandardInput.WriteLine(command);
            process.StandardInput.Flush();
        }

        public void LoadVmdk(string filename) {
            MakeSureFileExists(filename);
            FileInfo fi = new FileInfo(filename);
            MakeSureIsVmdkFile(fi);
            MakeSureTmpFileNotExists(fi);
            string tmpFileName = Rename(fi, "tmp.vmdk");
            CreateVmdk(filename);
            File.Delete(filename);
            RenameTmpFileToOriginal(tmpFileName, filename);
            AttachVmdk();
        }

        private static void MakeSureTmpFileNotExists(FileInfo fi) {
            string tmpFilename = $@"{fi.DirectoryName}tmp.vmdk";
            if (File.Exists(tmpFilename)) {
                throw new Exception("tmp.vmdk already exists");
            }
        }

        private static void RenameTmpFileToOriginal(string tmpFileName, string filename) {
            File.Move(tmpFileName, filename);
        }


        private static void MakeSureFileExists(string filename) {
            if (!File.Exists(filename)) {
                throw new Exception("file not exists");
            }
        }

        private static void MakeSureIsVmdkFile(FileInfo fi) {
            if (fi.Extension.ToLower() != ".vmdk") {
                throw new Exception("not a vmdk file");
            }
        }

        private static string Rename(FileInfo fi, string newName) {
            string newFileName = $@"{fi.DirectoryName}\{newName}";
            fi.MoveTo(newFileName);
            return newFileName;
        }

        public void Dispose() {
            process.Close();
        }
    }
}
