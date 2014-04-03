using XtraViewScopeFormApplication.Models.Enums;

namespace XtraViewScopeFormApplication.Models.XmpTransmission
{
    /// <summary>
    /// The XtraView heartbeat is composed of 8 ADD2 packets.
    /// </summary>
    public class Heartbeat : XmpPacketTransmission
    {
        public override XmpPacketTransmissionType XmpPacketTransmissionType
        {
            get 
            {
                return XmpPacketTransmissionType.Heartbeat;
            }
        }
    }
}
