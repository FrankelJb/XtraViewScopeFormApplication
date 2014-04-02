using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XtraViewScope.Models.Enums;
using XtraViewScopeFormApplication.Models.Enums;
using XtraViewScopeFormApplication.Models.XmpPackets;

namespace XtraViewScope.Models
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
