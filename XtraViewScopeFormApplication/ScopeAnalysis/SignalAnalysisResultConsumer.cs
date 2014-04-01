using ScopeLibrary.ReportWriting;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XtraViewScope.ScopeAnalysis;
using XtraViewScopeFormApplication.Models.Enums;

namespace XtraViewScopeFormApplication.ReportWriting
{
    public class SignalAnalysisResultConsumer
    {
        private string outputDirectory;
        public string OutputDirectory { 
            get{
                return outputDirectory;
            } 
            set
            {
                outputDirectory = value;
            }
        }

        private string fileNameFormat;
        public string FileNameFormat
        {
            get
            {
                return fileNameFormat;
            }
            set
            {
                fileNameFormat = value;
            }
        }

        public void consumeSignalAnalysisResults()
        {
            while (Program.runSignalAnalysisResultConsumer)
            {
                SignalAnalysisResultContainer signalAnalysisResultContainer = Program.signalAnalysisResultBlockingQueue.Take();

                foreach (IReportWriter reportWriter in Program.reportWriters)
                {
                    reportWriter.Report.ReportContents.SignalAnalysisResultContainer = signalAnalysisResultContainer;

                    Add2SignalAnalysisResult add2SignalAnalysisResult = signalAnalysisResultContainer.SignalAnalysisResult as Add2SignalAnalysisResult;
                    if (add2SignalAnalysisResult.XmpPacketTransmission.XmpPacketTransmissionType == XmpPacketTransmissionType.Heartbeat)
                    {
                        reportWriter.FilePathString = FileNameFormat + "_heartbeat";
                        reportWriter.OutputDirectory = OutputDirectory + "\\heartbeat";
                    }
                    else if (add2SignalAnalysisResult.XmpPacketTransmission.XmpPacketTransmissionType == XmpPacketTransmissionType.IrInbound)
                    {
                        reportWriter.FilePathString = FileNameFormat + "_ir";
                        reportWriter.OutputDirectory = OutputDirectory + "\\ir";
                    }

                    Task.Factory.StartNew(reportWriter.WriteReport);
                }
            }
        }
    }
}
