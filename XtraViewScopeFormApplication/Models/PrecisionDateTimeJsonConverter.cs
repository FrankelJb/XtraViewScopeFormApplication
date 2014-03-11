using NationalInstruments;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtraViewScope.Models
{
    class PrecisionDateTimeJsonConverter : JsonConverter
    {
        const string FULL_DATE_AND_TIME_FORMAT = "yyyy/MM/dd HH:mm:ss.FFFFFFF";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var precisionDateTime = (PrecisionDateTime)value;
            writer.WriteStartObject();
            serializer.Serialize(writer, precisionDateTime.ToString(FULL_DATE_AND_TIME_FORMAT));
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(PrecisionDateTime).IsAssignableFrom(objectType);
        }
    }
}
