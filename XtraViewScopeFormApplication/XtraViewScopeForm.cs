using ScopeLibrary.ConfigManagement;
using ScopeLibrary.SignalAnalysis;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using XtraViewScope.ConnectionManagement;
using XtraViewScope.ScopeAnalysis;

namespace XtraViewScopeFormApplication
{
    public partial class XtraViewScopeForm : Form
    {
        public XtraViewScopeForm()
        {
            InitializeComponent();

            if (Directory.Exists(@"."))
            {
                foreach (string filename in Directory.GetFiles(@"."))
                {
                    if (filename.ToLower().Contains("config"))
                    {
                        configFilePath.Text = Path.GetFullPath(filename);
                    }
                    //progressReportLinkLabel.Text += filename + " ";
                    //System.Diagnostics.Debug.WriteLine(filename);
                }
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            progressReportLinkLabel.Text = "";
            progressReportLinkLabel.Links.Clear();
            ChangeControlState(false);

            backgroundWorker1.RunWorkerAsync();  
        }

        //This thread is called when the user starts the application.
        //It is invoked using a background worker so that the UI does not appear to freeze and the user can stop the application.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            Program.configManager = new XmlConfigManager();
            Program.configManager.ConfigFilePath = Path.GetFullPath(ConfigFilePath);
            Program.configManager.loadConfigDocument();

            SignalAnalyser signalAnalyser = new Add2SignalAnalyser();

            XtraViewScopeConnectionManager xtraViewScopeConnectionManager = new XtraViewScopeConnectionManager();

            while (true)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                stopwatch.Reset();
                stopwatch.Start();

                System.Diagnostics.Debug.WriteLine("time before acquisition " + stopwatch.Elapsed);
                xtraViewScopeConnectionManager.StartAcquisition();
                System.Diagnostics.Debug.WriteLine("time after acquisition " + stopwatch.Elapsed);
                signalAnalyser.StartTime = xtraViewScopeConnectionManager.StartTime;
                signalAnalyser.Waveforms = xtraViewScopeConnectionManager.Waveforms;
                signalAnalyser.WaveformInfo = xtraViewScopeConnectionManager.WaveformInfo;
                Program.report.ReportContents.SignalAnalysisResultContainer = signalAnalyser.analyseScopeSignal();

                Program.reportWriter.OutputDirectory = OutputDirectory;
                Program.reportWriter.FilePathString = FileNameFormat;
                Program.reportWriter.Report = Program.report;

                //Thread reportWriterThread = new Thread (Program.reportWriter.WriteReport);
                //reportWriterThread.Start();

                Program.reportWriter.WriteReport();

                backgroundWorker1.ReportProgress(0, Program.reportWriter.FilePathString);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine("total time " + stopwatch.Elapsed);                
            }
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressReportLabel.Text = "Wrote to: " + e.UserState.ToString();
            progressReportLinkLabel.Text = "Wrote to: " + e.UserState.ToString();
            progressReportLinkLabel.Links.Clear();
            progressReportLinkLabel.Links.Add(new LinkLabel.Link("Wrote to: ".Length, e.UserState.ToString().Length));
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                progressReportLinkLabel.Text = "Cancelled scope signal analysis";
                progressReportLinkLabel.Links.Clear();
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error. " + (e.Error as Exception).ToString());
                progressReportLinkLabel.Text = "Error";
                progressReportLinkLabel.Links.Clear();
            }
            else
            {
                progressReportLinkLabel.Text = "The task has been completed. Results: " + e.Result.ToString();
                progressReportLinkLabel.Links.Clear();
            }
            
            ChangeControlState(true);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                progressReportLinkLabel.Text = "Stopping...";
                progressReportLinkLabel.Links.Clear();
            }
            backgroundWorker1.CancelAsync();
        }

        private void configFilePath_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                configFilePath.Text = openFileDialog1.FileName;
            }
        }

        private void outputDirectory_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                outputDirectory.Text = openFileDialog1.FileName;
            }
        }

        string ConfigFilePath
        {
            get
            {
                return configFilePath.Text;
            }
        }

        string OutputDirectory
        {
            get
            {
                return outputDirectory.Text;
            }
        }

        string FileNameFormat
        {
            get
            {
                return fileNameFormat.Text;
            }
        }


        void ChangeControlState(bool isEnabled)
        {
            configFilePath.Enabled = isEnabled;
            outputDirectory.Enabled = isEnabled;
            fileNameFormat.Enabled = isEnabled;
            startButton.Enabled = isEnabled;
            this.Refresh();
        }

        private void progressReportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Program.reportWriter.FilePathString != null)
            {
                System.Diagnostics.Process.Start(Program.reportWriter.FullFilePath);
                progressReportLinkLabel.LinkVisited = true;
            }
        }
    }
}
