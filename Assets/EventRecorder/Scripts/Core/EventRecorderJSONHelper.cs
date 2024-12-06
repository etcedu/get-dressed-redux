
//#define USE_BACKUP_SERIALIZER

using System;

namespace SimcoachGames.EventRecorder
{
    public enum JsonFormatting
    {
        NONE,
        PRETTY,
    }

    public static class EventRecorderJSONHelper
    {
        public static string SerializeObject(object value, JsonFormatting jsonFormatting = JsonFormatting.NONE)
        {
#if !USE_BACKUP_SERIALIZER
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, (Newtonsoft.Json.Formatting) jsonFormatting);
#else
        return SimpleJson.SimpleJson.SerializeObject(value);
#endif
        }

        public static bool TryDeserializeObject(string value, Type type, out object obj)
        {
            obj = new object();
#if !USE_BACKUP_SERIALIZER
            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
#else
        obj = SimpleJson.SimpleJson.DeserializeObject(value);
#endif

            return obj != null;
        }
    }
}