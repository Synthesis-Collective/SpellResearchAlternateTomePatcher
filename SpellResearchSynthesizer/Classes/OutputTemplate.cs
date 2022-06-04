using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellResearchSynthesizer.Classes
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class OutputTemplate
    {
        [JsonProperty("newSpells")]
        public List<SpellInfo> NewSpells { get; set; } = new List<SpellInfo>();
        [JsonProperty("removedSpells")]
        public List<SpellInfo> RemovedSpells { get; set; } = new List<SpellInfo>();
        [JsonProperty("researchDataLists")]
        public JToken? ResearchDataLists { get; set; }
    }
}
