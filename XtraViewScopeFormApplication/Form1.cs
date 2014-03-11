using ScopeLibrary.ConfigManagement;
using ScopeLibrary.ConnectionManagement;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XtraViewScope.ConnectionManagement;
using XtraViewScope.ScopeAnalysis;

namespace XtraViewScopeFormApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.configManager = new XmlConfigManager();
            Program.configManager.ConfigFilePath = Path.Combine(Application.StartupPath, @"\Resources\XMLFile1.xml");
            Program.configManager.loadConfigDocument();

            XtraViewScopeConnectionManager xtraViewScopeConnectionManager = new XtraViewScopeConnectionManager();
            xtraViewScopeConnectionManager.StartAcquisition();

            SignalAnalyser signalAnalyser = new Add2SignalAnalyser();
            signalAnalyser.StartTime = xtraViewScopeConnectionManager.StartTime;
            signalAnalyser.Waveforms = xtraViewScopeConnectionManager.Waveforms;
            signalAnalyser.WaveformInfo = xtraViewScopeConnectionManager.WaveformInfo;

            Program.reportContents.SignalAnalysisResultContainer = signalAnalyser.analyseScopeSignal();
            Program.report.ReportContents = Program.reportContents;

            Program.reportWriter.WriteReport(Program.report);
        }
    }
}
