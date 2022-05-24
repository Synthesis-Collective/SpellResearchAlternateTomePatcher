using System.Collections.Generic;

namespace SpellResearchAlternateTomePatcher.Classes
{
    public class SpellInfo
    {
        public string Name { get; set; } = string.Empty;
        private string _Tier = string.Empty;
        public string Tier { get => _Tier; set => _Tier = value.ToLower(); }
        public string School { get; set; } = string.Empty;
        private string _CastingType = string.Empty;
        public string CastingType { get => _CastingType; set => _CastingType = value.ToLower(); }
        public List<AliasedArchetype> Targeting { get; set; } = new List<AliasedArchetype>();
        public List<AliasedArchetype> Elements { get; set; } = new List<AliasedArchetype>();
        public List<AliasedArchetype> Techniques { get; set; } = new List<AliasedArchetype>();
        public string SpellID { get; set; } = string.Empty;
        public string SpellESP => string.IsNullOrEmpty(SpellID) ? "" : SpellID.Split('|')[1];
        public string SpellFormID => string.IsNullOrEmpty(SpellID) ? "" : SpellID.Split('|')[2];
        public string? TomeID { get; set; }
        public string? TomeESP => string.IsNullOrEmpty(TomeID) ? null : TomeID.Split('|')[1];
        public string? TomeFormID => string.IsNullOrEmpty(TomeID) ? null : TomeID.Split('|')[2];
        public string? ScrollID { get; set; }
        public bool Enabled { get; set; } = true;
    }
}