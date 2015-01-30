$Pokemon::Gender0 = "Genderless";
$Pokemon::Gender1 = "Male";
$Pokemon::Gender2 = "Female";

//NATURENAME INCREASES DECREASES
$Pokemon::Nature0 = "Hardy  ";
$Pokemon::Nature1 = "Lonely Atk Def";
$Pokemon::Nature2 = "Brave Atk Speed";
$Pokemon::Nature3 = "Adamant Atk SpAtk";
$Pokemon::Nature4 = "Naughty Atk SpDef";
$Pokemon::Nature5 = "Bold Def Atk";
$Pokemon::Nature6 = "Docile  ";
$Pokemon::Nature7 = "Relaxed Def Speed";
$Pokemon::Nature8 = "Impish Def SpAtk";
$Pokemon::Nature9 = "Lax Def SpDef";
$Pokemon::Nature10 = "Timid Speed Atk";
$Pokemon::Nature11 = "Hasty Speed Def";
$Pokemon::Nature12 = "Serious  ";
$Pokemon::Nature13 = "Jolly Speed SpAtk";
$Pokemon::Nature14 = "Naive Speed SpDef";
$Pokemon::Nature15 = "Modest SpAtk Atk";
$Pokemon::Nature16 = "Mild SpAtk Def";
$Pokemon::Nature17 = "Quiet SpAtk Speed";
$Pokemon::Nature18 = "Bashful  ";
$Pokemon::Nature19 = "Rash SpAtk SpDef";
$Pokemon::Nature20 = "Calm SpDef Atk";
$Pokemon::Nature21 = "Gentle SpDef Def";
$Pokemon::Nature22 = "Sassy SpDef Speed";
$Pokemon::Nature23 = "Careful SpDef SpAtk";
$Pokemon::Nature24 = "Quirky  ";

$Pokemon::ShinyThreshold = 8;

$Pokemon::IV0 = "HP";
$Pokemon::IV1 = "Atk";
$Pokemon::IV2 = "Def";
$Pokemon::IV3 = "Speed";
$Pokemon::IV4 = "SpAtk";
$Pokemon::IV5 = "SpDef";

function Pokemon_New(%data, %trainerBL_ID, %owner, %name)
{
	if(%data.class !$= "PokemonData")
		return -1;

	%this = new ScriptObject()
			{
				class = strReplace(%data.getName(), "Data", "");
				superClass = "Pokemon";
				data = %data;

				trainerID = (%trainerBL_ID !$= "" ? solveTrainerID(%trainerBL_ID) : "");
				secretID = (%trainerBL_ID !$= "" ? solveSecretID(%trainerBL_ID) : "");
				owner = (%owner >= 0 ? -1 : %owner + 0);
				nickname = (%name $= "" ? %data.name : %name);
				species = %data.name;

				move0 = -1;
				move1 = -1;
				move2 = -1;
				move3 = -1;
				pp0 = -1;
				pp1 = -1;
				pp2 = -1;
				pp3 = -1;
				ppMax0 = -1;
				ppMax1 = -1;
				ppMax2 = -1;
				ppMax3 = -1;

				item = -1;
				pokeball = -1;

				statLevel = 1;
				statMaxHP = 1;
				statHP = 1;
				statAtk = -1;
				statDef = -1;
				statSpAtk = -1;
				statSpDef = -1;
				statSpeed = -1;
				statEff = -1;
				statXP = 0;

				ivAtk = 0;
				ivDef = 0;
				ivSpAtk = 0;
				ivSpDef = 0;
				ivSpeed = 0;
				ivHP = 0;

				evAtk = 0;
				evDef = 0;
				evSpAtk = 0;
				evSpDef = 0;
				evSpeed = 0;
				evHP = 0;

				personality = mAbs(getRandom(0xFFFFFFFF, 0x80000000) | 0) | 0; //we're gonna make do even if torque doesn't want to
				gender = 0;
				ability = -1;
				nature = -1;
				shiny = false;
			};
	MissionCleanup.add(%this);

	%this.solvePersonality();
	%this.generateIVs();
	%this.setStatsForLevel(1);

	return %this;
}

function isPokemon(%obj)
{
	if(!isObject(%obj))
		return false;

	if(%obj.getClassName() !$= "ScriptObject")
		return false;

	if(%obj.superClass !$= "Pokemon")
		return false;

	return true;
}

function Pokemon::onAdd(%this)
{
	if(!isObject(PokemonSet))
		new SimSet(PokemonSet);
	PokemonSet.add(%this);
}

function Pokemon::solvePersonality(%this)
{
	if(%this.data.genderRatio != -1)
	{
		%pGender = %this.personality % 128;
		%genPerc = %pGender / 128;
		if(%genPerc <= %this.data.genderRatio)
			%this.gender = 1;
		else
			%this.gender = 2;
	}
	else
		%this.gender = 0;

	if(getFieldCount(%this.data.ability) == 1)
		%this.ability = %this.data.ability;
	else
		%this.ability = getField(%this.data.ability, %this.personality % 2);

	%this.nature = %this.personality % 25;

	if(%this.trainerID $= "")
		%tID = $Pokemon::GenericTrainerID;
	else
		%tID = %this.trainerID;

	if(%this.secretID $= "")
		%sID = $Pokemon::GenericSecretID;
	else
		%sID = %this.secretID;

	%p1 = mFloor(%this.personality / 32768);
	%p2 = %this.personality % 32768;
	%s = %tID ^ %sID ^ %p1 ^ %p2;
	if(%s < $Pokemon::ShinyThreshold)
		%this.shiny = true;
}

