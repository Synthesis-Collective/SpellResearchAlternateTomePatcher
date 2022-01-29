# SpellResearchAlternateTomePatcher
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
Usage:
Add mods to be patched in the settings using the format: espname:scriptname

e.g.
Skyrim.esm;_SR_ImportSkyrim.psc
Dawnguard.esm;_SR_ImportDawnguard.psc
Dragonborn.esm;_SR_ImportDragonborn.psc
Inigo.esp;SpellResearchInigo.psc

The scripts need to exists in Data/Scripts/Source/*.psc

e.g.
data/scripts/source/_SR_ImportSkyrim.psc
data/scripts/source/_SR_ImportDawnguard.psc

## Reccommendations
Recommended to get [Spell Research Patch Compendium Redux](https://www.nexusmods.com/skyrimspecialedition/mods/61177) for Spell Research Patches.

