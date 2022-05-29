using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpellResearchAlternateTomePatcher.Classes
{
    public class SpellConfiguration
    {
        public List<SpellInfo> Spells { get; set; } = new List<SpellInfo>();

        public static SpellConfiguration FromJson(string spellconf, ArchetypeList allowedArchetypes)
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
            SpellConfiguration config = new();
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
            SpellConfiguration config = new();
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

        private static readonly Regex rx = new("^.*\\(\\s*(?<fid>(0x)?[a-fA-F0-9]+),\\s\"(?<esp>.*\\.es[pml])\".*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        private static readonly Regex rskill = new("^.*_SR_ListSpellsSkill(?<skill>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex rcasting = new("^.*_SR_ListSpellsCasting(?<casting>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex rlevel = new("^.*_SR_ListAllSpells[1-5](?<level>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex rtarget = new("^.*_SR_ListSpellsTarget(?<target>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex rtechnique = new("^.*_SR_ListSpellsTechnique(?<technique>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex relement = new("^.*_SR_ListSpellsElement(?<element>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static SpellConfiguration FromPsc(string spellconf, ArchetypeList allowedArchetypes)
        {
            SpellConfiguration config = new();
            SpellInfo? spellInfo = null;
            foreach (string line in spellconf.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries))
            {
                if (line.Contains("TempSpell", StringComparison.OrdinalIgnoreCase) && line.Contains("GetFormFromFile", StringComparison.OrdinalIgnoreCase))
                {
                    MatchCollection matches = rx.Matches(line);
                    spellInfo = new();
                    string fid = matches.First().Groups["fid"].Value.Trim();
                    string esp = matches.First().Groups["esp"].Value.Trim();
                    spellInfo.SpellID = string.Format("__formData|{0}|0x{1}", esp, fid);
                }
                else if (line.Contains("RemoveAddedForm", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else if (line.Contains("TempTome", StringComparison.OrdinalIgnoreCase) && line.Contains("GetFormFromFile", StringComparison.OrdinalIgnoreCase) && spellInfo != null)
                {
                    MatchCollection matches = rx.Matches(line);
                    string fid = matches.First().Groups["fid"].Value.Trim();
                    string esp = matches.First().Groups["esp"].Value.Trim();

                    spellInfo.TomeID = string.Format("__formData|{0}|0x{1}", esp, fid);
                    config.Spells.Add(spellInfo);
                    spellInfo = new SpellInfo();
                }
                // spellcount >= 1 to ignore all the spellresearch import stuff
                else if (spellInfo != null)
                {
                    MatchCollection mskill = rskill.Matches(line);
                    MatchCollection mcasting = rcasting.Matches(line);
                    MatchCollection mlevel = rlevel.Matches(line);
                    MatchCollection mtarget = rtarget.Matches(line);
                    MatchCollection mtechnique = rtechnique.Matches(line);
                    MatchCollection melement = relement.Matches(line);
                    if (mskill.Count > 0)
                    {
                        string match = mskill.First().Groups["skill"].Value.Trim();
                        AliasedArchetype? school = allowedArchetypes.Skills.FirstOrDefault(archetype => archetype.Name.ToLower() == match.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == match.ToLower()));
                        if (school == null)
                        {
                            Console.WriteLine($"School {match} not found");
                            continue;
                        }
                        spellInfo.School = school.Name;
                    }
                    else if (mcasting.Count > 0)
                    {
                        string match = mcasting.First().Groups["casting"].Value.Trim();
                        AliasedArchetype? castingType = allowedArchetypes.CastingTypes.FirstOrDefault(archetype => archetype.Name.ToLower() == match.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == match.ToLower()));
                        if (castingType == null)
                        {
                            Console.WriteLine($"Casting type {match} not found");
                            continue;
                        }
                        spellInfo.CastingType = castingType.Name;
                    }
                    else if (mlevel.Count > 0)
                    {
                        string match = mlevel.First().Groups["level"].Value.Trim();
                        AliasedArchetype? level = allowedArchetypes.Levels.FirstOrDefault(archetype => archetype.Name.ToLower() == match.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == match.ToLower()));
                        if (level == null)
                        {
                            Console.WriteLine($"Level {match} not found");
                            continue;
                        }
                        spellInfo.Tier = level.Name;
                    }
                    else if (mtarget.Count > 0)
                    {
                        string match = mtarget.First().Groups["target"].Value.Trim();
                        AliasedArchetype? target = allowedArchetypes.Targets.FirstOrDefault(archetype => archetype.Name.ToLower() == match.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == match.ToLower()));
                        if (target == null)
                        {
                            Console.WriteLine($"Targeting type {match} not found");
                            continue;
                        }
                        spellInfo.Targeting.Add(target);
                    }
                    else if (mtechnique.Count > 0)
                    {
                        string match = mtechnique.First().Groups["technique"].Value.Trim();
                        AliasedArchetype? technique = allowedArchetypes.Techniques.FirstOrDefault(archetype => archetype.Name.ToLower() == match.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == match.ToLower()));
                        if (technique == null)
                        {
                            Console.WriteLine($"Technique {match} not found");
                            continue;
                        }
                        spellInfo.Techniques.Add(technique);
                    }
                    else if (melement.Count > 0)
                    {
                        string match = melement.First().Groups["element"].Value.Trim();
                        AliasedArchetype? element = allowedArchetypes.Elements.FirstOrDefault(archetype => archetype.Name.ToLower() == match.ToLower() || archetype.Aliases.Any(alias => alias.ToLower() == match.ToLower()));
                        if (element == null)
                        {
                            Console.WriteLine($"Element {match} not found");
                            continue;
                        }
                        spellInfo.Techniques.Add(element);
                    }

                }
            }
            return config;
        }
    }
}