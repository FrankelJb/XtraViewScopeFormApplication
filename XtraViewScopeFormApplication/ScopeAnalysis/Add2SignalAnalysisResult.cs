using ScopeLibrary.SignalAnalysis;
using XtraViewScopeFormApplication.Models.XmpTransmission;

namespace XtraViewScopeFormApplication.ScopeAnalysis
{
    public class Add2SignalAnalysisResult : ISignalAnalysisResult
    {
        private XmpPacketTransmission xmpPacketTransmission;
        public AbstractPacketTransmission PacketTransmission
        {
            get
            {
                return xmpPacketTransmission;
            }
            set
            {
                xmpPacketTransmission = value as XmpPacketTransmission;
            }
        }
    }
}
