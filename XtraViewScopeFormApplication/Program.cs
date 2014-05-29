using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
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
