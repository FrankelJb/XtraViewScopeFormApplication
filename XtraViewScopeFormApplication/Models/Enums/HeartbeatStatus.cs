using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtraViewScope.Models.Enums
{
    public struct HeartbeatStatus
    {
        public const string Passed = "Hearbeat was normal";
        public const string FailedAdd2Packets = "Heartbeat had malformed add2 packets";
        public const string FailedNibbles = "Heartbeat had malformed nibbles";
        //TODO: correct failed status
    }
}
