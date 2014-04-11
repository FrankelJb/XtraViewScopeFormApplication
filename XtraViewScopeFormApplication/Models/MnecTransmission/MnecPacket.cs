using ScopeLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XtraViewScopeFormApplication.Models.Enums;
using XtraViewScopeFormApplication.Models.XmpTransmission;

namespace XtraViewScopeFormApplication.Models.MnecTransmission
{
    /// <summary>
    //TODO: summary
    /// </summary>
    public class MnecPacket : IrPacket
    {

        public MnecPacket(Collection<Nibble> nibbles)
        {
            Nibbles = nibbles;
        }

        //TODO: set up the heartbeat requirements

        /// <summary>
        /// The binary value is obtained by concatenating the binary value of all of the 8 nibbles.
        /// </summary>
        public string BinaryValue
        {
            get
            {
                string binaryValue = "";
                if (Nibbles != null)
                {
                    foreach (Nibble nibble in Nibbles)
                    {
                        binaryValue += nibble.BinaryValue;
                    }
                }
                else
                {
                    status.Add("The binary value could not be calculated correctly as there no nibbles.");
                }
                return binaryValue;
            }
        }

        private List<string> status = new List<string>();
        public List<String> Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        /// <summary>
        /// Mnec packets are composed of TODO:??? nibbles.
        /// </summary>
        private Collection<Nibble> nibbles;
        public Collection<Nibble> Nibbles
        {
            get
            {
                return nibbles;
            }
            set
            {
                nibbles = value;
            }
        }
    }
}
