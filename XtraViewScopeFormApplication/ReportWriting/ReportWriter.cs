using ScopeLibrary.ReportWriting;
using System;
using System.IO;
using XtraViewScopeFormApplication;

namespace XtraViewScope.ReportWriting
{
    public class ReportWriter : IReportWriter
    {
        private string outputDirectory;
        public string OutputDirectory
        {
            set
            {
                outputDirectory = value;
            }
        }

        private string filePathString;
        public string FilePathString
        {
            get
            {
                return filePathString;
            }
            set
            {
                filePathString = value;
            }
        }

        public void WriteReport(Report report)
        {
            if(Program.configManager.getProperty("DirectoryPath") != null)
            {
                outputDirectory = Program.configManager.getProperty("DirectoryPath");
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string filetype = "txt";
            if (report.ReportContents is JsonReportContents)
            {
                filetype = "json";
            }
            else if(report.ReportContents is XmlReportContents)
            {
                filetype = "xml";
            }

            filePathString += "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + filetype;

            FileInfo filePath = new FileInfo(Path.Combine(outputDirectory, filePathString));
            if (!filePath.Exists)
            {
                filePath.Create().Close();
            }

            File.WriteAllText(filePath.ToString(), report.ReportContents.ToString());
        }
    }
}
