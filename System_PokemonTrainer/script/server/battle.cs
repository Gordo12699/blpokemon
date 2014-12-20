//This is a table of values which dictates the modification to a pokemon's stat when affected by moves such as growl, leer, swords dance, etc.
%mods = -7;
$Pokemon::Battle::ModTable[%mods++] = 0.25; //-6
$Pokemon::Battle::ModTable[%mods++] = 0.28; //-5
$Pokemon::Battle::ModTable[%mods++] = 0.33; //-4
$Pokemon::Battle::ModTable[%mods++] = 0.40; //-3
$Pokemon::Battle::ModTable[%mods++] = 0.50; //-2
$Pokemon::Battle::ModTable[%mods++] = 0.66; //-1
$Pokemon::Battle::ModTable[%mods++] = 1;	//0
$Pokemon::Battle::ModTable[%mods++] = 1.5;  //+1
$Pokemon::Battle::ModTable[%mods++] = 2;	//+2
$Pokemon::Battle::ModTable[%mods++] = 2.5;	//+3
$Pokemon::Battle::ModTable[%mods++] = 3;	//+4
$Pokemon::Battle::ModTable[%mods++] = 3.5;	//+5
$Pokemon::Battle::ModTable[%mods++] = 4;	//+6
$Pokemon::Battle::ModTableMin = -6;
$Pokemon::Battle::ModTableMax = 6;

function PKMN_SolveDamage(%attackerLvl, %attackerAttk, %attackPwr, %defendDef, %sameMod, %typeMod)
{
	//Follows formula as detailed at this web page: https://www.math.miami.edu/~jam/azure/compendium/battdam.htm
	//Note that this is the end formula; modifications must be made to the individual inputs in real battle for things like crits, buffs, etc.
	%r1 = (2 * %attackerLvl) / 5 + 2;
	%r2 = %r1 * %attackerAttk * %attkPwr;
	%r3 = %r2 / %defendDef;
	%r4 = (%r3 / 50) + 2;
	%r5 = %r4 * %sameMod;
	%r6 = %r5 * %typeMod / 10;
	%r7 = %r6 * getRandom(217, 255);
	%r8 = %r7 / 255;

	return %r8;
}

function PKMN_InitBattle()
{
	if(!isObject(PokemonBattleGroup))
	{
		new SimGroup(PokemonBattleGroup);
		MissionCleanup.add(PokemonBattleGroup);
	}

	%this = new ScriptObject(PokemonBattle)
			{
				combatants = -1;
				type = 0;
			};
	PokemonBattleGroup.add(%this);
	MissionCleanup.add(PokemonBattleGroup);
}

function PokemonBattle::setCombatants(%this, %teamA, %teamB)
{

}