using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Common.Utils
{
    public static class JsonUtils
    {

        public static T ParseJson<T> (string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            T t = (T) serializer.ReadObject(stream);
            return t;
        }


        public static string ToJson<T> (T t)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, t);
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            string json = Encoding.UTF8.GetString(dataBytes);
            return json;
        }

    }
}
