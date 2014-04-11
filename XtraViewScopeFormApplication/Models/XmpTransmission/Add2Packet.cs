using ScopeLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XtraViewScopeFormApplication.Models.Enums;

namespace XtraViewScopeFormApplication.Models.XmpTransmission
{
    /// <summary>
    /// ADD2 packets are a type of XMP Packet defined by having 8 nibbles. 
    /// The binary value of the first nibble is always 1110 (14).
    /// The second nibble is the checksum.
    /// The first three bits of the third nibble is the message type.
    /// The fourth nibble is the sequence number.
    /// The rest of the nibbles contain the payload.
    /// </summary>
    public class Add2Packet : IrPacket
    {
        public Add2Packet()
        {
        }

        public Add2Packet(Collection<Nibble> nibbles)
        {
            Nibbles = nibbles;
        }

        public IrType IrType
        {
            get
            {
                return IrType.Add2;
            }
        }

        /// <summary>
        /// Returns the calculation of the checksum. The sum of the nibbles mod 16 should be zero.
        /// </summary>
        /// <exception cref="MalformedXmpPacketException">If the checksum doesn't equal zero then there is a problem with this XMP Packet</exception>
        public int Checksum
        {
            get
            {
                int checksum = 0;
                if (Nibbles != null && Nibbles.Count > 1)
                {
                    if (Nibbles[1].IsNotValid)
                    {
                        status.Add("The nibble containing the checksum has either not been captured or the ADD2 Packet is malformed");
                        return -1;
                    }

                    foreach (Nibble nibble in Nibbles)
                    {
                        checksum += nibble.DecimalValue;
                    }
                    if (checksum % 16 != 0)
                    {
                        status.Add("The nibble containing the checksum has either not been captured or the ADD2 Packet is malformed");
                        return checksum;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    status.Add("The nibble containing the checksum has either not been captured or the ADD2 Packet is malformed");
                    return -1;
                }
            }
        }

        /// <summary>
        /// The message type for ADD2 packets can be type 0, type 1, or type 2. This is obtained from the 3rd nibble bitshifted 1 bit to the right.
        /// </summary>
        public MessageType MessageType
        {
            get
            {
                if (nibbles != null && nibbles.Count > 3)
                {
                    if (Nibbles[2].IsNotValid)
                    {
                        status.Add("The nibble containing the message type has either not been captured or the ADD2 Packet is malformed");
                        return MessageType.None;
                    }
                    return (MessageType) (nibbles[2].DecimalValue > 1 ? nibbles[2].DecimalValue >> 1 : 0);
                }
                else
                {
                    status.Add("The nibble containing the message type has either not been captured or the ADD2 Packet is malformed");
                    return MessageType.None;
                }
            }
        }

        public int SequenceNumber
        {
            get
            {
                if (nibbles != null && nibbles.Count > 3)
                {
                    if (Nibbles[3].IsNotValid)
                    {
                        status.Add("The nibble containing the sequence number has either not been captured or the ADD2 Packet is malformed.");
                        return -1;
                    }

                    return nibbles[3].DecimalValue;
                }
                else
                {
                    status.Add("The nibble containing the sequence number has either not been captured or the ADD2 Packet is malformed.");
                    return -1;
                }
            }
        }
        /// <summary>
        /// The payload of the ADD2 packet is contained in the last 4 nibbles. The decimal payload is calculated by summing the 4 payload data in the 4 nibbles.
        /// </summary>
        public int PayloadDecimal
        {
            get
            {
                int payload = 0;
                if (Nibbles != null && Nibbles.Count == 8)
                {
                    for (int i = 4; i < Nibbles.Count; i++)
                    {
                        if (Nibbles[i].IsNotValid)
                        {
                            status.Add("The payload could not be calculated correctly as there are invalid nibbles.");
                            return -1;
                        }

                        payload += Nibbles[i].DecimalValue;
                    }
                }
                else
                {
                    status.Add("The payload could not be calculated correctly as there are nibbles missing.");
                }
                return payload;
            }
        }

        /// <summary>
        /// The payload of the ADD2 packet is contained in the last 4 nibbles. The binary payload is calculated by concatenating the binary value of the last 4 nibbles.
        /// </summary>
        public string PayloadBinary
        {
            get
            {
                string payload = "";
                if (Nibbles != null && Nibbles.Count == 8)
                {
                    for (int i = 4; i < Nibbles.Count; i++)
                    {
                        payload += Nibbles[i].BinaryValue;
                    }

                    payload = payload.Replace("b", "");
                }

                return payload;
            }
        }

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
        /// ADD2 packets are composed of 8 nibbles.
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
