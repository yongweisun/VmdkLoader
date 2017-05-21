using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VmdkLoader {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }


        DiskpartProxy proxy = new DiskpartProxy();
        private void LoadVmdk(string filename) {
            try {
                proxy.LoadVmdk(filename);
            }
            catch (System.IO.IOException ioex) {
                MessageBox.Show($"if file locked by another process, please computer management tool, select diskpart management, using detach vhd function.\n{ioex.Message}");
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e) {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK) {
                LoadVmdk(this.openFileDialog1.FileName);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            proxy.Dispose();
            base.OnFormClosing(e);
        }

        private void MainForm_Load(object sender, EventArgs e) {
            proxy.DataReceived += Proxy_DataReceived;
        }

        private void Proxy_DataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e) {
            DataReceivedEventHandler dele = AsyncChangeText;
            this.txtLog.Invoke(dele,new object[] { sender, e });
        }

        private void AsyncChangeText(object sender, System.Diagnostics.DataReceivedEventArgs e) {
            this.txtLog.Text += e.Data+"\r\n";
        }

        private void btnCreate_Click(object sender, EventArgs e) {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK) {
                CreateVmdk(saveFileDialog1.FileName);
            }
        }

        private void CreateVmdk(string fileName) {
            try {
                proxy.CreateVmdk(fileName,(int)nudSize.Value);
                proxy.AttachVmdk();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}