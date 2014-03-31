using ScopeLibrary;
using ScopeLibrary.ReportWriting;
using System;
using System.Windows.Forms;
using XtraViewScope.ReportWriting;
using log4net;
using System.Collections.Generic;
using System.Threading;
using ScopeLibrary.SignalAnalysis;
using System.ComponentModel;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace XtraViewScopeFormApplication
{
    public static class Program
    {

        static readonly object reportWriterLock = new object();
        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly ILog log = LogManager.GetLogger(typeof (Program)) ;

        public static IConfigManager configManager;
        public static List<IReportWriter> reportWriters;
        public static BackgroundWorker signalAnalyserBackgroundWorker = new BackgroundWorker();
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
