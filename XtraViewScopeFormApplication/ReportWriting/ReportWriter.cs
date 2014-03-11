using ScopeLibrary.ReportWriting;
using System;
using System.IO;
using XtraViewScopeFormApplication;

namespace XtraViewScope.ReportWriting
{
    public class ReportWriter : IReportWriter
    {
        public void WriteReport(Report report)
        {
            string directoryPath = @"C:\waveform";

            if(Program.configManager.getProperty("DirectoryPath") != null)
            {
                directoryPath = Program.configManager.getProperty("DirectoryPath");
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePathString = @"Scope_Analysis_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";

            FileInfo filePath = new FileInfo(Path.Combine(directoryPath, filePathString));
            if (!filePath.Exists)
            {
                filePath.Create().Close();
            }

            File.WriteAllText(filePath.ToString(), report.ReportContents.ToString());
        }
    }
}
