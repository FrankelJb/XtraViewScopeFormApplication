using NationalInstruments;
using System.Collections.ObjectModel;
using XtraViewScopeFormApplication.Models.Enums;

namespace XtraViewScopeFormApplication.Models.XmpTransmission
{
    public abstract class XmpPacketTransmission
    {
        private string xmpPacketTransmissionStatus;
        public string XmpPacketTransmissionStatus
        {
            get
            {
                foreach (Add2Packet add2Packet in Add2Packets)
                {
                    if (add2Packet.Status.Count > 0)
                    {
                        xmpPacketTransmissionStatus = Enums.XmpPacketTransmissionStatus.FailedAdd2Packets;
                    }
                    foreach (Nibble nibble in add2Packet.Nibbles)
                    {
                        if (nibble.NibbleStatus != null)
                        {
                            xmpPacketTransmissionStatus = Enums.XmpPacketTransmissionStatus.FailedNibbles;
                        }
                    }
                }
                if (xmpPacketTransmissionStatus == null || xmpPacketTransmissionStatus.Equals(""))
                {
                    xmpPacketTransmissionStatus = Enums.XmpPacketTransmissionStatus.Passed;
                }
                return xmpPacketTransmissionStatus;
            }
        }

        public abstract XmpPacketTransmissionType XmpPacketTransmissionType
        {
            get;
        }

        private Collection<Add2Packet> add2Packets;
        public Collection<Add2Packet> Add2Packets
        {
            get
            {
                return add2Packets;
            }
            set
            {
                add2Packets = value;
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
