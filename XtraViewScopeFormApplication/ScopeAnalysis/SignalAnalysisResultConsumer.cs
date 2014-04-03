using NationalInstruments;
using ScopeLibrary.ReportWriting;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XtraViewScope.Models;
using XtraViewScope.ScopeAnalysis;
using XtraViewScopeFormApplication.Models;
using XtraViewScopeFormApplication.Models.Enums;
using XtraViewScopeFormApplication.ScopeAnalysis;

namespace XtraViewScopeFormApplication.ReportWriting
{
    public class SignalAnalysisResultConsumer
    {
        private XtraViewScopeForm XtraViewScopeForm { get; set; }
        public SignalAnalysisResultConsumer(XtraViewScopeForm xtraViewScopeForm)
        {
            XtraViewScopeForm = xtraViewScopeForm;

            XmpTransmissionDelegates.IrInboundAnalysedEvent += new XmpTransmissionDelegates.IrInboundAnalysedEventHandler(consumeIrInbound);
            XmpTransmissionDelegates.HeartbeatAnalysedEvent += new XmpTransmissionDelegates.HeartbeatAnalysedEventHandler(consumeHeartbeat);
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

        public void consumeIrInbound(XmpTransmissionEventArgs e)
        {
            Add2SignalAnalysisResult add2SignalAnalysisResult = e.SignalAnalysisResultContainer.SignalAnalysisResult as Add2SignalAnalysisResult;

            StringBuilder sb = new StringBuilder();
            IrInbound irInbound = add2SignalAnalysisResult.XmpPacketTransmission as IrInbound;

            foreach (Add2Packet add2Packet in irInbound.Add2Packets)
            {
                foreach (Nibble nibble in add2Packet.Nibbles)
                {
                    sb.Append(String.Format("{0:X}", nibble.DecimalValue));
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

        public void consumeHeartbeat(XmpTransmissionEventArgs e)
        {
            Add2SignalAnalysisResult add2SignalAnalysisResult = e.SignalAnalysisResultContainer.SignalAnalysisResult as Add2SignalAnalysisResult;

            Heartbeat heartbeat = add2SignalAnalysisResult.XmpPacketTransmission as Heartbeat;
            HeartbeatTiming.LatestHeartbeatDateTime = heartbeat.TimeCaptured;

            foreach (IReportWriter reportWriter in Program.reportWriters)
            {
                reportWriter.Report.ReportContents.SignalAnalysisResultContainer = e.SignalAnalysisResultContainer;

                reportWriter.FilePathString = FileNameFormat + "_heartbeat";
                reportWriter.OutputDirectory = OutputDirectory + "\\heartbeat";

                Task.Factory.StartNew(reportWriter.WriteReport);
            }

            XtraViewScopeForm.uiBackgroundWorker.ReportProgress(-1, HeartbeatTiming);
        }
    }

    public class HeartbeatTiming
    {
        public int numberOfHeartbeats { get; set; }
        public PrecisionTimeSpan? Total { get; set; }
        public double Average { get; set; }
        public PrecisionTimeSpan? Shortest { get; set; }
        public PrecisionTimeSpan? Longest { get; set; }
        private PrecisionDateTime? latestHeartbeatDateTime;
        public PrecisionDateTime? LatestHeartbeatDateTime
        {
            get
            {
                return latestHeartbeatDateTime;
            }
            set
            {
                if (latestHeartbeatDateTime == null)
                {
                    latestHeartbeatDateTime = value;
                    Shortest = PrecisionTimeSpan.MaxValue;
                    Longest = PrecisionTimeSpan.MinValue;
                    Total = new PrecisionTimeSpan();
                }
                else
                {
                    if (Shortest.Value > value - latestHeartbeatDateTime.Value)
                    {
                        Shortest = value - latestHeartbeatDateTime.Value;
                    }
                    if (Longest.Value < value - latestHeartbeatDateTime.Value)
                    {
                        Longest = value - latestHeartbeatDateTime.Value;
                    }

                    Total += value - latestHeartbeatDateTime;
                    numberOfHeartbeats++;
                    latestHeartbeatDateTime = value;
                    Average = Total.Value.TotalSeconds / numberOfHeartbeats;
                    
                }
            }
        }
    }
}
