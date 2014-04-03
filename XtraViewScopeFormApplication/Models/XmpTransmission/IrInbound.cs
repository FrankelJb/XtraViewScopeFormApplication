using XtraViewScopeFormApplication.Models.Enums;

namespace XtraViewScopeFormApplication.Models.XmpTransmission
{
    public class IrInbound : XmpPacketTransmission
    {
        public override XmpPacketTransmissionType XmpPacketTransmissionType
        {
            get
            {
                return XmpPacketTransmissionType.IrInbound;
            }
        }
    }
}
