using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtraViewScope.Models.Enums
{
    public struct HeartbeatStatus
    {
        public const string Passed = "";
        public const string Failed1 = "Nibble duration was not within any ranges defined in Add2 Table";
        public const string Failed2 = "Heartbeat had malformed add2 packets";
        public const string Failed3 = "Heartbeat had malformed nibbles";
        //TODO: correct failed status
    }
}
