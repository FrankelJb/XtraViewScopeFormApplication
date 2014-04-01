using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XtraViewScope.Models;
using XtraViewScopeFormApplication.Models.XmpPackets;
using XtraViewScopeFormApplication.ScopeAnalysis;

namespace XtraViewScope.ScopeAnalysis
{
    public class Add2SignalAnalysisResult : ISignalAnalysisResult
    {

        private XmpPacketTransmission xmpPacketTransmission;
        public XmpPacketTransmission XmpPacketTransmission
        {
            get
            {
                return xmpPacketTransmission;
            }
            set
            {
                xmpPacketTransmission = value;
            }
        }
    }
}
