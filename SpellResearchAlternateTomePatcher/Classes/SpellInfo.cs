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
        public string? TomeID { get; set; }
        public string? ScrollID { get; set; }
        public bool Enabled { get; set; } = true;
    }
}