function Pokemon::generateIVs(%this)
{
	%highest = 0;
	for(%i = 0; %i < 6; %i++)
	{
		%v = %this.iv[$Pokemon::IV[%i]] = getRandom(31);
		if(%v > %highest)
		{
			%highest = %v;
			%highestIndex = %i;
		}
	}

	for(%i = %this.personality % 6; true; %i = (%i+1) % 6)
	{
		if(%highestIndex == %i)
		{
			%this.characteristic = %i;
			break;
		}
	}
}

function Pokemon::setStatsForLevel(%this, %level)
{
	%hpPerc = %this.statHP / %this.statMaxHP;
	%this.statMaxHP = mFloor(((%this.ivHP + 2 * %this.data.baseHP + %this.evHP / 4 + 100) * %level) / 100 + 10);
	%this.statHP = mFloor(%this.statMaxHP * %hpPerc);
	if(%this.statHP > %this.statMaxHP)
		%this.statHP = %this.statMaxHP;
	else if(%this.statHP < 0)
		%this.statHP = 0;

	for(%i = 1; %i < 6; %i++)
	{
		%type = $Pokemon::IV[%i];
		%this.stat[%type] = (((%this.iv[%type] + 2 * %this.data.base[%type] + %this.ev[%type] / 4) * %level) / 100 + 5);

		%nature = $Pokemon::Nature[%this.nature];
		%eff = searchWords(%nature, %type) - 1;
		if(%eff < 0)
			%natureEff = 1;
		else if(%eff == 0)
			%natureEff = 1.1;
		else if(%eff == 1)
			%natureEff = 0.9;

		%this.stat[%type] *= %natureEff;
		%this.stat[%type] = mFloor(%this.stat[%type]);
	}

	%this.statLevel = %level;
}

function Pokemon::levelUp(%this)
{
	if((%lvl = %this.getStat("Level")) >= 100)
		return;
	if(%lvl < 1)
		%lvl = 1;

	%this.setStatsForLevel(%lvl + 1);
}

function Pokemon::getStat(%this, %stat)
{
	return %this.stat[%stat];
}

function Pokemon::getHP(%this)
{
	return %this.getStat("HP");
}

function Pokemon::getHPPerc(%this)
{
	return %this.getStat("HP") / %this.getStat("MaxHP");
}

function Pokemon::setHP(%this, %val)
{
	%oval = %this.getStat("HP");
	if(%val < 0)
		%val = 0;
	else if(%val > %v = %this.getStat("MaxHP"))
		%val = %v;

	PokeDebug("+Pokemon " @ %this @ " set " @ %oval @ "-(" @ %val-%oval @ ")->" @ %val, %this);

	return %this.statHP = mFloor(%val);
}

function Pokemon::setHPPerc(%this, %val)
{
	if(%val > 1)
		%val = 1;
	else if(%val < 0)
		%val = 0;

	return %this.setHP(mFloor(%this.getStat("MaxHP") * %val));
}

function Pokemon::modHP(%this, %mod)
{
	return %this.setHP(mFloor(eval("return %this.statHP" @ %mod @ ";")));
}

function Pokemon::modHPPerc(%this, %mod)
{
	%perc = %this.getHPPerc();
	%perc = eval("return %perc" @ %mod @ ";");

	if(%perc < 0)
		%perc = 0;
	if(%perc > 1)
		%perc = 1;

	return %this.setHPPerc(%perc);
}

function Pokemon::setMove(%this, %slot, %move)
{
	if(%move.class !$= "PokemonMove")
		return false;

	%slot = %slot % 4;
	%this.move[%slot] = %move;
	%this.pp[%slot] = %move.pp;
	%this.ppMax[%slot] = %move.pp;

	return true;
}

function Pokemon::getMove(%this, %slot)
{
	%slot = %slot % 4;
	if(!isObject(%this.move[%slot]))
		return -1;
	return %this.move[%slot];
}

function Pokemon::getPP(%this, %slot)
{
	%slot = %slot % 4;

	if(!isObject(%this.move[%slot]))
		return -1;

	return %this.pp[%slot];
}

function Pokemon::setPP(%this, %slot, %val)
{
	%slot = %slot % 4;

	if(!isObject(%this.move[%slot]))
		return -1;

	if(%val > %this.ppMax[%slot])
		%val = %this.ppMax[%slot];
	else if(%val < 0)
		%val = 0;

	return %this.pp[%slot] = %val;
}

function Pokemon::getMaxPP(%this, %slot)
{
	%slot = %slot % 4;

	if(!isObject(%this.move[%slot]))
		return -1;

	return %this.ppMax[%slot];
}

function Pokemon::applyPPUpLevel(%this, %slot, %val)
{
	%val = %val % 4;
	%slot = %slot % 4;

	if(!isObject(%this.move[%slot]))
		return -1;

	%this.ppMax[%slot] = %this.move[%slot].getPPLevel(%val);

	return %this.ppMax[%slot];
}

function Pokemon::removeMove(%this, %slot)
{
	%slot = %slot % 4;

	if(!isObject(%this.move[%slot]))
		return false;

	%this.move[%slot] = -1;
	%this.pp[%slot] = -1;
	%this.ppMax[%slot] = -1;

	return true;
}