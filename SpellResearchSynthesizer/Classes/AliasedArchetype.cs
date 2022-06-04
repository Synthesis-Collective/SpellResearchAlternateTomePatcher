using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SpellResearchSynthesizer.Classes
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AliasedArchetype
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "";
        [JsonProperty(PropertyName = "aliases")]
        public List<string> Aliases { get; set; } = new List<string>();

        public class Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(AliasedArchetype);
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                if (value is not AliasedArchetype arch)
                {
                    throw new ArgumentException("Only AliasedArchetypes");
                }
                writer.WriteValue(arch.Name);
            }

            public override bool CanRead => false;
        }
    }
}
