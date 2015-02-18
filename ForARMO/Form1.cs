using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace ForARMO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        int count = 0, i=0;
        private static Thread m_thread = null;
        private string Nm, Str;
        

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
               label9.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Properties.Settings.Default.Name = this.textBox1.Text;
            Properties.Settings.Default.Str = this.textBox2.Text;
            Properties.Settings.Default.Path = this.label9.Text;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            textBox1.Text = Properties.Settings.Default.Name;
            textBox2.Text = Properties.Settings.Default.Str;
            label9.Text = Properties.Settings.Default.Path;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.LoadSettings();
        }

        

        public TreeNode FileSearchFunction(DirectoryInfo dir, BackgroundWorker worker, DoWorkEventArgs e)
        {
            TreeNode vrt = new TreeNode(dir.Name);
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                
                try
                {
                    foreach (DirectoryInfo inf in dir.GetDirectories())
                    {
                        TreeNode branch = FileSearchFunction(inf, worker, e);
                        if(branch.Nodes.Count!= 0)
                            vrt.Nodes.Add(branch);
                    }
                    foreach (FileInfo inf in dir.GetFiles(Nm))
                    {
                        label4.BeginInvoke((MethodInvoker) delegate() {this.label4.Text = inf.Name; });
                        label8.BeginInvoke((MethodInvoker) delegate() {this.label8.Text = Convert.ToString(count); });
                        count++;
                        string tmp = File.ReadAllText(inf.FullName, System.Text.Encoding.GetEncoding(1251));
                        if (tmp.IndexOf(Str, StringComparison.CurrentCulture) != -1)
                        {
                            TreeNode node = new TreeNode(inf.Name,0,0);
                            node.BackColor = Color.Red;
                            vrt.Nodes.Add(node);
                        }
                    }
                    
                }
                catch  { }
            }
            return vrt;            
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (label9.Text != "" & textBox1.Text != "" & textBox2.Text != "")
            {
                treeView1.Nodes.Clear();
                count = 0;
                Nm = textBox1.Text;
                Str = textBox2.Text;
                DirectoryInfo dir = new DirectoryInfo(label9.Text);
                i = 0;
                timer1.Interval = 1000;
                timer1.Enabled = true;
                label6.Text = Convert.ToString(i);
                if (backgroundWorker1.IsBusy != true)
                {
                    backgroundWorker1.RunWorkerAsync(dir);
                    button3.Enabled = true;
                }
            }
            else
                MessageBox.Show("Заполните поля!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                backgroundWorker1.CancelAsync();
            }
            timer1.Stop();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = FileSearchFunction((DirectoryInfo)e.Argument, worker, e);
            treeView1.BeginInvoke((MethodInvoker)delegate() { this.treeView1.Nodes.Add((TreeNode)e.Result); }); 
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
                timer1.Stop();
                label4.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            i++;
            label6.Text = Convert.ToString(i);
        }
    }
}
