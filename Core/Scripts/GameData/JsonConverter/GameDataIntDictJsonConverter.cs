using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class GameDataIntDictJsonConverter<T> : JsonConverter
        where T : BaseGameData
    {
        private IDictionary<int, T> _collection;

        public GameDataIntDictJsonConverter(IDictionary<int, T> collection)
        {
            _collection = collection;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<T, int>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            IDictionary<T, int> dict = (IDictionary<T, int>)existingValue ?? new Dictionary<T, int>();
            foreach (var prop in obj.Properties())
            {
                if (string.IsNullOrEmpty(prop.Name))
                    dict.Add(null, (int)prop.Value);
                else if (_collection.TryGetValue(BaseGameData.MakeDataId(prop.Name), out T key))
                    dict.Add(key, (int)prop.Value);
            }
            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IDictionary<T, int> dict = (IDictionary<T, int>)value;
            JObject obj = new JObject();
            foreach (var kvp in dict)
            {
                if (kvp.Key != null)
                    obj.Add(kvp.Key.Id, kvp.Value);
                else
                    obj.Add(string.Empty, kvp.Value);
            }
            obj.WriteTo(writer);
        }
    }
}
