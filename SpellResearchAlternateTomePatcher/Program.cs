using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;

using System.IO;
using System.Text.RegularExpressions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json.Linq;

namespace SpellResearchAlternateTomePatcher
{

    public class Program
    {
        static Lazy<Settings> settings = null!;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "alteredtomespatch.esp")
                .SetAutogeneratedSettings(
                    nickname:"Altered Tomes Patcher Settings",
                    path: "alteredtomesettings.json",
                    out settings)
                .Run(args);

        }


        private static IBookGetter? fixformidandresolve(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, ModKey mk, string fid)
        {

            var fkey = fixformid(fid, mk);

            var bookLink = new FormLink<IBookGetter>(fkey);


            // if things are normal
            if (bookLink.TryResolve(state.LinkCache, out var bookRecord))
            {
                return bookRecord;
            }
            
            //if things are messed up 
            else
            {
                Console.WriteLine("WARNING: problem with {0}", fkey);
                Console.WriteLine("mk: {0}", mk);

                var mod = state.LoadOrder.TryGetValue(mk);
                var masters = mod?.Mod?.ModHeader.MasterReferences ?? new List<MasterReference>();

                foreach (var master in masters)
                {
                    var mfkey = fixformid(fid, master.Master);
                    Console.WriteLine("Trying {0} with {1}", fkey, mfkey);
                    var mbookLink = new FormLink<IBookGetter>(mfkey);
                    if (mbookLink.TryResolve(state.LinkCache, out var masterBookRecord))
                    {
                        Console.WriteLine("Fixed {0} with {1}", fkey, mfkey);
                        return masterBookRecord;
                    }

                }
            }

            Console.WriteLine("ERROR: Could not fix {0}", fkey);
            return null;
        }

        // fixes some things with formids in spellresearch scripts and returns a formkey
        private static FormKey fixformid(string fid, ModKey mk) {

            FormKey formKey;
            var fkeystr = fid + ":" + mk.FileName;
            
            // hex value and too many zeroes
            if (fid.Contains("0x", StringComparison.OrdinalIgnoreCase))
            {
                // first try to convert directly
                if (FormKey.TryFactory(fkeystr, out formKey))
                {
                    return formKey;
                }

                // handle ESL's
                if (fid.Substring(0, 2).Equals("FE")) {
                    fid = "00000" + fid.Substring(5, 3);
                }
                else {
                    // to make str processing easier
                    fid = fid.Replace("0x", "00").PadLeft(6, '0');
                    
                    // hardcoded load order for some reason, (papyrus is fine with this apparently)
                    if (fid.Length == 8 && !fid.Substring(0, 2).Equals("00")) {
                        fid = "00" + fid.Substring(2, 6);
                    }
                    // needs to be 8 digits for mutagen
                    if (fid.Length > 6) {
                        fid = fid.Substring(fid.Length - 6, 6);
                    }
                }
                fkeystr = fid + ":" + mk.FileName;
                return FormKey.Factory(fkeystr);
            }
            // decimal value
            else
            {
                int num = Convert.ToInt32(fid, 10);
                string h = num.ToString("X");
                if (!h.Contains("0x", StringComparison.OrdinalIgnoreCase))
                {
                    h = "0x" + h;
                }
                return fixformid(h, mk);

            }
        }

        // removes 'Spell Tome' elements from description name
        public static string fix_name(IBookGetter book, Regex rnamefix)
        {
            var n = book?.Name?.ToString() ?? "";
            MatchCollection mnamefix = rnamefix.Matches(n);
            if (mnamefix.Count > 0)
            {
                n = mnamefix.First().Groups["tomename"].Value.Trim();
            }

            return n;
        }

        // Creates a string description of a spell given its archetypes
        private static string process_text(Dictionary<string, dynamic> spell, JObject config, LevelSettings s)
        {


            string strbuilder = "";
            strbuilder += "A " + spell["level"].ToLower();
            strbuilder += " spell of the ";
            if (s.useFontColor) {
                strbuilder += "<font color='" + (config["Colors"]?[spell["skill"]] ?? "#000000").ToString() + "'>";
            }
            strbuilder += spell["skill"].ToLower();
            if (s.useFontColor)
            {
                strbuilder += "</font><font color='#000000'>";
            }
            strbuilder += " school, cast ";
            if (spell["casting"].Equals("Concentration"))
                strbuilder += "through immense concentration. ";
            else if (spell["casting"].Equals("FireForget"))
                strbuilder += "by firing and forgetting. ";

            for (int i = 0; i < spell["target"].Count; i++) {
                string a = spell["target"][i];
                if (a.Equals("Actor"))
                    strbuilder += "This spell is fired where aimed. "; 
                else if (a.Equals("AOE"))
                    strbuilder += "This spell has an area of effect. ";
                else if (a.Equals("Location"))
                    strbuilder += "This spell is cast in a specific location. ";
                else if (a.Equals("Self"))
                    strbuilder += "This spell is cast on oneself. ";
            }

            if (spell["element"].Count > 0) {
                strbuilder += "Elements of ";
                if (s.useFontColor) { strbuilder += "</font>"; }
                int idx=0;
                foreach (string e in spell["element"]) {
                    if (idx == 0) 
                    {
                        if (s.useFontColor) { strbuilder += "<font color='" + (config["Colors"]?[e] ?? "#000000").ToString() + "'>"; }
                        strbuilder += e.ToLower();
                    }
                    else
                    {
                        if (idx == spell["element"].Count - 1)
                            strbuilder += " and "; 
                        else
                            strbuilder += ", ";

                        if (s.useFontColor) {strbuilder += "<font color='" + (config["Colors"]?[e] ?? "#000000").ToString() + "'>";}
                        strbuilder += e.ToLower();
                    }
                    if (s.useFontColor) { strbuilder += "</font>"; }
                    idx+=1;
                }
                strbuilder += ". ";
            }

            if (spell["technique"].Count > 0) {
                if (spell["element"].Count > 0 && s.useFontColor) {strbuilder += "<font color='#000000'>";}
                strbuilder += "The technique to cast this spell is of ";
                if (s.useFontColor) { strbuilder += "</font>"; }
                int idx=0;
                foreach (string t in spell["technique"]) {
                    if (idx == 0)
                    {
                        if (s.useFontColor) { strbuilder += "<font color='" + (config["Colors"]?[t] ?? "#000000").ToString() + "'>"; }
                        strbuilder += t.ToLower();
                    }
                    else
                    {
                        if (idx == spell["technique"].Count - 1)
                            strbuilder += " and ";
                        else
                            strbuilder += ", ";
                        if (s.useFontColor) { strbuilder += "<font color='" + (config["Colors"]?[t] ?? "#000000").ToString() + "'>"; }
                        strbuilder += t.ToLower();
                    }
                    if (s.useFontColor) { strbuilder += "</font>"; }
                    idx+=1;
                }
                strbuilder += ".";
            }

            return strbuilder;
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {


            string extraSettingsPath = Path.Combine(state.ExtraSettingsDataPath, "config.json");
            if (!File.Exists(extraSettingsPath)) throw new ArgumentException($"Required settings missing! {extraSettingsPath}");
            var configText = File.ReadAllText(extraSettingsPath);
            var config = JObject.Parse(configText);


            foreach (string modpsc in settings.Value.pscnames)
            {
                if (modpsc.Trim().Equals("")) {continue;}
                
                Console.WriteLine(modpsc);

                string modname = modpsc.Split(";")[0].Trim();
                string pscname = modpsc.Split(";")[1].Trim();

                var modKey = ModKey.FromFileName(modname);
                if (!state.LoadOrder.ContainsKey(modKey))
                {
                    Console.WriteLine("WARNING: Mod not Found: {0}", modname);
                    continue;
                }


                var scriptPath = Path.Combine(state.DataFolderPath, "scripts", "source", pscname);
                if (!File.Exists(scriptPath)) {
                    Console.WriteLine("WARNING: Script not Found: {0}", scriptPath);
                    continue;

                }

                string[] lines = File.ReadAllLines(scriptPath);
                int spellcount = 0;
                Dictionary<string, dynamic> archetypemap = new Dictionary<string, dynamic>();
                Regex rx = new Regex("^.*\\(\\s*(?<fid>(0x)?[a-fA-F0-9]+),\\s\"(?<esp>.*\\.es[pml])\".*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                Regex rnamefix = new Regex("^.+\\s+(Tome)\\:?(?<tomename>.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


                Regex rskill = new Regex("^.*_SR_ListSpellsSkill(?<skill>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                Regex rcasting = new Regex("^.*_SR_ListSpellsCasting(?<casting>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                Regex rlevel = new Regex("^.*_SR_ListAllSpells[1-5](?<level>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                Regex rtarget = new Regex("^.*_SR_ListSpellsTarget(?<target>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                Regex rtechnique = new Regex("^.*_SR_ListSpellsTechnique(?<technique>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                Regex relement = new Regex("^.*_SR_ListSpellsElement(?<element>[A-Za-z]+).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                // parse the psc
                foreach (string line in lines)
                {
                    // start of spell in psc
                    if (line.Contains("TempSpell", StringComparison.OrdinalIgnoreCase) && line.Contains("GetFormFromFile", StringComparison.OrdinalIgnoreCase))
                    {
                        spellcount++;
                        archetypemap = new Dictionary<string, dynamic>();
                        archetypemap["skill"] = "";
                        archetypemap["casting"] = "";
                        archetypemap["level"] = "";
                        archetypemap["target"] = new List<string>();
                        archetypemap["technique"] = new List<string>();
                        archetypemap["element"] = new List<string>();
                    }
                    // ignore removeaddedform
                    else if (line.Contains("RemoveAddedForm", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    // end of spell
                    else if (line.Contains("TempTome", StringComparison.OrdinalIgnoreCase) && line.Contains("GetFormFromFile", StringComparison.OrdinalIgnoreCase) && spellcount >= 1)
                    {
                        MatchCollection matches = rx.Matches(line);
                        string fid = matches.First().Groups["fid"].Value.Trim();
                        string esp = matches.First().Groups["esp"].Value.Trim();

                        // get the modkey from the esp in the psc
                        var good_modkey = ModKey.TryFromFileName(esp, out modKey);
                        if (!good_modkey)
                        {
                            Console.WriteLine("ERROR: could not determine mod: {0} for {1}, skipping", esp, fid);
                            continue;
                        }
                        // get formkey for book and get text
                        //FormKey fkey = fixformid(fid, modKey);
                        //FormKey fkey = fixformidandresolve(state, modKey, fid);
                        IBookGetter? bookRecord = fixformidandresolve(state, modKey, fid);

                        //Console.WriteLine(fkey.ToString());
                        //var bookLink = new FormLink<IBookGetter>(fkey);

                        //if (bookLink.TryResolve(state.LinkCache, out var bookRecord))
                        if (bookRecord != null)
                        {

                            LevelSettings s;
                            if (archetypemap["level"].Equals("Novice")) { s = settings.Value.novice; }
                            else if (archetypemap["level"].Equals("Apprentice")) { s = settings.Value.apprentice; }
                            else if (archetypemap["level"].Equals("Adept")) { s = settings.Value.adept; }
                            else if (archetypemap["level"].Equals("Expert")) { s = settings.Value.Expert; }
                            else if (archetypemap["level"].Equals("Master")) { s = settings.Value.Master; }
                            else {
                                s = new();
                            }
                            string desc = process_text(archetypemap, config, s).Trim();

                            var font = s.font;
                            //var font = config["Fonts"]?[archetypemap["level"]].ToString() ?? "$HandwrittenFont";
                            var name = fix_name(bookRecord, rnamefix);

                            if (font.Equals("$FalmerFont") || font.Equals("$DragonFont") || font.Equals("$MageScriptFont")) {
                                
                                char[] tagsep = new char[] {'<','>','#','0','1','2','3','4','5','6','7','8','9'};
                                char[] separators = new char[] {'!','@','$','%','^','&','*','(',')','{','}','[',']','-','_','+',':','"',';',',','.','?','~'};

                                desc = desc.ToUpper();
                                name = name.ToUpper();

                                string[] tempdesc = desc.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                string[] tempname = name.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                desc = String.Join("", tempdesc);
                                name = String.Join("", tempname);
                            }

                            string PREAMBLE = "<br><br><p align=\"center\"><font face='" + font + "'><font size='40'></font>";
                            string imgpath = "";
                            string img = "";

                            if (s.useImage)
                            {
                                if (archetypemap["element"].Count > 0)
                                {
                                    imgpath = config["Images"]?[archetypemap["element"][0]] ?? "";
                                }
                                else if (archetypemap["technique"].Count > 0)
                                {
                                    imgpath = config["Images"]?[archetypemap["technique"][0]] ?? "";
                                }

                                if (!imgpath.Equals(""))
                                {
                                    img = "<br><br><img src='img://" + imgpath + "' height='296' width='296'>";
                                }
                            }
                            string PAGE = "<br><br></p>[pagebreak]<br><br><p align=\"left\"><font face='" + font + "'><font size='40'></font>";
                            string POST = "</font></p>";

                            var btext = PREAMBLE + name + img + PAGE + desc + POST;
                            btext = Regex.Replace(btext, @"FONT\s*(COLOR)*", m => m.Value.ToLower());
                            Console.WriteLine("DESC: {0}", btext);

                            var bookOverride = state.PatchMod.Books.GetOrAddAsOverride(bookRecord);
                            bookOverride.Teaches = new BookTeachesNothing();
                            bookOverride.BookText = btext;

                        }
                        else {
                            Console.WriteLine("ERROR: Could Not Resolve {0}", fid);
                        } 

                    }
                    // spellcount >= 1 to ignore all the spellresearch import stuff
                    else if (spellcount >= 1)
                    {
                        MatchCollection mskill = rskill.Matches(line);
                        MatchCollection mcasting = rcasting.Matches(line);
                        MatchCollection mlevel = rlevel.Matches(line);
                        MatchCollection mtarget = rtarget.Matches(line);
                        MatchCollection mtechnique = rtechnique.Matches(line);
                        MatchCollection melement = relement.Matches(line);
                        if (mskill.Count > 0)
                        {
                            archetypemap["skill"] = mskill.First().Groups["skill"].Value.Trim();
                        }
                        else if (mcasting.Count > 0)
                        {
                            archetypemap["casting"] = mcasting.First().Groups["casting"].Value.Trim();
                        }
                        else if (mlevel.Count > 0)
                        {
                            archetypemap["level"] = mlevel.First().Groups["level"].Value.Trim();
                        }
                        else if (mtarget.Count > 0)
                        {
                            archetypemap["target"].Add(mtarget.First().Groups["target"].Value.Trim());
                        }
                        else if (mtechnique.Count > 0)
                        {
                            archetypemap["technique"].Add(mtechnique.First().Groups["technique"].Value.Trim());
                        }
                        else if (melement.Count > 0)
                        {
                            archetypemap["element"].Add(melement.First().Groups["element"].Value.Trim());
                        }

                    }

                }

            }

        }

    }
}
