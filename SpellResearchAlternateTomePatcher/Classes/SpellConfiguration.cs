using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellResearchAlternateTomePatcher.Classes
{
    public class SpellConfiguration
    {
        public List<SpellInfo> Spells { get; set; } = new List<SpellInfo>();

        public static SpellConfiguration From(string spellconf, ArchetypeList allowedArchetypes)
        {
            SpellConfiguration config = new SpellConfiguration();
            JObject data = JObject.Parse(spellconf);
            if (data == null) return config;
            // Mysticism JSON Patch handling
            JToken? newSpells = data["newSpells"];
            if (newSpells != null)
            {
                foreach (JToken newSpell in newSpells)
                {
                    string name = (string?)newSpell["name"] ?? string.Empty;
                    if (name == string.Empty)
                    {
                        Console.WriteLine("Error reading spell name");
                        continue;
                    }
                    Console.WriteLine($"Processing spell \"{name}\"");
                    string spellID = (string?)newSpell["spellId"] ?? string.Empty;
                    if (spellID == string.Empty)
                    {
                        Console.WriteLine("Error reading spell ID");
                        continue;
                    }
                    string skill = (string?)newSpell["school"] ?? string.Empty;
                    if (skill == string.Empty)
                    {
                        Console.WriteLine($"Error: Skill not found");
                        continue;
                    }
                    if (!allowedArchetypes.Skills.Any(archetype => archetype.Name.ToLower() == skill.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == skill.ToLower())))
                    {
                        Console.WriteLine($"Error: Skill \"{skill}\" not known");
                        continue;
                    }
                    string level = (string?)newSpell["tier"] ?? string.Empty;
                    if (level == string.Empty)
                    {
                        Console.WriteLine($"Error: Spell level not found");
                        continue;
                    }
                    if (!allowedArchetypes.Levels.Any(archetype => archetype.Name.ToLower() == level.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == level.ToLower())))
                    {
                        Console.WriteLine($"Error: Spell level \"{level}\" not known");
                        continue;
                    }
                    string castingType = (string?)newSpell["castingType"] ?? string.Empty;
                    if (castingType == string.Empty)
                    {
                        Console.WriteLine($"Error: Casting type not found");
                        continue;
                    }
                    if (!allowedArchetypes.CastingTypes.Any(archetype => archetype.Name.ToLower() == castingType.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == castingType.ToLower())))
                    {
                        Console.WriteLine($"Error: Casting type \"{castingType}\" not known");
                        continue;
                    }
                    JArray? target = (JArray?)newSpell["targeting"];
                    if (target == null)
                    {
                        Console.WriteLine("Targeting type list not found");
                        continue;
                    }
                    if (target.Count == 0)
                    {
                        Console.WriteLine("Targeting type list empty");
                        continue;
                    }
                    List<string> foundTargets = new List<string>();
                    foreach (string? targetType in target)
                    {
                        if (string.IsNullOrEmpty(targetType))
                        {
                            Console.WriteLine("Empty targeting type found in list");
                            continue;
                        }
                        if (!allowedArchetypes.Targets.Any(archetype => archetype.Name.ToLower() == targetType.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == targetType.ToLower())))
                        {
                            Console.WriteLine($"Targeting type {targetType} not known");
                            continue;
                        }
                        foundTargets.Add(targetType);
                    }
                    JArray? elements = (JArray?)newSpell["elements"];
                    if (elements == null)
                    {
                        Console.WriteLine("Element list not found");
                        continue;
                    }
                    if (elements.Count == 0)
                    {
                        Console.WriteLine("Element list empty");
                        continue;
                    }
                    List<string> foundElements = new List<string>();
                    foreach (string? element in elements)
                    {
                        if (string.IsNullOrEmpty(element))
                        {
                            Console.WriteLine("Empty element found in list");
                            continue;
                        }
                        if (!allowedArchetypes.Elements.Any(archetype => archetype.Name.ToLower() == element.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == element.ToLower())))
                        {
                            Console.WriteLine($"Element {element} not known");
                            continue;
                        }
                        foundElements.Add(element);
                    }
                    JArray? techniques = (JArray?)newSpell["techniques"];
                    if (techniques == null)
                    {
                        Console.WriteLine("Technique list not found");
                        continue;
                    }
                    if (techniques.Count == 0)
                    {
                        Console.WriteLine("Technique list empty");
                        continue;
                    }
                    List<string> foundTechniques = new List<string>();
                    foreach (string? technique in techniques)
                    {
                        if (string.IsNullOrEmpty(technique))
                        {
                            Console.WriteLine("Empty technique found in list");
                            continue;
                        }
                        if (!allowedArchetypes.Techniques.Any(archetype => archetype.Name.ToLower() == technique.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == technique.ToLower())))
                        {
                            Console.WriteLine($"Technique {technique} not known");
                            continue;
                        }
                        foundTechniques.Add(technique);
                    }
                    string? tomeID = (string?)newSpell["tomeId"];
                    string? scrollID = (string?)newSpell["scrollId"];
                    config.Spells.Add(new SpellInfo
                    {
                        SpellID = spellID,
                        Name = name,
                        School = skill,
                        Tier = level,
                        CastingType = castingType,
                        Targeting = foundTargets,
                        Elements = foundElements,
                        Techniques = foundTechniques,
                        TomeID = tomeID,
                        ScrollID = scrollID
                    });
                }
            }
            else
            {

            }
            return config;
        }
    }
}