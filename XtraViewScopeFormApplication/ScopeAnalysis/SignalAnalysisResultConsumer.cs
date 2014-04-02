using NationalInstruments;
using ScopeLibrary.ReportWriting;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XtraViewScope.Models;
using XtraViewScope.ScopeAnalysis;
using XtraViewScopeFormApplication.Models.Enums;
using XtraViewScopeFormApplication.ScopeAnalysis;

namespace XtraViewScopeFormApplication.ReportWriting
{
    public class SignalAnalysisResultConsumer
    {
        private XtraViewScopeForm XtraViewScopeForm { get; set; }
        public SignalAnalysisResultConsumer(XtraViewScopeForm xtraViewScopeForm)
        {
            HeartbeatTiming = new HeartbeatTiming();
            HeartbeatTiming.LatestHeartbeatDateTime = null;
            HeartbeatTiming.Shortest = null;
            HeartbeatTiming.Longest = null;
            XtraViewScopeForm = xtraViewScopeForm;
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
                SignalAnalysisResultContainer signalAnalysisResultContainer = Program.signalAnalysisResultBlockingCollection.Take();
                Add2SignalAnalysisResult add2SignalAnalysisResult = signalAnalysisResultContainer.SignalAnalysisResult as Add2SignalAnalysisResult;

                if (add2SignalAnalysisResult.XmpPacketTransmission.XmpPacketTransmissionType == XmpPacketTransmissionType.Heartbeat)
                {
                    Heartbeat heartbeat = add2SignalAnalysisResult.XmpPacketTransmission as Heartbeat;

                    HeartbeatTiming.LatestHeartbeatDateTime = heartbeat.TimeCaptured;

                    System.Diagnostics.Debug.WriteLine(HeartbeatTiming.ToString());

                    XtraViewScopeForm.uiBackgroundWorker.ReportProgress(-1, HeartbeatTiming);

                    //foreach (SignalAnalysisResultContainer nextSignalAnalysisResultContainer in Program.signalAnalysisResultBlockingCollection)
                    //{
                    //    Add2SignalAnalysisResult nextAdd2SignalAnalysisResult = nextSignalAnalysisResultContainer.SignalAnalysisResult as Add2SignalAnalysisResult;
                    //}
                }
                else if (add2SignalAnalysisResult.XmpPacketTransmission.XmpPacketTransmissionType == XmpPacketTransmissionType.IrInbound)
                {
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
                }
                

                foreach (IReportWriter reportWriter in Program.reportWriters)
                {
                    reportWriter.Report.ReportContents.SignalAnalysisResultContainer = signalAnalysisResultContainer;

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

        public override string ToString()
        {
            return "Total " + Total.ToString() + Environment.NewLine +
                "latestHeartbeatDateTime " + latestHeartbeatDateTime.Value.ToLongTimeString() + Environment.NewLine +
                "average " + Average + Environment.NewLine +
                "shortest " + Shortest + Environment.NewLine +
                "longest " + Longest + Environment.NewLine
                ;
        }
    }
}
