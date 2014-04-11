using NationalInstruments;
using System.Collections.ObjectModel;
using XtraViewScopeFormApplication.Models.Enums;

namespace XtraViewScopeFormApplication.Models.MnecTransmission
{
    public abstract class MnecPacketTransmission
    {
        private string mnecPacketTransmissionStatus;
        public string MnecPacketTransmissionStatus
        {
            get
            {
                return mnecPacketTransmissionStatus;
            }
        }

        private Collection<MnecPacket> mnecPackets;
        public Collection<MnecPacket> MnecPackets
        {
            get
            {
                return mnecPackets;
            }
            set
            {
                mnecPackets = value;
            }
        }

        private PrecisionDateTime timeCaptured;
        public PrecisionDateTime TimeCaptured
        {
            get
            {
                return timeCaptured;
            }
            set
            {
                timeCaptured = value;
            }
        }
    }
}
