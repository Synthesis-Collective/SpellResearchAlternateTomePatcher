using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellResearchAlternateTomePatcher.Classes
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ArchetypeList
    {
        [JsonProperty(PropertyName = "levels")]
        public List<AliasedArchetype> Levels { get; set; } = new List<AliasedArchetype>();
        [JsonProperty(PropertyName = "castingTypes")]
        public List<AliasedArchetype> CastingTypes { get; set; } = new List<AliasedArchetype>();
        [JsonProperty(PropertyName = "targets")]
        public List<AliasedArchetype> Targets { get; set; } = new List<AliasedArchetype>();
        [JsonProperty(PropertyName = "skills")]
        public List<AliasedArchetype> Skills { get; set; } = new List<AliasedArchetype>();
        [JsonProperty(PropertyName = "elements")]
        public List<AliasedArchetype> Elements { get; set; } = new List<AliasedArchetype>();
        [JsonProperty(PropertyName = "techniques")]
        public List<AliasedArchetype> Techniques { get; set; } = new List<AliasedArchetype>();

        [JsonObject(MemberSerialization.OptIn)]
        public class AliasedArchetype
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; } = "";
            [JsonProperty(PropertyName = "aliases")]
            public List<string> Aliases { get; set; } = new List<string>();
        }
    }
}
