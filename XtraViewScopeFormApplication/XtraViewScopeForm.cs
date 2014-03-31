using ScopeLibrary.ConfigManagement;
using ScopeLibrary.ConnectionManagement;
using ScopeLibrary.ReportWriting;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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

            //Load the config file and set the default save file format
            Program.configManager.ConfigFilePath = Path.GetFullPath(ConfigFilePath);
            Program.configManager.loadConfigDocument();

            //Set the file name to be <device name>_<channel_number>
            string deviceAndChannel = "";
            if (Program.configManager.getProperty("ResourceName") != null)
            {
                deviceAndChannel += Program.configManager.getProperty("ResourceName");
            }
            else
            {
                deviceAndChannel += "Dev1";
            }

            deviceAndChannel += "_";
            if (Program.configManager.getProperty("ChannelName") != null)
            {
                deviceAndChannel += Program.configManager.getProperty("ChannelName");
            }
            else
            {
                deviceAndChannel += "0";
            }

            FileNameFormat = deviceAndChannel;

            Program.signalAnalyserBackgroundWorker.DoWork += new DoWorkEventHandler(analyseSignalBackgroundWorker_doWork);
            Program.signalAnalyserBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(analyseSignalBackgroundWorker_RunWorkerCompleted);

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Text = "0";
            pictureBox1.Image = Properties.Resources.Busy;
            progressReportLinkLabel.Text = "";
            progressReportLinkLabel.Links.Clear();

            averageLabel.Text = "Average: ";
            shortestLabel.Text = "Shortest: ";
            longestLabel.Text = "Longest: ";

            //Instantiate the config file using the appropriate format
            if (configFilePath.Text.ToLower().Contains(".properties"))
            {
                Program.configManager = new PropertyConfigManager();
            }
            else
            {
                Program.configManager = new XmlConfigManager();
            }

            //Load the config file and set the default save file format
            Program.configManager.ConfigFilePath = Path.GetFullPath(ConfigFilePath);
            Program.configManager.loadConfigDocument();

            ChangeControlState(false);

            //We need new reportWriters each time the acquisition is started based on the configs
            Program.reportWriters = new List<IReportWriter>();

            //Set the report content type
            if (Program.configManager.getProperty("ReportContents") != null)
            {
                if (Program.configManager.getProperty("ReportContents").Equals("Xml"))
                {
                    IReportWriter reportWriter = new ReportWriter();
                    reportWriter.Report = new Report();
                    reportWriter.Report.ReportContents = new XmlReportContents();
                    Program.reportWriters.Add(reportWriter);
                }
                else if (Program.configManager.getProperty("ReportContents").Equals("Json"))
                {
                    IReportWriter reportWriter = new ReportWriter();
                    reportWriter.Report = new Report();
                    reportWriter.Report.ReportContents = new JsonReportContents();
                    Program.reportWriters.Add(reportWriter);
                }
                else if (Program.configManager.getProperty("ReportContents").Equals("XmlAndJson"))
                {
                    IReportWriter reportWriter = new ReportWriter();
                    reportWriter.Report = new Report();
                    reportWriter.Report.ReportContents = new XmlReportContents();
                    Program.reportWriters.Add(reportWriter);
                    
                    reportWriter = new ReportWriter();
                    reportWriter.Report = new Report();
                    reportWriter.Report.ReportContents = new JsonReportContents();
                    Program.reportWriters.Add(reportWriter);
                }
            }
            else
            {
                IReportWriter reportWriter = new ReportWriter();
                reportWriter.Report = new Report();
                reportWriter.Report.ReportContents = new XmlReportContents();
                Program.reportWriters.Add(reportWriter);
            }

            //Do the work using a background worker to free the UI
            backgroundWorker1.RunWorkerAsync();  
        }

        //This thread is called when the user starts the application.
        //It is invoked using a background worker so that the UI does not appear to freeze and the user can stop the application.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            SignalAnalyser signalAnalyser = new Add2SignalAnalyser();
            TimeSpan totalRunTime = new TimeSpan();
            TimeSpan shortestRunTime = TimeSpan.MaxValue;
            TimeSpan longestRunTime = TimeSpan.MinValue;
            Stopwatch stopwatch = new Stopwatch();

            IScopeConnectionManager xtraViewScopeConnectionManager = new XtraViewScopeConnectionManager();

            //The first time the hearbeat is sensed can be 0.0 < currentTime < 30.0 seconds. 
            //This data disrupts the timing captured below, so make sure that it isn't included by ignoring the first heartbeat
            //xtraViewScopeConnectionManager.StartAcquisition();

            int count = 1;
            while (true)
            {
                //Acquire the scope signal
                stopwatch.Restart();
                xtraViewScopeConnectionManager.StartAcquisition();
                stopwatch.Stop();

                Program.log.Info("Scope acquisition completed for the " + ScopeLibrary.Util.NumberToString.AddOrdinal(count) + " time");
                
                if (stopwatch.Elapsed < shortestRunTime)
                {
                    shortestRunTime = stopwatch.Elapsed;
                } 

                if (stopwatch.Elapsed > longestRunTime)
                {
                    longestRunTime = stopwatch.Elapsed;
                }

                totalRunTime += stopwatch.Elapsed;

                signalAnalyser.StartTime = xtraViewScopeConnectionManager.StartTime;
                signalAnalyser.Waveforms = xtraViewScopeConnectionManager.Waveforms;
                signalAnalyser.WaveformInfo = xtraViewScopeConnectionManager.WaveformInfo;

                //Analyse the acquired signal
                Program.signalAnalyserBackgroundWorker.RunWorkerAsync(signalAnalyser);

                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    Program.log.Info("Acquisition cancelled by user, stopping");
                    return;
                }
                //Generate the link on the interface in another thread that waits for the report to be created first.
                int passCount = count;
                Task.Factory.StartNew(() => backgroundWorker1.ReportProgress(passCount, new HeartbeatTiming(totalRunTime.TotalSeconds / passCount, shortestRunTime, longestRunTime)));
                count++;
            }
        }

        private void analyseSignalBackgroundWorker_doWork(object sender, DoWorkEventArgs e)
        {
            SignalAnalyser signalAnalyser = (SignalAnalyser)e.Argument;
            e.Result = signalAnalyser.analyseScopeSignal();
        }

        List<Task> reportWriterTasks = new List<Task>();

        private void analyseSignalBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SignalAnalysisResultContainer signalAnalysisResultContainer = (SignalAnalysisResultContainer)e.Result;

            foreach (IReportWriter reportWriter in Program.reportWriters)
            {
                reportWriter.OutputDirectory = OutputDirectory;
                reportWriter.FilePathString = FileNameFormat;

                reportWriter.Report.ReportContents.SignalAnalysisResultContainer = signalAnalysisResultContainer;

                reportWriterTasks.Add(Task.Factory.StartNew(reportWriter.WriteReport));
            }

            if (backgroundWorker1.IsBusy)
            {
                Task.Factory.StartNew(() => backgroundWorker1.ReportProgress(-1));
            }
        }
    
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 0)
            {
                HeartbeatTiming heartbeatTiming = (HeartbeatTiming)e.UserState;
                Program.log.Info("Average: " + Math.Round(heartbeatTiming.Average, 3) + " seconds");
                averageLabel.Text = "Average: " + Math.Round(heartbeatTiming.Average, 3) + " seconds";
                Program.log.Info("Shortest: " + Math.Round(heartbeatTiming.Shortest.TotalSeconds, 3) + " seconds");
                shortestLabel.Text = "Shortest: " + Math.Round(heartbeatTiming.Shortest.TotalSeconds, 3) + " seconds";
                Program.log.Info("Longest: " + Math.Round(heartbeatTiming.Longest.TotalSeconds, 3) + " seconds");
                longestLabel.Text = "Longest: " + Math.Round(heartbeatTiming.Longest.TotalSeconds, 3) + " seconds";

                startButton.Text = e.ProgressPercentage.ToString();
            }
            else
            {
                Task.WaitAll(reportWriterTasks.ToArray());

                progressReportLinkLabel.Text = "";
                progressReportLinkLabel.Links.Clear();
                int currentLinkPosition = 0;

                foreach (IReportWriter reportWriter in Program.reportWriters)
                {
                    Program.log.Info("Wrote to: " + reportWriter.FullFilePath);
                    if (progressReportLinkLabel.Text.Length > 0)
                    {
                        progressReportLinkLabel.Text += Environment.NewLine;
                        currentLinkPosition += Environment.NewLine.Length;
                    }

                    progressReportLinkLabel.Text += reportWriter.FullFilePath;
                    progressReportLinkLabel.Links.Add(currentLinkPosition, reportWriter.FullFilePath.Length, reportWriter.FullFilePath);
                    currentLinkPosition += reportWriter.FullFilePath.Length;
                }
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error. " + (e.Error as Exception).ToString());
                Program.log.Fatal((e.Error as Exception).ToString(), e.Error as Exception);
            }

            pictureBox1.Image = null;
            startButton.Text = "Start";
            ChangeControlState(true);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                pictureBox1.Image = Properties.Resources.Stopping;
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

            Program.configManager.ConfigFilePath = Path.GetFullPath(ConfigFilePath);
            Program.configManager.loadConfigDocument();

            //Set the file name to be <device name>_<channel_number>
            string deviceAndChannel = "";
            if (Program.configManager.getProperty("ResourceName") != null)
            {
                deviceAndChannel += Program.configManager.getProperty("ResourceName");
            }
            else
            {
                deviceAndChannel += "Dev1";
            }

            deviceAndChannel += "_";
            if (Program.configManager.getProperty("ChannelName") != null)
            {
                deviceAndChannel += Program.configManager.getProperty("ChannelName");
            }
            else
            {
                deviceAndChannel += "0";
            }

            FileNameFormat = deviceAndChannel;
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
            set
            {
                fileNameFormat.Text = value;
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
            LinkLabel lnk = new LinkLabel();
            lnk = (LinkLabel)sender;
            lnk.Links[lnk.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }

    public class HeartbeatTiming
    {
        public double Average { get; set; }
        public TimeSpan Shortest { get; set; }
        public TimeSpan Longest { get; set; }
        public HeartbeatTiming(double average, TimeSpan shortest, TimeSpan longest)
        {
            Average = average;
            Shortest = shortest;
            Longest = longest;
        }
    }
}
