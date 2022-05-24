namespace SpellResearchAlternateTomePatcher.Classes
{
    public class ArchetypeDisplayParameters
    {
        private string? _Name;
        public string? Name { get => _Name; set => _Name = value?.ToLower(); }
        public string? Color { get; set; }
        public string? Image { get; set; }

    }
}