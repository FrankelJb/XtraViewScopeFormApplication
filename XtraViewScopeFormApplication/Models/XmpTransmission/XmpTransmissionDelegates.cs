using ScopeLibrary.SignalAnalysis;
using System;

namespace XtraViewScopeFormApplication.Models.XmpTransmission
{
    public class TransmissionDelegates
    {
        public delegate void HeartbeatAnalysedEventHandler(TransmissionEventArgs e);
        public static event HeartbeatAnalysedEventHandler HeartbeatAnalysedEvent;

        public static void raiseHearbeatAnalysed(SignalAnalysisResultContainer signalAnalysisResultContainer)
        {
            if (TransmissionDelegates.HeartbeatAnalysedEvent != null)
            {
                HeartbeatAnalysedEvent(new TransmissionEventArgs(signalAnalysisResultContainer));
            }
        } 

        public delegate void IrInboundAnalysedEventHandler(TransmissionEventArgs e);
        public static event IrInboundAnalysedEventHandler IrInboundAnalysedEvent;

        public static void raiseIrInboundAnalysed(SignalAnalysisResultContainer signalAnalysisResultContainer)
        {
            if (TransmissionDelegates.IrInboundAnalysedEvent != null)
            {
                IrInboundAnalysedEvent(new TransmissionEventArgs(signalAnalysisResultContainer));
            }
        }   
    }

    public class TransmissionEventArgs : EventArgs
    {
        public TransmissionEventArgs(SignalAnalysisResultContainer signalAnalysisResultContainer)
        {
            this.signalAnalysisResultContainer = signalAnalysisResultContainer;
        }

        private SignalAnalysisResultContainer signalAnalysisResultContainer;
        public SignalAnalysisResultContainer SignalAnalysisResultContainer
        {
            get
            {
                return signalAnalysisResultContainer;
            }
        }
    }
}
