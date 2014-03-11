using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScopeLibrary.Util;
using System;

namespace XtraViewScope.Models
{
    public class NibbleJsonConverter : JsonConverter
    {
        const string FULL_DATE_AND_TIME_FORMAT = "yyyy/MM/dd HH:mm:ss.FFFFFFF";

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Nibble);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                JObject o = (JObject)t;

                Nibble nibble = (Nibble)value;
                //The json serializer returns precisionDateTimes as yyyy/mm/dd, this makes sure that it is in the format yyyy/mm/dd hh:mm:ss.ffffffff
                o.Property("PulseStartTime").Value = JToken.FromObject(nibble.PulseStartTime.ToString(FULL_DATE_AND_TIME_FORMAT));
                o.Property("PulseEndTime").Value = JToken.FromObject(nibble.PulseEndTime.ToString(FULL_DATE_AND_TIME_FORMAT));
                o.Property("NoiseStartTime").Value = JToken.FromObject(nibble.NoiseStartTime.ToString(FULL_DATE_AND_TIME_FORMAT));
                o.Property("NoiseEndTime").Value = JToken.FromObject(nibble.NoiseEndTime.ToString(FULL_DATE_AND_TIME_FORMAT));

                //The nano second portion of the duration is all that is returned when calculating these nibbles.
                //This makes sure that it is returned in a format that is human readable and rounded to 3 decimal places.
                o.Property("PulseDuration").Value = JToken.FromObject(Math.Round(TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(nibble.PulseDuration), 3));
                o.Property("NoiseDuration").Value = JToken.FromObject(Math.Round(TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(nibble.NoiseDuration), 3));
                o.Property("TotalDuration").Value = JToken.FromObject(Math.Round(TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(nibble.TotalDuration), 3));

                o.Property("TimeDeviation").Value = JObject.Parse(" { " + ScopeLibrary.Util.TimeDeviation.GradeDeviation(nibble.TimeDeviation) + " : " + JToken.FromObject(Math.Round(nibble.TimeDeviation, 3)) + " } ");

                if (nibble.NibbleStatus == null)
                {
                    o.Property("NibbleStatus").Remove();
                }

                o.WriteTo(writer);
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }
    }
}
