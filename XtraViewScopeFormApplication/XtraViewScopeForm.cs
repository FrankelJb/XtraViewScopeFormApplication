using ScopeLibrary.ConfigManagement;
using ScopeLibrary.SignalAnalysis;
using System;
using System.ComponentModel;
using System.IO;
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
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            progressReportLabel.Text = "";
            ChangeControlState(false);

            backgroundWorker1.RunWorkerAsync();  
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Program.configManager = new XmlConfigManager();
            Program.configManager.ConfigFilePath = Path.GetFullPath(ConfigFilePath);
            Program.configManager.loadConfigDocument();

            SignalAnalyser signalAnalyser = new Add2SignalAnalyser();

            XtraViewScopeConnectionManager xtraViewScopeConnectionManager = new XtraViewScopeConnectionManager();

            while (true)
            {
                xtraViewScopeConnectionManager.StartAcquisition();

                signalAnalyser.StartTime = xtraViewScopeConnectionManager.StartTime;
                signalAnalyser.Waveforms = xtraViewScopeConnectionManager.Waveforms;
                signalAnalyser.WaveformInfo = xtraViewScopeConnectionManager.WaveformInfo;

                Program.reportContents.SignalAnalysisResultContainer = signalAnalyser.analyseScopeSignal();
                Program.report.ReportContents = Program.reportContents;
                Program.reportWriter.OutputDirectory = OutputDirectory;
                Program.reportWriter.FilePathString = FileNameFormat;
                Program.reportWriter.WriteReport(Program.report);

                backgroundWorker1.ReportProgress(0, Program.reportWriter.FilePathString);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressReportLabel.Text = "Wrote to: " + e.UserState.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                progressReportLabel.Text = "Cancelled scope signal analysis";
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error. " + (e.Error as Exception).ToString());
                progressReportLabel.Text = "Error";
            }
            else
            {
                progressReportLabel.Text = "The task has been completed. Results: " + e.Result.ToString();
            }
            
            ChangeControlState(true);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            progressReportLabel.Text = "Stopping...";
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
    }
}
