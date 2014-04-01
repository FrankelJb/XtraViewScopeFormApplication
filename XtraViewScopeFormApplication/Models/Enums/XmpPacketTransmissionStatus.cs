using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtraViewScopeFormApplication.Models.Enums
{
    public struct XmpPacketTransmissionStatus
    {
        public const string Passed = "XMP Transmission was normal";
        public const string FailedAdd2Packets = "XMP Transmission had malformed add2 packets";
        public const string FailedNibbles = "XMP Transmission had malformed nibbles";
        //TODO: correct failed status
    }
}
