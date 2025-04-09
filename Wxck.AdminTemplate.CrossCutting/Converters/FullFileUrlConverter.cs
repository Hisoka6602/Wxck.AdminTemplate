using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wxck.AdminTemplate.CrossCutting.Utils;

namespace Wxck.AdminTemplate.CrossCutting.Converters {

    public class FullFileUrlConverter : JsonConverter<string> {

        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer) {
            if (string.IsNullOrWhiteSpace(value)) {
                writer.WriteNull();
                return;
            }

            var httpContext = HttpContextHelper.Current;
            if (httpContext == null) {
                writer.WriteValue(value);
                return;
            }

            var request = httpContext.Request;

            // 获取应用的 PathBase + 静态文件的 RequestPath（如 /scr）

            var basePath = request.PathBase.HasValue ? request.PathBase.Value : "";

            var normalizedPath = value.Replace('\\', '/').TrimStart('/');

            var fullUrl = $"{request.Scheme}://{request.Host}{basePath}/{normalizedPath}";
            writer.WriteValue(fullUrl);
        }

        public override string? ReadJson(JsonReader reader, Type objectType, string? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) {
            return reader.Value?.ToString();
        }
    }
}