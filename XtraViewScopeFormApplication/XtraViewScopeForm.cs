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
using XtraViewScopeFormApplication.ConnectionManagement;
using XtraViewScopeFormApplication.Models.XmpTransmission;
using XtraViewScopeFormApplication.ReportWriting;
using XtraViewScopeFormApplication.ScopeAnalysis;

namespace XtraViewScopeFormApplication
{
    public partial class XtraViewScopeForm : Form
    {
        SignalAnalysisResultConsumer signalAnalysisResultConsumer;

        public XtraViewScopeForm()
        {
            InitializeComponent();
            Program.log.Info("Form initialised");

            initialiseScopeConfigurationComponenents();
            Program.log.Info("Scope configuration components initialised");

            //Instantiate the analysed signal consumer with a reference to the form so that the results of analysed signal can be written to the gui
            signalAnalysisResultConsumer = new SignalAnalysisResultConsumer(this);
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

            //Instantiate the keyPressConfigManager
            Program.keyPressConfigManager = new PropertyConfigManager();

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

        }

        private int heartbeatCount = 0;
        private void startButton_Click(object sender, EventArgs e)
        {
            heartbeatCount = 0;
            startButton.Text = "0";
            pictureBox1.Image = Properties.Resources.Busy;

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

            //Load the hex values of the key presseses into a dictionary
            Program.keyPressConfigManager.ConfigFilePath = @"..\..\Resources\irKeyMapping.properties";
            Program.keyPressConfigManager.loadConfigDocument();

            //Load the config file and set the default save file format
            Program.configManager.ConfigFilePath = Path.GetFullPath(ConfigFilePath);
            Program.configManager.loadConfigDocument();

            ChangeControlState(false);

            //We need new reportWriters each time the acquisition is started based on the configs
            Program.reportWriters = new List<IReportWriter>();


            //Set the report content type
            if (Program.configManager.getProperty("ReportContents") != null)
            {
                if (Program.configManager.getProperty("ReportContents").Contains("Xml"))
                {
                    IReportWriter reportWriter = new ReportWriter();
                    reportWriter.Report = new Report();
                    reportWriter.Report.ReportContents = new XmlReportContents();
                    Program.reportWriters.Add(reportWriter);
                }
                if (Program.configManager.getProperty("ReportContents").Contains("Json"))
                {
                    IReportWriter reportWriter = new ReportWriter();
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
            uiBackgroundWorker.RunWorkerAsync();  
        }

        IScopeConnectionManager xtraViewScopeConnectionManager;
        //This thread is called when the user starts the application.
        //It is invoked using a background worker so that the UI does not appear to freeze and the user can stop the application.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            SignalAnalyser signalAnalyser = new Add2SignalAnalyser();
            
            //Start the connection manager consumer
            ScopeConnectionManagerConsumer scopeConnectionManagerConsumer = new ScopeConnectionManagerConsumer();
            Program.runConnectionManagerConsumer = true;
            Task.Factory.StartNew(scopeConnectionManagerConsumer.consumeConnectionManager);

            //Set the signal analysis heartbeat to be an empty timing object
            signalAnalysisResultConsumer.clearHeartBeatTiming();
            signalAnalysisResultConsumer.OutputDirectory = OutputDirectory;
            signalAnalysisResultConsumer.FileNameFormat = FileNameFormat;

            int count = 1;
            while (true)
            {
                xtraViewScopeConnectionManager = new XtraViewScopeConnectionManager();

                //Acquire the scope signal
                xtraViewScopeConnectionManager.StartAcquisition();
                Program.scopeConnectionBlockingCollection.Add(xtraViewScopeConnectionManager);

                Program.log.Info("Scope acquisition completed for the " + ScopeLibrary.Util.NumberToString.AddOrdinal(count) + " time");

                signalAnalyser.StartTime = xtraViewScopeConnectionManager.StartTime;
                signalAnalyser.Waveforms = xtraViewScopeConnectionManager.Waveforms;
                signalAnalyser.WaveformInfo = xtraViewScopeConnectionManager.WaveformInfo;

                if (uiBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    Program.log.Info("Acquisition cancelled by user, stopping");
                    Program.runConnectionManagerConsumer = false;
                    return;
                }

                count++;
            }
        }

        private void uiBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                startButton.Text = heartbeatCount++.ToString();
                HeartbeatTiming hearbeatTiming = (HeartbeatTiming)e.UserState;
                averageLabel.Text = "Average: " + Math.Round(hearbeatTiming.Average, 3);
                shortestLabel.Text = "Shortest: " + Math.Round(hearbeatTiming.Shortest.Value.TotalSeconds, 3);
                longestLabel.Text = "Longest: " + Math.Round(hearbeatTiming.Longest.Value.TotalSeconds, 3);
            }
            else if (e.ProgressPercentage == -2)
            {
                string keyPress = (string)e.UserState;
                if(irKeyPresses.Text.Length > 0)
                {
                    irKeyPresses.AppendText(Environment.NewLine);
                }

                if (Program.keyPressConfigManager.getProperty(keyPress) != null)
                {
                    irKeyPresses.AppendText(Program.keyPressConfigManager.getProperty(keyPress));
                }
                else
                {
                    irKeyPresses.AppendText(keyPress);//TODO: Change this to "unknown"
                }
            }
        }

        private void uiBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            if (uiBackgroundWorker.IsBusy)
            {
                pictureBox1.Image = Properties.Resources.Stopping;
            }
            uiBackgroundWorker.CancelAsync();
            xtraViewScopeConnectionManager.CloseSession();
        }


        private void clearKeyPresses_Click(object sender, EventArgs e)
        {
            irKeyPresses.Clear();
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
            shouldSaveKeyPresses.Enabled = isEnabled;
            this.Refresh();
        }

        private void graphHeartbeats_Click(object sender, EventArgs e)
        {
            Process p = new Process(); // create process (i.e., the python program
            p.StartInfo.FileName = @"C:\Python27\python.exe";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false; // make sure we can read the output from stdout
            p.StartInfo.Arguments = @"C:\Projects\Python\wxPython.py"; // start the python program with two parameters
            p.Start();
        }
    }
}
