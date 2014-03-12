using ScopeLibrary.ReportWriting;
using System;
using System.IO;
using XtraViewScopeFormApplication;

namespace XtraViewScope.ReportWriting
{
    public class ReportWriter : IReportWriter
    {
        private Report report;
        public Report Report
        {
            get
            {
                return report;
            }
            set
            {
                report = value;
            }
        }

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

        public string FullFilePath
        {
            get
            {
                return Path.Combine(outputDirectory, filePathString);
            }
        }

        public void WriteReport()
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
            if (Report.ReportContents is JsonReportContents)
            {
                filetype = "json";
            }
            else if (Report.ReportContents is XmlReportContents)
            {
                filetype = "xml";
            }

            FilePathString += "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + filetype;

            FileInfo filePath = new FileInfo(Path.Combine(outputDirectory, FilePathString));
            if (!filePath.Exists)
            {
                filePath.Create().Close();
            }

            File.WriteAllText(filePath.ToString(), Report.ReportContents.ToString());
        }
    }
}
