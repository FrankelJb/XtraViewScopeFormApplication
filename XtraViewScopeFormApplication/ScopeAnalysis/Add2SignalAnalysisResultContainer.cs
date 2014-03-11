using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XtraViewScope.Models;

namespace XtraViewScope.ScopeAnalysis
{
    public class Add2SignalAnalysisResult : ISignalAnalysisResult
    {
        private Heartbeat heartbeat;
        public Heartbeat Heartbeat
        {
            get
            {
                return heartbeat;
            }
            set
            {
                heartbeat = value;
            }
        }

        //private Collection<Add2Packet> add2Packets;
        //public Collection<Add2Packet> Add2Packets
        //{
        //    get
        //    {
        //        return add2Packets;
        //    }
        //    set
        //    {
        //        add2Packets = value;
        //    }
        //}
    }
}
