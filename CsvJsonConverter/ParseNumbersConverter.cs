using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CsvJsonConverter
{
    internal sealed class ParseNumbersConverter : JsonConverter
    {
        public override bool CanRead => false;
        public override bool CanWrite => true;
        public override bool CanConvert(Type type) => type == typeof(string);
        private static readonly Regex regexDecimal = new("^-?[0-9]*\\.?[0-9]+$");

        public override void WriteJson(
            JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
                return;

            var str = value as string;
            if (str is null)
                return;

            if (!regexDecimal.IsMatch(str) ||
                !decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalResult) ||
                (str.StartsWith('0') && decimalResult != 0)
            )
            {
                writer.WriteValue(str.ToString());
                return;
            }

            if (long.TryParse(str, out var longResult) && longResult == decimalResult)
                writer.WriteValue(longResult);
            else
            {
                //double x = d - Math.Truncate(d);

                writer.WriteValue(decimalResult);
            }
        }

        public override object ReadJson(
            JsonReader reader, Type type, object? existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
