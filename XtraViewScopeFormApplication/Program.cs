using ScopeLibrary;
using ScopeLibrary.ReportWriting;
using System;
using System.Windows.Forms;
using XtraViewScope.ReportWriting;

namespace XtraViewScopeFormApplication
{
    static class Program
    {
        public static IConfigManager configManager;
        public static IReportWriter reportWriter = new ReportWriter();
        public static object reportWriterLock = new Object();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new XtraViewScopeForm());
        }
    }
}
