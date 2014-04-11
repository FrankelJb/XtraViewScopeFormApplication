using ScopeLibrary.SignalAnalysis;
using XtraViewScopeFormApplication.Models.XmpTransmission;

namespace XtraViewScopeFormApplication.ScopeAnalysis
{
    public class Add2SignalAnalysisResult : ISignalAnalysisResult
    {
        private XmpPacketTransmission xmpPacketTransmission;
        public XmpPacketTransmission XmpPacketTransmission
        {
            get
            {
                return xmpPacketTransmission;
            }
            set
            {
                xmpPacketTransmission = value;
            }
        }
    }
}
