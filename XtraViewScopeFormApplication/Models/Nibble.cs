using NationalInstruments;
using Newtonsoft.Json;
using ScopeLibrary.Util;
using System;
using System.Collections.Generic;
using XtraViewScope.Models.Dictionaries;

namespace XtraViewScope.Models
{
    public class Nibble
    {
        private BinaryValue binaryValue;
        [JsonIgnore]
        public BinaryValue BinaryValue
        {
            get
            {
                return binaryValue;
            }
            set
            {
                binaryValue = value;
            }
        }

        public string BinaryValueString
        {
            get
            {
                string b = "";
                return b += binaryValue;
            }
        }

        public int DecimalValue
        {
            get
            {
                return (int) BinaryValue;
            }
        }

        private PrecisionDateTime pulseStartTime;
        public PrecisionDateTime PulseStartTime {
            get
            {
                return pulseStartTime;
            }
            set
            {
                pulseStartTime = value;
            }
        }

        private PrecisionDateTime pulseEndTime;
        public PrecisionDateTime PulseEndTime
        {
            get
            {
                return pulseEndTime;
            }
            set
            {
                pulseEndTime = value;
            }
        }

        public PrecisionTimeSpan PulseDuration
        {
            get
            {
                return PulseEndTime - PulseStartTime;
            }
        }

        private PrecisionDateTime noiseStartTime;
        public PrecisionDateTime NoiseStartTime
        {
            get
            {
                return noiseStartTime;
            }
            set
            {
                noiseStartTime = value;
            }
        }

        private PrecisionDateTime noiseEndTime;
        public PrecisionDateTime NoiseEndTime
        {
            get
            {
                return noiseEndTime;
            }
            set
            {
                noiseEndTime = value;
            }
        }

        public PrecisionTimeSpan NoiseDuration
        {
            get
            {
                if (NoiseEndTime == null)
                {
                    throw new NullReferenceException("End time of noise segment has not been set");
                }
                if (NoiseStartTime == null)
                {
                    throw new NullReferenceException("Start time of noise segment has not been set");
                }
                return NoiseEndTime - NoiseStartTime;
            }
        }

        public PrecisionTimeSpan TotalDuration
        {
            get
            {
                return PulseDuration + NoiseDuration;
            }
        }

        public double OptimalDuration
        {
            get
            {
                try
                {
                    return Add2Dictionary.GetOptimalDuration(TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(TotalDuration));
                }
                catch (KeyNotFoundException)
                {
                    nibbleStatus ="Nibble duration does not fall within available ranges";
                    IsNotValid = true;
                    return -1;
                }
            }
        }

        public double TimeDeviation
        {
            get
            {
                return Math.Abs(TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(TotalDuration) - OptimalDuration);
            }
        }

        private string nibbleStatus = null;
        public string NibbleStatus
        {
            get
            {
                return nibbleStatus;
            }
            set
            {
                nibbleStatus = value;
            }
        }

        private bool isNotValid;
        [JsonIgnore]
        public bool IsNotValid
        {
            get
            {
                return isNotValid;
            }
            set
            {
                isNotValid = value;
            }
        }
    }
}
