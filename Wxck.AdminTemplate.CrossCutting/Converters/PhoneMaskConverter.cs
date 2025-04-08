using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Wxck.AdminTemplate.CrossCutting.Converters {

    public class PhoneMaskConverter : JsonConverter<string> {

        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer) {
            if (string.IsNullOrEmpty(value)) {
                writer.WriteValue(value); // 如果是空值，不做处理
                return;
            }
            var maskedPhone = value.Length == 11 ? $"{value[..3]}****{value[7..]}" : value;
            writer.WriteValue(maskedPhone);
        }

        public override string? ReadJson(JsonReader reader, Type objectType, string? existingValue, bool hasExistingValue,
            JsonSerializer serializer) {
            return reader.Value?.ToString();
        }
    }
}