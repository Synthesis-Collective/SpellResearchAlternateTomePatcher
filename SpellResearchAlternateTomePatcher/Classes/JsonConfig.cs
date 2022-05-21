using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellResearchAlternateTomePatcher.Classes
{
    public class JsonConfig
    {
        public Dictionary<string, Archetype> Archetypes { get; set; } = new Dictionary<string, Archetype>();

        public static JsonConfig From(string configText)
        {
            JsonConfig config = new JsonConfig();
            JObject data = JObject.Parse(configText);
            if (data == null) return config;
            foreach (JProperty property in data.Properties())
            {
                if (property.Name == "Colors")
                {
                    foreach (JProperty archColor in property.Value)
                    {
                        if (!config.Archetypes.ContainsKey(archColor.Name))
                        {
                            config.Archetypes[archColor.Name] = new Archetype
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
                }
                else if (property.Name == "Images")
                {
                    foreach (JProperty archImage in property.Value)
                    {
                        if (!config.Archetypes.ContainsKey(archImage.Name))
                        {
                            Console.WriteLine($"Archetype {archImage.Name} found in image list but not in color list!");
                            config.Archetypes[archImage.Name] = new Archetype
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
                }
            }
            return config;
        }
    }
}
