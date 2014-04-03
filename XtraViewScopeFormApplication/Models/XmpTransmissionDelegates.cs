using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtraViewScopeFormApplication.Models
{
    public class XmpTransmissionDelegates
    {
        public delegate void HeartbeatAnalysedEventHandler(XmpTransmissionEventArgs e);
        public static event HeartbeatAnalysedEventHandler HeartbeatAnalysedEvent;

        public static void raiseHearbeatAnalysed(SignalAnalysisResultContainer signalAnalysisResultContainer)
        {
            if (XmpTransmissionDelegates.HeartbeatAnalysedEvent != null)
            {
                HeartbeatAnalysedEvent(new XmpTransmissionEventArgs(signalAnalysisResultContainer));
            }
        } 

        public delegate void IrInboundAnalysedEventHandler(XmpTransmissionEventArgs e);
        public static event IrInboundAnalysedEventHandler IrInboundAnalysedEvent;

        public static void raiseIrInboundAnalysed(SignalAnalysisResultContainer signalAnalysisResultContainer)
        {
            if (XmpTransmissionDelegates.IrInboundAnalysedEvent != null)
            {
                IrInboundAnalysedEvent(new XmpTransmissionEventArgs(signalAnalysisResultContainer));
            }
        }   
    }

    public class XmpTransmissionEventArgs : EventArgs
    {
        public XmpTransmissionEventArgs(SignalAnalysisResultContainer signalAnalysisResultContainer)
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
