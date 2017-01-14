using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Http
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            return WinRTJson.Serialize(obj);
        }

        public T Deserialize<T>(string content)
        {
            return WinRTJson.Deserialize(content);
        }
    }

    public class SimpleJsonSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            return SimpleJson.SimpleJson.SerializeObject(obj);
        }

        public T Deserialize<T>(string content)
        {
            return SimpleJson.SimpleJson.DeserializeObject<T>(content);
        }

        public static void SetDateFormats(string format)
        {
            SimpleJson.SimpleJson.SetDateFormats(format);
        }
    }

    public class NewtonsoftJsonSerializer : ISerializer
    {
        static DateTimeConverterBase _dateTimeConverter;

        public string Serialize(object obj)
        {
            return _dateTimeConverter == null ? JsonConvert.SerializeObject(obj) : JsonConvert.SerializeObject(obj, _dateTimeConverter);
        }

        public T Deserialize<T>(string content)
        {
            return _dateTimeConverter == null ? JsonConvert.DeserializeObject<T>(content) : JsonConvert.DeserializeObject<T>(content, _dateTimeConverter);
        }

        public static void SetDateFormats(string format)
        {
            _dateTimeConverter = new IsoDateTimeConverter() { DateTimeFormat = format };
        }
    }
}
