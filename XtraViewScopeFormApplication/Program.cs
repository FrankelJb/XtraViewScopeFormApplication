using log4net;
using ScopeLibrary;
using ScopeLibrary.ConnectionManagement;
using ScopeLibrary.ReportWriting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Forms;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace XtraViewScopeFormApplication
{
    public static class Program
    {

        static readonly object reportWriterLock = new object();
        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IConfigManager configManager;
        public static IConfigManager keyPressConfigManager;
        public static List<IReportWriter> reportWriters;

        //This blocking collection is used to queue acquired signal to be analysed
        public static BlockingCollection<IScopeConnectionManager> scopeConnectionBlockingCollection = new BlockingCollection<IScopeConnectionManager>();
        public static bool runConnectionManagerConsumer = true;

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
