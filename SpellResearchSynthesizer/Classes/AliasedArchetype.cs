using Newtonsoft.Json;
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
    }
}
