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
        public static Report report;
        public static ReportContents reportContents;
        public static IReportWriter reportWriter = new ReportWriter();

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
