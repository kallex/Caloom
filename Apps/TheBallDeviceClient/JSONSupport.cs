using System.IO;
using JsonFx.Json;

namespace TheBall.Support.DeviceClient
{
    public static class JSONSupport
    {
        public static void SerializeToJSONStream(object obj, Stream outputStream)
        {
            var writer = new JsonWriter();
            using (StreamWriter textWriter = new StreamWriter(outputStream))
            {
                writer.Write(obj, textWriter);
            }
        }

        public static string SerializeToJSONString(object obj)
        {
            var writer = new JsonWriter();
            return writer.Write(obj);
        }

        public static T GetObjectFromString<T>(string jsonString)
        {
            var reader = new JsonReader();
            return reader.Read<T>(jsonString);
        }

        public static T GetObjectFromStream<T>(Stream stream)
        {
            var reader = new JsonReader();
            using (TextReader textReader = new StreamReader(stream))
            {
                return reader.Read<T>(textReader);
            }
        }
    }
}
