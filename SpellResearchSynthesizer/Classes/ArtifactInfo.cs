using Mutagen.Bethesda.Skyrim;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellResearchSynthesizer.Classes
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ArtifactInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        public string ArtifactID { get; set; } = string.Empty;
        public IItemGetter? ArtifactForm { get; set; }
        public string ArtifactESP => ArtifactForm == null ? string.IsNullOrEmpty(ArtifactID) ? "" : ArtifactID.Split('|')[1] : ArtifactForm.FormKey.ModKey.FileName.ToString().ToLower();
        public string ArtifactFormID => ArtifactForm == null ? string.IsNullOrEmpty(ArtifactID) ? "" : ArtifactID.Split('|')[2] : ArtifactForm.FormKey.ID.ToString("X6").ToLower();
        [JsonProperty("artifactID")]
        public string JsonArtifactID => $"__formData|{ArtifactESP}|0x{ArtifactFormID}";
        [JsonProperty("tier")]
        public int Tier { get; set; } = 0;
        [JsonProperty("schools")]
        public List<string> Schools { get; set; } = new();
        [JsonProperty("castingTypes")]
        public List<string> CastingTypes { get; set; } = new();
        [JsonProperty("targeting", ItemConverterType = typeof(AliasedArchetype.Converter))]
        public List<AliasedArchetype> Targeting { get; set; } = new();
        [JsonProperty("elements", ItemConverterType = typeof(AliasedArchetype.Converter))]
        public List<AliasedArchetype> Elements { get; set; } = new List<AliasedArchetype>();
        [JsonProperty("techniques", ItemConverterType = typeof(AliasedArchetype.Converter))]
        public List<AliasedArchetype> Techniques { get; set; } = new List<AliasedArchetype>();
        [JsonProperty("equippableAll")]
        public bool Equippable { get; set; } = false;
        [JsonProperty("equippableArtifact")]
        public bool EquippableArtifact { get; set; } = false;
        [JsonProperty("equippableText")]
        public bool EquippableText { get; set; } = false;
    }
}
