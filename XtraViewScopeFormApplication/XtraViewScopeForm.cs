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
            startButton.Text = "0";
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
            TimeSpan totalRunTime = new TimeSpan();
            TimeSpan shortestRunTime = TimeSpan.MaxValue;
            TimeSpan longestRunTime = TimeSpan.MinValue;
            Stopwatch stopwatch = new Stopwatch();

            IScopeConnectionManager xtraViewScopeConnectionManager = new XtraViewScopeConnectionManager();

            //The first time the hearbeat is sensed can be 0.0 < currentTime < 30.0 seconds. 
            //This data disrupts the timing captured below, so make sure that it isn't included by ignoring the first heartbeat
            xtraViewScopeConnectionManager.StartAcquisition();

            int count = 1;
            while (true)
            {
                //Acquire the scope signal
                stopwatch.Restart();
                xtraViewScopeConnectionManager.StartAcquisition();
                Program.log.Info("Scope acquisition completed for the " + ScopeLibrary.Util.NumberToString.AddOrdinal(count) + " time");

                stopwatch.Stop();
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
                int passCount = count;
                Thread reportProgressThread = new Thread(() => backgroundWorker1.ReportProgress(passCount, new HeartbeatTiming(totalRunTime.TotalSeconds / passCount, shortestRunTime, longestRunTime)));
                reportProgressThread.Start();
                count++;
            }
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            HeartbeatTiming heartbeatTiming = (HeartbeatTiming) e.UserState;
            Program.log.Info("Average: " + Math.Round(heartbeatTiming.Average, 3) + " seconds");
            averageLabel.Text = "Average: " + Math.Round(heartbeatTiming.Average, 3) + " seconds";
            Program.log.Info("Shortest: " + Math.Round(heartbeatTiming.Shortest.TotalSeconds, 3) + " seconds");
            shortestLabel.Text =  "Shortest: " + Math.Round(heartbeatTiming.Shortest.TotalSeconds, 3) + " seconds";
            Program.log.Info("Longest: " + Math.Round(heartbeatTiming.Longest.TotalSeconds, 3) + " seconds");
            longestLabel.Text = "Longest: " + Math.Round(heartbeatTiming.Longest.TotalSeconds, 3) + " seconds";

            //Wait for the report writer to finish so that the name is generated correctly
            reportWriterThread.Join();
            Program.log.Info("Wrote to: " + Program.reportWriter.FullFilePath);
            progressReportLinkLabel.Text = "Wrote to: " + Program.reportWriter.FullFilePath;
            startButton.Text = e.ProgressPercentage.ToString();
            progressReportLinkLabel.Links.Clear();
            progressReportLinkLabel.Links.Add(new LinkLabel.Link("Wrote to: ".Length, Program.reportWriter.FullFilePath.Length));
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                reportWriterThread.Join();
                Program.log.Info("Wrote to: " + Program.reportWriter.FullFilePath);
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
            startButton.Text = "Start";
            ChangeControlState(true);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                pictureBox1.Image = Properties.Resources.Stopping;
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
