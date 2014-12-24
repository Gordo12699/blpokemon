function PokemonMove::getMaxPP(%this)
{
	if(%this.pp == 1)
		return 1;

	%add = %this.pp * 0.6;

	return mFloor(%this.pp + %add);
}

function PokemonMove::getPPLevel(%this, %level)
{
	if(%this.pp == 1)
		return 1;

	%level = %level % 4;

	%add = %this.pp * 0.2 * %level;

	%val = mFloor(%this.pp + %add);
	if(%val > %this.getMaxPP()) //shouldn't happen ever logically but w/e
		%val = %this.getMaxPP(); 

	return %val;
}

function PokemonMove::getEffectCt(%this)
{
	return getFieldCount(%this.effect);
}

function PokemonMove::getEffect(%this, %i)
{
	return getField(%this.effect, %i);
}

function PokemonMove::searchEffects(%this, %key)
{
	%ct = %this.getEffectCt();
	for(%i = 0; %i < %ct; %i++)
	{
		%eff = %this.getEffectKeyword(%i);
		if(%eff $= %key)
			return %i;
	}
	return -1;
}

function PokemonMove::getEffectTarget(%this, %i)
{
	%eff = %this.getEffect(%i);
	%targ = firstWord(%eff);
	return %targ;
}

function PokemonMove::getEffectKeyword(%this, %i)
{
	%eff = %this.getEffect(%i);
	%key = getWord(%eff, 1);
	return %key;
}

function PokemonMove::getEffectParameters(%this, %i)
{
	%eff = %this.getEffect(%i);
	%key = getWords(%eff, 2, getWordCount(%eff) - 1);
	return %key;
}

function PokemonMove::getEffectParameter(%this, %i, %j)
{
	%params = %this.getEffectParameters(%i);
	return getWord(%params, %j);
}

function PokemonMove::execute(%this, %battle, %user, %target)
{
	if(!isObject(%battle) || %battle.getName() !$= "PokemonBattle")
		return false;
	if(!isObject(%user) || %user.superClass !$= "Pokemon" || !isObject(%target) || %target.superClass !$= "Pokemon")
		return false;

	PokeDebug("&MOVE" SPC %this SPC "EXECUTED IN BATTLE" SPC %battle, %battle, %user, %target, %this);
	PokeDebug("&--USER:" SPC %user, %battle, %user, %target, %this);
	PokeDebug("&--TARGET:" SPC %target, %battle, %user, %target, %this);

	%acc = %this.accuracy;
	%accStage = %battle.getModStage(%user, "Accuracy");
	%evStage = %battle.getModStage(%target, "Evasion");
	%stage = %accStage - %evStage;
	if(%stage > $Pokemon::Battle::ModTableMax)
		%stage = $Pokemon::Battle::ModTableMax;
	else if(%stage < $Pokemon::Battle::ModTableMin)
		%stage = $Pokemon::Battle::ModTableMin;

	PokeDebug("&--ACCURACY CHECK", %battle, %user, %target, %this);
	PokeDebug("   &--BASE ACCURACY:" SPC %acc, %battle, %user, %target, %this);
	PokeDebug("   &--ACC. STAGE:" SPC %accStage, %battle, %user, %target, %this);
	PokeDebug("   &--EVSN. STAGE:" SPC %evStage, %battle, %user, %target, %this);
	PokeDebug("   &--TOTAL STAGE:" SPC %stage, %battle, %user, %target, %this);

	%acc *= $Pokemon::Battle::ModTableAE[%stage];
	if(%acc > 1)
		%acc = 1;

	PokeDebug("   &--END ACCURACY:" SPC %acc, %battle, %user, %target, %this);

	if((%arand = getRandom()) > %acc)
		return -1;

	PokeDebug("   &--GENERATED HIT" SPC %arand, %battle, %user, %target, %this);

	%ect = %this.getEffectCt();
	for(%i = 0; %i < %ect; %i++)
	{
		%eff = %this.getEffectKeyword(%i);
		if(isFunction(%f = "PokemonEffect_Pre_" @ %eff))
		{
			PokeDebug("&--CALLING PRE-MOVE EFFECT FOR" SPC %eff, %battle, %user, %target, %this);
			call(%f, %battle, %user, %target, %this.getEffectTarget(%i), %this.getEffectParameters(%i));
		}
	}

	if(%this.power > 0)
	{
		PokeDebug("&--CALCULATING DAMAGE", %battle, %user, %target, %this);
		%mod = 1;
		%crit = false;
		%c = %battle.getCritStage(%user);
		%cprob = $Pokemon::Battle::CritTable[%c];
		if(getRandom() < %cprob)
		{
			PokeDebug("   &--CRIT; MOD*2, -ATK MODS IGNORED, +DEF MODS IGNORED", %battle, %user, %target, %this);
			%mod *= 2;
			%crit = true;
		}

		%tmod = Pokemon_GetTypeMod(%this.type, %target.data.type1, %target.data.type2);
		%mod *= %tmod;

		PokeDebug("   &--TYPE MOD:" SPC %tmod, %battle, %user, %target, %this);

		if(%this.type $= %user.data.type1 || %this.type $= %user.data.type2)
		{
			PokeDebug("   &--STAB BONUS; MOD*1.5", %battle, %user, %target, %this);
			%mod *= 1.5;
		}

		%sp = (%this.class $= "Special" ? "Sp" : "");
		%attStage = %battle.getModStage(%user,  %sp @ "Atk", 0, %crit);
		%defStage = %battle.getModStage(%user, %sp @ "Atk", %crit, 0);

		PokeDebug("   &--ATK STAGE:" SPC %attStage, %battle, %user, %target, %this);
		PokeDebug("   &--DEF STAGE:" SPC %defStage, %battle, %user, %target, %this);

		%att = %user.getStat(%sp @ "Atk") * $Pokemon::Battle::ModTable[%attStage];
		%def = %user.getStat(%sp @ "Def") * $Pokemon::Battle::ModTable[%defStage];

		PokeDebug("   &--ATK END:" SPC %att, %battle, %user, %target, %this);
		PokeDebug("   &--DEF END:" SPC %def, %battle, %user, %target, %this);

		%dam = Pokemon_SolveDamage(%user.getStat("Level"), %att, %this.power * (%user.getStat("Eff") $= "BURN" ? 0.5 : 1), %def, %mod);

		PokeDebug("   &--TOTAL DAMAGE:" SPC %dam, %battle, %user, %target, %this);

		%target.modHP(-%dam);
	}

	for(%i = 0; %i < %ect; %i++)
	{
		%eff = %this.getEffectKeyword(%i);
		if(isFunction(%f = "PokemonEffect_Post_" @ %eff))
		{
			PokeDebug("&--CALLING POST-MOVE EFFECT FOR" SPC %eff, %battle, %user, %target, %this);
			call(%f, %battle, %user, %target, %this.getEffectTarget(%i), %this.getEffectParameters(%i));
		}
	}
}
