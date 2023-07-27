using Newtonsoft.Json;
using System;
using System.Globalization;

namespace CsvJsonConverter
{
    internal sealed class FormatNumbersAsTextConverter : JsonConverter
    {
        public override bool CanRead => false;
        public override bool CanWrite => true;
        public override bool CanConvert(Type type) => type == typeof(int);

        public override void WriteJson(
            JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
                return;

            int number = (int)value;
            writer.WriteValue(number.ToString(CultureInfo.InvariantCulture));
        }

        public override object ReadJson(
            JsonReader reader, Type type, object? existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
