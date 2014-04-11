using ScopeLibrary;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XtraViewScopeFormApplication.Models.XmpTransmission;

namespace XtraViewScopeFormApplication.Models.MnecTransmission
{
    /// <summary>
    //TODO: summary
    /// </summary>
    public class MnecPacket : AbstractIrPacket
    {
        public MnecPacket(Collection<AbstractInfromationUnit> informationUnits)
        {
            InformationUnits = informationUnits;
        }

        public override IrType IrType
        {
            get
            {
                return IrType.Mnec;
            }
        }

        //TODO: Fix this checksum
        /// <summary>
        /// Returns the calculation of the checksum. The sum of the nibbles mod 16 should be zero.
        /// </summary>
        /// <exception cref="MalformedXmpPacketException">If the checksum doesn't equal zero then there is a problem with this XMP Packet</exception>
        public override int Checksum
        {
            get
            {
                int checksum = 0;
                if (InformationUnits != null && InformationUnits.Count > 1)
                {
                    if (InformationUnits[1].IsNotValid)
                    {
                        Status.Add("The nibble containing the checksum has either not been captured or the ADD2 Packet is malformed");
                        return -1;
                    }

                    foreach (AbstractInfromationUnit abstractInformationUnit in InformationUnits)
                    {
                        checksum += abstractInformationUnit.DecimalValue;
                    }
                    if (checksum % 16 != 0)
                    {
                        Status.Add("The nibble containing the checksum has either not been captured or the ADD2 Packet is malformed");
                        return checksum;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    Status.Add("The nibble containing the checksum has either not been captured or the ADD2 Packet is malformed");
                    return -1;
                }
            }
        }
    }
}
