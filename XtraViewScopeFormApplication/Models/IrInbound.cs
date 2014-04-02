using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XtraViewScopeFormApplication.Models.Enums;
using XtraViewScopeFormApplication.Models.XmpPackets;

namespace XtraViewScopeFormApplication.ScopeAnalysis
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
