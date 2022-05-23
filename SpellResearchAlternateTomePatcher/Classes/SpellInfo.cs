using System.Collections.Generic;

namespace SpellResearchAlternateTomePatcher.Classes
{
    public class SpellInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Tier { get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;
        public string CastingType { get; set; } = string.Empty;
        public List<string> Targeting { get; set; } = new List<string>();
        public List<string> Elements { get; set; } = new List<string>();
        public List<string> Techniques { get; set; } = new List<string>();
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