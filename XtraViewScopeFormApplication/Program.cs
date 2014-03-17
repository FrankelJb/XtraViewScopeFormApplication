using ScopeLibrary;
using ScopeLibrary.ReportWriting;
using System;
using System.Windows.Forms;
using XtraViewScope.ReportWriting;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace XtraViewScopeFormApplication
{
    public static class Program
    {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly ILog log = LogManager.GetLogger(typeof (Program)) ;

        public static IConfigManager configManager;
        public static IReportWriter reportWriter = new ReportWriter();
        public static object reportWriterLock = new Object();
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
