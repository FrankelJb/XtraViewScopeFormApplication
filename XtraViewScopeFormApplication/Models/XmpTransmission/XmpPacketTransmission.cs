using NationalInstruments;
using ScopeLibrary;
using ScopeLibrary.SignalAnalysis;
using System.Collections.ObjectModel;
using XtraViewScopeFormApplication.Models.Enums;

namespace XtraViewScopeFormApplication.Models.XmpTransmission
{
    public abstract class XmpPacketTransmission : AbstractPacketTransmission
    {
        private string xmpPacketTransmissionStatus;
        public string XmpPacketTransmissionStatus
        {
            get
            {
                foreach (AbstractIrPacket irPacket in IrPackets)
                {
                    if (irPacket.Status.Count > 0)
                    {
                        xmpPacketTransmissionStatus = Enums.XmpPacketTransmissionStatus.FailedAdd2Packets;
                    }
                    foreach (AbstractInfromationUnit abstractInfromationUnit in irPacket.InformationUnits)
                    {
                        Nibble nibble = abstractInfromationUnit as Nibble;
                        if (nibble.Status != null)
                        {
                            xmpPacketTransmissionStatus = Enums.XmpPacketTransmissionStatus.FailedNibbles;
                        }
                    }
                }
                if (xmpPacketTransmissionStatus == null || xmpPacketTransmissionStatus.Equals(""))
                {
                    xmpPacketTransmissionStatus = Enums.XmpPacketTransmissionStatus.Passed;
                }
                return xmpPacketTransmissionStatus;
            }
        }

        public abstract XmpPacketTransmissionType XmpPacketTransmissionType
        {
            get;
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

        //private PrecisionDateTime timeCaptured;
        //public PrecisionDateTime TimeCaptured
        //{
        //    get
        //    {
        //        return timeCaptured;
        //    }
        //    set
        //    {
        //        timeCaptured = value;
        //    }
        //}
    }
}
