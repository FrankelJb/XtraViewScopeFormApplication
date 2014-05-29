using ScopeLibrary.SignalAnalysis;
using XtraViewScopeFormApplication.Models.MnecTransmission;

namespace XtraViewScopeFormApplication.ScopeAnalysis
{
    public class MnecSignalAnalysisResult : ISignalAnalysisResult
    {
        private MnecPacketTransmission mnecPacketTransmission;
        public AbstractPacketTransmission PacketTransmission
        {
            get
            {
                return mnecPacketTransmission;
            }
            set
            {
                mnecPacketTransmission = value as MnecPacketTransmission;
            }
        }
    }
}
