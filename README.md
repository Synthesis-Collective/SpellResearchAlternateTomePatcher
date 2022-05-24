# JSON FORK

Intended to support JSON files used by https://www.nexusmods.com/skyrimspecialedition/mods/42381 and https://www.nexusmods.com/skyrimspecialedition/mods/48515.
Current functionality:
1. Reads the JSON file for the Mysticism patch.
2. Updates the tomes.

Upcoming functionality:
1. Read the patches of the classic JSON Patch format.
2. Allow skipping tomes by level - as it stands, this + no starting spells = you're going to need an epic quest (or Faralda and 30 Septims) to unlock your first spell. *Can* it even be done without Fari, anyone mad enough to try?
3. Restore old PSC file reading, but make it look slightly less awful. Nothing against you, Gutie, everything against Regex, necessary evil though it may be here.

Potential future functionality:
1. Export the aforementioned PSC's to JSON files that can then be eaten by the JSON Patch.
2. Update the export engine to support the Mysticism patch's formatting and fish out any changes to vanilla spells similar to it (which will probably only be relevant to Odin users) and create a copy of Kalithnir's plugin to process the newly generated patch.

# Gutie's Legacy - I touched none of this yet

# SpellResearchAlternateTomePatcher

Make sure you are using the required, or later version of mutagen, patching with mutagen versions prior to the required version will cause CTD's.

## Update - Fonts, Colors, and Images
Make sure to reload your settings if you used this patcher before.\
For each skill level, you have the option to select the following:\
- Text Font
- Whether the books will have an image
- Whether the text archetypes will be colored

The fonts can be any of:
- $SkyrimBooks
- $HandwrittenFont
- $DaedricFont
- $DragonFont
- $DragonFont
- $FalmerFont
- $MageScriptFont

See https://www.creationkit.com/index.php?title=Book for depictions. You can probably also make and addyour own fonts.

The image will be based on the first archetype found from elements/techniques, a mapping of archetypes to images can be found in the file config.json located in the synthesis patcher data folder. In order for the images to show correctly, the path must direct to a dds/png file, it must also start with the root folder textures. e.g.\
"textures/interface/exported/widgets/spellresearchbook/textures/archetype_2.dds". The set of textures in the default config file is from the mod [Spell Research - Experience Book](https://www.nexusmods.com/skyrimspecialedition/mods/28355). Note the root folder for the textures in that mod is 'interface', so in order for those to work you have to extract them from the bsa and copy the interfaces folder into a new folder called 'textures'

The archetype colors are also defined in the config file, any hexadecimal representation will do.
<!-- ![book1](https://user-images.githubusercontent.com/98627298/152105281-5a76f057-09d2-4d8e-bd68-a1c397f2629c.JPG) -->
<!-- ![book2](https://user-images.githubusercontent.com/98627298/152105316-1c2db180-470f-4e80-a54d-a1ee1534b0a6.JPG) -->
<p float="left">
<img src="https://user-images.githubusercontent.com/98627298/152105281-5a76f057-09d2-4d8e-bd68-a1c397f2629c.JPG" width="400">
<img src="https://user-images.githubusercontent.com/98627298/152105316-1c2db180-470f-4e80-a54d-a1ee1534b0a6.JPG" width="400">  
</p>

## Description
Same as the [zEdit patcher](https://www.nexusmods.com/skyrimspecialedition/mods/39301), but in Synthesis.

This patcher prevents spell books from teaching spells and instead changes the book text to describe the archetypes needed to learn them with spell research.

Only books that are 'correctly' patched with a spell research patch will be affected. If a spell book doesn't have a patch, or the patch is somehow incorrect, it will not be affected and will function as normal.

Some examples:

Flames:
A Novice spell of the destruction school, cast through immense Concentration. This spell is fired where Aimed. Elements of Fire. 

Candlelight:
A Novice spell of the Alteration school, cast by Firing and Forgetting. This spell is cast on Oneself. Elements of Construct and Light. 

Revenant:
A Adept spell of the Conjuration school, cast by Firing and Forgetting. This spell is fired where Aimed. Elements of Creature, Flesh, Soul and Undead. The technique to cast this spell is of Control, Infuse and Summoning.


## Usage
Select what formats you want for each skill level, see update notes for details.\
Add mods to be patched in the setting 'Pscnames' using the format: espname:scriptname

Pscnames example\
Skyrim.esm;_SR_ImportSkyrim.psc\
Dawnguard.esm;_SR_ImportDawnguard.psc\
Dragonborn.esm;_SR_ImportDragonborn.psc\
Inigo.esp;SpellResearchInigo.psc

The scripts need to exists in Data/Scripts/Source/*.psc

e.g.\
data/scripts/source/_SR_ImportSkyrim.psc\
data/scripts/source/_SR_ImportDawnguard.psc

## Reccommendations
Recommended to get [Spell Research Patch Compendium Redux](https://www.nexusmods.com/skyrimspecialedition/mods/61177) for Spell Research Patches.

