using ScopeLibrary.ConfigManagement;
using ScopeLibrary.ConnectionManagement;
using ScopeLibrary.ReportWriting;
using ScopeLibrary.SignalAnalysis;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using XtraViewScope.ConnectionManagement;
using XtraViewScope.ReportWriting;
using XtraViewScope.ScopeAnalysis;

namespace XtraViewScopeFormApplication
{
    public partial class XtraViewScopeForm : Form
    {
        public XtraViewScopeForm()
        {
            InitializeComponent();
            Program.log.Info("Form initialised");

            initialiseScopeConfigurationComponenents();
            Program.log.Info("Scope configuration components initialised");
        }

        private void initialiseScopeConfigurationComponenents()
        {
            //Try and find a config file in the directory of the application
            foreach (string filename in Directory.GetFiles(@"."))
            {
                if (filename.ToLower().Contains("config"))
                {
                    configFilePath.Text = Path.GetFullPath(filename);
                }
            }

            //Instantiate the config file using the appropriate format
            if (configFilePath.Text.ToLower().Contains(".properties"))
            {
                Program.configManager = new PropertyConfigManager();
            }
            else
            {
                Program.configManager = new XmlConfigManager();
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            //startButton.Text = "Busy...";
            pictureBox1.Image = Properties.Resources.Busy;
            progressReportLinkLabel.Text = "";
            progressReportLinkLabel.Links.Clear();
            ChangeControlState(false);

            //Instantiate the report object and parse the config file
            Program.reportWriter.Report = new Report();
            Program.configManager.ConfigFilePath = Path.GetFullPath(ConfigFilePath);
            Program.configManager.loadConfigDocument();

            //Set the report content type
            if (Program.configManager.getProperty("ReportContents") != null)
            {
                if (Program.configManager.getProperty("ReportContents").Equals("Xml"))
                {
                    Program.reportWriter.Report.ReportContents = new XmlReportContents();
                }
                else if (Program.configManager.getProperty("ReportContents").Equals("Json"))
                {
                    Program.reportWriter.Report.ReportContents = new JsonReportContents();
                }
            }
            else
            {
                Program.reportWriter.Report.ReportContents = new XmlReportContents();
            }

            //Do the work using a background worker to free the UI
            backgroundWorker1.RunWorkerAsync();  
        }

        private Thread reportWriterThread;

        //This thread is called when the user starts the application.
        //It is invoked using a background worker so that the UI does not appear to freeze and the user can stop the application.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            SignalAnalyser signalAnalyser = new Add2SignalAnalyser();

            IScopeConnectionManager xtraViewScopeConnectionManager = new XtraViewScopeConnectionManager();
            int count = 0;
            while (true)
            {
                count++;
                //Acquire the scope signal
                xtraViewScopeConnectionManager.StartAcquisition();
                Program.log.Info("Scope acquisition started for the " + ScopeLibrary.Util.NumberToString.AddOrdinal(count) + " time");
                signalAnalyser.StartTime = xtraViewScopeConnectionManager.StartTime;
                signalAnalyser.Waveforms = xtraViewScopeConnectionManager.Waveforms;
                signalAnalyser.WaveformInfo = xtraViewScopeConnectionManager.WaveformInfo;

                //Analyse the acquired signal
                Program.reportWriter.Report.ReportContents.SignalAnalysisResultContainer = signalAnalyser.analyseScopeSignal();
                Program.reportWriter.OutputDirectory = OutputDirectory;
                Program.reportWriter.FilePathString = FileNameFormat;

                //This creates the file with the analysed data.
                //Run in a thread so that the scope can wait for the next signal.
                reportWriterThread = new Thread(Program.reportWriter.WriteReport);
                reportWriterThread.Start();

                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    Program.log.Info("Acquisition cancelled by user, stopping");
                    return;
                }
                //Generate the link on the interface in another thread that waits for the report to be created first.
                Thread reportProgressThread = new Thread(() => backgroundWorker1.ReportProgress(0));
                reportProgressThread.Start();
            }
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Wait for the report writer to finish so that the name is generated correctly
            pictureBox1.Image = Properties.Resources.WritingReport;
            reportWriterThread.Join();
            Program.log.Info("Wrote to: " + Program.reportWriter.FullFilePath);
            progressReportLinkLabel.Text = "Wrote to: " + Program.reportWriter.FullFilePath;
            progressReportLinkLabel.Links.Clear();
            progressReportLinkLabel.Links.Add(new LinkLabel.Link("Wrote to: ".Length, Program.reportWriter.FullFilePath.Length));
            pictureBox1.Image = Properties.Resources.Busy;
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
                Program.log.Fatal((e.Error as Exception).ToString(), e.Error as Exception);
            }
            else
            {
                progressReportLinkLabel.Text = "The task has been completed. Results: " + e.Result.ToString();
                progressReportLinkLabel.Links.Clear();
            }

            pictureBox1.Image = null;
            ChangeControlState(true);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                pictureBox1.Image = Properties.Resources.Stopping;
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
                Process.Start(Program.reportWriter.FullFilePath);
                progressReportLinkLabel.LinkVisited = true;
            }
        }
    }
}
