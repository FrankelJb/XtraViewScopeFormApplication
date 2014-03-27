using ScopeLibrary;
using ScopeLibrary.ReportWriting;
using System;
using System.Windows.Forms;
using XtraViewScope.ReportWriting;
using log4net;
using System.Collections.Generic;
using System.Threading;
using ScopeLibrary.SignalAnalysis;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace XtraViewScopeFormApplication
{
    public static class Program
    {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly ILog log = LogManager.GetLogger(typeof (Program)) ;

        public static IConfigManager configManager;
        public static List<IReportWriter> reportWriters;
        public static SignalAnalysisResultContainer analysedSignal;
        public static Thread signalAnalyserThread;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log.Info("Application started");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new XtraViewScopeForm());
        }
    }
}
