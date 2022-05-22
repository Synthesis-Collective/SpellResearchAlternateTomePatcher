using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellResearchAlternateTomePatcher.Classes
{
    public class ArchetypeVisualInfo
    {
        public Dictionary<string, ArchetypeDisplayParameters> Archetypes { get; set; } = new Dictionary<string, ArchetypeDisplayParameters>();

        public static ArchetypeVisualInfo From(string configText)
        {
            ArchetypeVisualInfo config = new();
            JObject data = JObject.Parse(configText);
            if (data == null) return config;
            JToken? colors = data["Colors"];
            if (colors == null) return config;
            foreach (JProperty archColor in colors)
            {
                if (!config.Archetypes.ContainsKey(archColor.Name))
                {
                    config.Archetypes[archColor.Name] = new ArchetypeDisplayParameters
                    {
                        Name = archColor.Name
                    };
                }
                if (config.Archetypes[archColor.Name].Color != null)
                {
                    Console.WriteLine($"Duplicate color entry for archetype {archColor.Name}!");
                }
                config.Archetypes[archColor.Name].Color = archColor.Value.ToString();
            }
            JToken? images = data["Images"];
            if (images == null) return config;
            foreach (JProperty archImage in images)
            {
                if (!config.Archetypes.ContainsKey(archImage.Name))
                {
                    Console.WriteLine($"Archetype {archImage.Name} found in image list but not in color list!");
                    config.Archetypes[archImage.Name] = new ArchetypeDisplayParameters
                    {
                        Name = archImage.Name,
                        Color = "#000000"
                    };
                }
                if (config.Archetypes[archImage.Name].Image != null)
                {
                    Console.WriteLine($"Duplicate image entry for archetype {archImage.Name}!");
                }
                config.Archetypes[archImage.Name].Image = archImage.Value.ToString();
            }
            return config;
        }
    }
}
