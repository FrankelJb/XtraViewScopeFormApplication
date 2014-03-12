using ScopeLibrary.ConfigManagement;
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

            initializeScopeComponents();
        }

        private void initializeScopeComponents()
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
                Program.reportWriter.Report.ReportContents = new JsonReportContents();
            }

            //Do the work using a background worker to free the UI
            backgroundWorker1.RunWorkerAsync();  
        }

        private Thread reportWriterThread;

        //This thread is called when the user starts the application.
        //It is invoked using a background worker so that the UI does not appear to freeze and the user can stop the application.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();

            SignalAnalyser signalAnalyser = new Add2SignalAnalyser();

            XtraViewScopeConnectionManager xtraViewScopeConnectionManager = new XtraViewScopeConnectionManager();
            while (true)
            {
                //Acquire the scope signal
                stopwatch.Restart();
                xtraViewScopeConnectionManager.StartAcquisition();
                System.Diagnostics.Debug.WriteLine("acquisition complete " + stopwatch.Elapsed);
                signalAnalyser.StartTime = xtraViewScopeConnectionManager.StartTime;
                signalAnalyser.Waveforms = xtraViewScopeConnectionManager.Waveforms;
                signalAnalyser.WaveformInfo = xtraViewScopeConnectionManager.WaveformInfo;

                //Analyse the acquired signal
                Program.reportWriter.Report.ReportContents.SignalAnalysisResultContainer = signalAnalyser.analyseScopeSignal();
                System.Diagnostics.Debug.WriteLine("analysis complete " + stopwatch.Elapsed);
                Program.reportWriter.OutputDirectory = OutputDirectory;
                Program.reportWriter.FilePathString = FileNameFormat;

                //This creates the file with the analysed data.
                //Run in a thread so that the scope can wait for the next signal.
                System.Diagnostics.Debug.WriteLine("report started " + stopwatch.Elapsed);
                reportWriterThread = new Thread(Program.reportWriter.WriteReport);
                reportWriterThread.Start();

                //Program.reportWriter.WriteReport();

                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //Generate the link on the interface in another thread that waits for the report to be created first.
                Thread reportProgressThread = new Thread(() => backgroundWorker1.ReportProgress(0));
                reportProgressThread.Start();
                System.Diagnostics.Debug.WriteLine("complete " + stopwatch.Elapsed);
            }
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Wait for the report writer to finish so that the name is generated correctly
            reportWriterThread.Join();
            progressReportLinkLabel.Text = "Wrote to: " + Program.reportWriter.FilePathString;
            progressReportLinkLabel.Links.Clear();
            progressReportLinkLabel.Links.Add(new LinkLabel.Link("Wrote to: ".Length, Program.reportWriter.FullFilePath.Length));
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
                Process.Start(Program.reportWriter.FullFilePath);
                progressReportLinkLabel.LinkVisited = true;
            }
        }
    }
}
