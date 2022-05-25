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
            SpellConfiguration config = new();
            if (spellconf.StartsWith("{"))
            {
                JObject data = JObject.Parse(spellconf);
                if (data == null) return config;
                config = ParseMysticismFormat(data, allowedArchetypes);
            }
            else if (spellconf.StartsWith("["))
            {
                JArray data = JArray.Parse(spellconf);
                if (data == null) return config;
                config = ParseJSONFormat(data, allowedArchetypes);
            }
            return config;
        }

        private static SpellConfiguration ParseMysticismFormat(JObject data, ArchetypeList allowedArchetypes)
        {
            // Mysticism JSON Patch handling
            SpellConfiguration config = new SpellConfiguration();
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
                    string spellID = (string?)newSpell["spellId"] ?? string.Empty;
                    if (spellID == string.Empty)
                    {
                        Console.WriteLine($"Error reading spell ID for {name}");
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
                    List<AliasedArchetype> foundTargets = new();
                    foreach (string? targetType in target)
                    {
                        if (string.IsNullOrEmpty(targetType))
                        {
                            Console.WriteLine("Empty targeting type found in list");
                            continue;
                        }
                        AliasedArchetype? arch = allowedArchetypes.Targets.FirstOrDefault(archetype => archetype.Name.ToLower() == targetType.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == targetType.ToLower()));
                        if (arch == null)
                        {
                            Console.WriteLine($"Targeting type {targetType} not known");
                            continue;
                        }
                        foundTargets.Add(arch);
                    }
                    JArray? elements = (JArray?)newSpell["elements"];
                    if (elements == null)
                    {
                        Console.WriteLine("Element list not found");
                        continue;
                    }
                    List<AliasedArchetype> foundElements = new();
                    foreach (string? element in elements)
                    {
                        if (string.IsNullOrEmpty(element))
                        {
                            Console.WriteLine("Empty element found in list");
                            continue;
                        }
                        AliasedArchetype? arch = allowedArchetypes.Elements.FirstOrDefault(archetype => archetype.Name.ToLower() == element.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == element.ToLower()));
                        if (arch == null)
                        {
                            Console.WriteLine($"Element {element} not known");
                            continue;
                        }
                        foundElements.Add(arch);
                    }
                    JArray? techniques = (JArray?)newSpell["techniques"];
                    if (techniques == null)
                    {
                        Console.WriteLine("Technique list not found");
                        continue;
                    }
                    List<AliasedArchetype> foundTechniques = new();
                    foreach (string? technique in techniques)
                    {
                        if (string.IsNullOrEmpty(technique))
                        {
                            Console.WriteLine("Empty technique found in list");
                            continue;
                        }
                        AliasedArchetype? arch = allowedArchetypes.Techniques.FirstOrDefault(archetype => archetype.Name.ToLower() == technique.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == technique.ToLower()));
                        if (arch == null)
                        {
                            Console.WriteLine($"Technique {technique} not known");
                            continue;
                        }
                        foundTechniques.Add(arch);
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
            return config;
        }

        private static SpellConfiguration ParseJSONFormat(JArray data, ArchetypeList allowedArchetypes)
        {
            // Mysticism JSON Patch handling
            SpellConfiguration config = new SpellConfiguration();
            if (data != null)
            {
                foreach (JToken newSpell in data)
                {
                    string name = (string?)newSpell["comment"] ?? string.Empty;
                    if (name == string.Empty)
                    {
                        Console.WriteLine("Error reading spell name");
                        continue;
                    }
                    string spellID = (string?)newSpell["spell"] ?? string.Empty;
                    if (spellID == string.Empty)
                    {
                        Console.WriteLine($"Error reading spell ID for {name}");
                        continue;
                    }
                    string skill = (string?)newSpell["skill"] ?? string.Empty;
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
                    string level = (string?)newSpell["level"] ?? string.Empty;
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
                    string castingType = (string?)newSpell["casting"] ?? string.Empty;
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
                    JArray? target = (JArray?)newSpell["target"];
                    if (target == null)
                    {
                        Console.WriteLine("Targeting type list not found");
                        continue;
                    }
                    List<AliasedArchetype> foundTargets = new();
                    foreach (string? targetType in target)
                    {
                        if (string.IsNullOrEmpty(targetType))
                        {
                            Console.WriteLine("Empty targeting type found in list");
                            continue;
                        }
                        AliasedArchetype? arch = allowedArchetypes.Targets.FirstOrDefault(archetype => archetype.Name.ToLower() == targetType.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == targetType.ToLower()));
                        if (arch == null)
                        {
                            Console.WriteLine($"Targeting type {targetType} not known");
                            continue;
                        }
                        foundTargets.Add(arch);
                    }
                    JArray? elements = (JArray?)newSpell["elements"];
                    if (elements == null)
                    {
                        Console.WriteLine("Element list not found");
                        continue;
                    }
                    List<AliasedArchetype> foundElements = new();
                    foreach (string? element in elements)
                    {
                        if (string.IsNullOrEmpty(element))
                        {
                            Console.WriteLine("Empty element found in list");
                            continue;
                        }
                        AliasedArchetype? arch = allowedArchetypes.Elements.FirstOrDefault(archetype => archetype.Name.ToLower() == element.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == element.ToLower()));
                        if (arch == null)
                        {
                            Console.WriteLine($"Element {element} not known");
                            continue;
                        }
                        foundElements.Add(arch);
                    }
                    JArray? techniques = (JArray?)newSpell["techniques"];
                    if (techniques == null)
                    {
                        Console.WriteLine("Technique list not found");
                        continue;
                    }
                    List<AliasedArchetype> foundTechniques = new();
                    foreach (string? technique in techniques)
                    {
                        if (string.IsNullOrEmpty(technique))
                        {
                            Console.WriteLine("Empty technique found in list");
                            continue;
                        }
                        AliasedArchetype? arch = allowedArchetypes.Techniques.FirstOrDefault(archetype => archetype.Name.ToLower() == technique.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == technique.ToLower()));
                        if (arch == null)
                        {
                            Console.WriteLine($"Technique {technique} not known");
                            continue;
                        }
                        foundTechniques.Add(arch);
                    }
                    string? tomeID = (string?)newSpell["tome"];
                    string? scrollID = (string?)newSpell["scroll"];
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
            return config;
        }
    }
}