using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XtraViewScope.Models.Enums;

namespace XtraViewScope.Models
{
    /// <summary>
    /// The XtraView heartbeat is composed of 8 ADD2 packets.
    /// </summary>
    public class Heartbeat
    {
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

        private string heartbeatStatus;
        public string HeartbeatStatus
        {
            get
            {
                foreach (Add2Packet add2Packet in Add2Packets)
                {
                    if (add2Packet.Status.Count > 0)
                    {
                        heartbeatStatus = Enums.HeartbeatStatus.FailedAdd2Packets;
                    }
                    foreach (Nibble nibble in add2Packet.Nibbles)
                    {
                        if (nibble.NibbleStatus != null)
                        {
                            heartbeatStatus = Enums.HeartbeatStatus.FailedNibbles;
                        }
                    }
                }
                if (heartbeatStatus == null || heartbeatStatus.Equals(""))
                {
                    heartbeatStatus = Enums.HeartbeatStatus.Passed;
                }
                return heartbeatStatus;
            }
            set
            {
                heartbeatStatus = value;
            }
        }
    }
}
