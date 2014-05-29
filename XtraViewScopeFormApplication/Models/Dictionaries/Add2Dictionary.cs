using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using XtraViewScopeFormApplication.Models.Enums;

namespace  XtraViewScopeFormApplication.Models.Dictionaries
{
    public static class Add2Dictionary
    {
        /// <summary>
        /// This dictionary is used in definition of the ADD2/OpenTV protocol. The duration of a nibble maps to a binary value described in this dictionary.
        /// </summary>
        public static Dictionary<NibbleKey, BinaryValue> NibbleDictionary = new Dictionary<NibbleKey, BinaryValue> 
        {
            { new NibbleKey(915, 1025),  BinaryValue.b0000 },
            { new NibbleKey(1051, 1162), BinaryValue.b0001 },
            { new NibbleKey(1188, 1299), BinaryValue.b0010 },
            { new NibbleKey(1325, 1435), BinaryValue.b0011 },
            { new NibbleKey(1461, 1572), BinaryValue.b0100 },
            { new NibbleKey(1598, 1709), BinaryValue.b0101 },
            { new NibbleKey(1735, 1846), BinaryValue.b0110 },
            { new NibbleKey(1872, 1982), BinaryValue.b0111 },
            { new NibbleKey(2008, 2119), BinaryValue.b1000 },
            { new NibbleKey(2145, 2256), BinaryValue.b1001 },
            { new NibbleKey(2282, 2392), BinaryValue.b1010 },
            { new NibbleKey(2418, 2529), BinaryValue.b1011 },
            { new NibbleKey(2555, 2666), BinaryValue.b1100 },
            { new NibbleKey(2692, 2803), BinaryValue.b1101 },
            { new NibbleKey(2829, 2939), BinaryValue.b1110 },
            { new NibbleKey(2965, 3076), BinaryValue.b1111 }
        };

        /// <summary>
        /// Get the binary value assigned to each nibble duration
        /// </summary>
        /// <param name="nibbleDuration">duration of nibble calculated as pulse duration + noise duration</param>
        /// <returns>binary value</returns>
        /// <exception cref="KeyNotFoundException">Thrown because the duration of the nibble is not between the upper and lower bounds of any of the table's keys</exception>
        public static BinaryValue GetBinaryData(double nibbleDuration)
        {
            foreach (NibbleKey key in NibbleDictionary.Keys)
            {
                if (key.LowerLimit <= nibbleDuration && key.UpperLimit >= nibbleDuration)
                {
                    return NibbleDictionary[key];
                }
            }
            //return 0.0;
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Get the total deviation of the duration of the nibble from the optimal nibble duration.
        /// 
        /// A nibble's duration should fall within a range of +-2.7 microseconds off the optimal duration calculated as (nibble decoding total time lower limit + nibble decoding total time upper limit) / 2
        /// </summary>
        /// <param name="nibbleDuration">duration of nibble calculated as pulse duration + noise duration</param>
        /// <returns>total deviation from the optimal duration</returns>
        /// <exception cref="KeyNotFoundException">Thrown because the duration of the nibble is not between the upper and lower bounds of any of the table's keys</exception>
        public static double GetDeviation(double nibbleDuration)
        {
            foreach (NibbleKey key in NibbleDictionary.Keys)
            {
                if (key.LowerLimit <= nibbleDuration && key.UpperLimit >= nibbleDuration)
                {
                    double optimalValue = (key.LowerLimit + key.UpperLimit) / 2;
                    return Math.Abs(optimalValue - nibbleDuration);
                }
            }
            //return 0.0;
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Get the optimal duration of the nibble.
        /// 
        /// A nibble's duration should fall within a range of +-2.7 microseconds off the optimal duration calculated as (nibble decoding total time lower limit + nibble decoding total time upper limit) / 2
        /// </summary>
        /// <param name="nibbleDuration">duration of nibble calculated as pulse duration + noise duration</param>
        /// <returns>optimal duration for nibble range</returns>
        /// <exception cref="KeyNotFoundException">Thrown because the duration of the nibble is not between the upper and lower bounds of any of the table's keys</exception>
        public static double GetOptimalDuration(double nibbleDuration)
        {
            foreach (NibbleKey key in NibbleDictionary.Keys)
            {
                if (key.LowerLimit <= nibbleDuration && key.UpperLimit >= nibbleDuration)
                {
                    return (key.LowerLimit + key.UpperLimit) / 2;
                }
            }
            //return 0.0;
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Get the failed duration of the nibble.
        /// 
        /// If a nibble's duration falls outside of the valid ranges, we must be able to return a deviation from a valid duration.
        /// </summary>
        /// <param name="nibbleDuration">duration of nibble calculated as pulse duration + noise duration</param>
        /// <returns>possible optimal duration for nibble range</returns>
        public static double GetFailedOptimalDuration(double nibbleDuration)
        {
            double optimalDuration = 0;
            if (nibbleDuration < 915)
            {
                optimalDuration = (915 + 1025) / 2;
            }
            else if (nibbleDuration > 3076)
            {
                optimalDuration = (2965 + 3076) / 2;
            }
            else
            {
                foreach (NibbleKey key in NibbleDictionary.Keys)
                {
                    if (key.UpperLimit < nibbleDuration)
                    {
                        optimalDuration = (key.LowerLimit + key.UpperLimit) / 2;
                    }
                }
            }

            return optimalDuration;
        }

        /// <summary>
        /// The nibble key's are used when looking up the binary value of a nibble's duration.
        /// 
        /// The key is defined using the nibble decoding total time lower limit and the nibble decoding total time upper limit.
        /// </summary>
        public class NibbleKey
        {
            public NibbleKey(int lowerLimit, int upperLimit)
            {
                this.lowerLimit = lowerLimit;
                this.upperLimit = upperLimit;
            }

            private int lowerLimit;
            public int LowerLimit
            {
                get
                {
                    return lowerLimit;
                }
            }

            private int upperLimit;
            public int UpperLimit
            {
                get
                {
                    return upperLimit;
                }
            }
        }
    }
}
