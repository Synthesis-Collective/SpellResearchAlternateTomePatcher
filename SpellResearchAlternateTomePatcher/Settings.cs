using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellResearchAlternateTomePatcher
{
    public class Settings
    {
        public LevelSettings novice = new();
        public LevelSettings apprentice = new();
        public LevelSettings adept = new();
        public LevelSettings Expert = new();
        public LevelSettings Master = new();
        public List<string> pscnames = new();
    }

    public class LevelSettings
    {
        public string font = "";
        public bool useFontColor;
        public bool useImage;
    }
}
