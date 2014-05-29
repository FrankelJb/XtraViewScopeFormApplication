using ScopeLibrary;
using ScopeLibrary.ReportWriting;
using ScopeLibrary.SignalAnalysis;
using ScopeLibrary.Util;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XtraViewScopeFormApplication.Models.MnecTransmission;
using XtraViewScopeFormApplication.Models.XmpTransmission;

namespace XtraViewScopeFormApplication.ScopeAnalysis
{
    public class SignalAnalysisResultConsumer
    {
        private XtraViewScopeForm XtraViewScopeForm { get; set; }
        public SignalAnalysisResultConsumer(XtraViewScopeForm xtraViewScopeForm)
        {
            XtraViewScopeForm = xtraViewScopeForm;

            TransmissionDelegates.IrInboundAnalysedEvent += new TransmissionDelegates.IrInboundAnalysedEventHandler(consumeIrInbound);
            TransmissionDelegates.HeartbeatAnalysedEvent += new TransmissionDelegates.HeartbeatAnalysedEventHandler(consumeHeartbeat);
        }

        private HeartbeatTiming heartbeatTiming;
        private HeartbeatTiming HeartbeatTiming {
            get
            {
                return heartbeatTiming;
            }
            set
            {
                heartbeatTiming = value;
            }
        }

        public void clearHeartBeatTiming()
        {
            HeartbeatTiming = new HeartbeatTiming();
            HeartbeatTiming.LatestHeartbeatDateTime = null;
            HeartbeatTiming.Shortest = null;
            HeartbeatTiming.Longest = null;
        }

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

        public void consumeIrInbound(TransmissionEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            ISignalAnalysisResult signalAnalysisResult = e.SignalAnalysisResultContainer.SignalAnalysisResult;

            foreach (AbstractIrPacket abstractIrPacket in signalAnalysisResult.PacketTransmission.IrPackets)
            {

                MnecPacket mnecPacket = abstractIrPacket as MnecPacket;
                if (mnecPacket != null)
                {
                    sb.Append(BinaryConverter.BinaryToHexString(BinaryConverter.AbstractInformationUnitCollectionToBitArray(mnecPacket.InformationUnits))); ;
                }
                else 
                {
                    foreach (AbstractInformationUnit abstractInfromationUnit in abstractIrPacket.InformationUnits)
                    {
                        sb.Append(String.Format("{0:X}", abstractInfromationUnit.DecimalValue));
                    }
                }
            }

            if (XtraViewScopeForm.uiBackgroundWorker.IsBusy)
            {
                XtraViewScopeForm.uiBackgroundWorker.ReportProgress(-2, sb.ToString());
            }

            if (XtraViewScopeForm.shouldSaveKeyPresses.Checked)
            {
                WriteHexValuesToFile(sb.Append(Environment.NewLine));
            }

            Program.log.Info("IR button pressed, hex = " + sb.ToString());
        }

        private void WriteHexValuesToFile(StringBuilder sb)
        {
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }

            FileInfo filePath = new FileInfo(Path.Combine(OutputDirectory, @"hexKeyPresses.txt"));
            if (!filePath.Exists)
            {
                filePath.Create().Close();
            }
            else
            {
                //File.WriteAllText(filePath.ToString(), String.Empty);
            }

            File.AppendAllText(filePath.ToString(), sb.ToString());
        }

        public void consumeHeartbeat(TransmissionEventArgs e)
        {
            ISignalAnalysisResult signalAnalysisResult = e.SignalAnalysisResultContainer.SignalAnalysisResult;

            //Heartbeat heartbeat = add2SignalAnalysisResult.PacketTransmission as Heartbeat;
            HeartbeatTiming.LatestHeartbeatDateTime = signalAnalysisResult.PacketTransmission.TimeCaptured;// heartbeat.TimeCaptured;

            foreach (IReportWriter reportWriter in Program.reportWriters)
            {
                reportWriter.Report.ReportContents.SignalAnalysisResultContainer = e.SignalAnalysisResultContainer;

                reportWriter.FilePathString = FileNameFormat + "_heartbeat";
                reportWriter.OutputDirectory = OutputDirectory + "\\heartbeat";

                Task.Factory.StartNew(reportWriter.WriteReport);
            }

            if (XtraViewScopeForm.uiBackgroundWorker.IsBusy)
            {
                XtraViewScopeForm.uiBackgroundWorker.ReportProgress(-1, HeartbeatTiming);
            }
        }
    }
}
