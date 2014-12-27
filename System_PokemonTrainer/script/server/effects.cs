$Pokemon::Effects::ValidModStats = "ATK DEF SPATK SPDEF ACCURACY EVASION";

function PokemonEffect_MOD(%stage, %battle, %user, %target, %targetType, %params)
{
	if(%targetType $= "USER")
		%this = %user;
	else
		%this = %target;

	%stat = getWord(%params, 0);
	%mod = getWord(%params, 1) + 0;

	if(searchWords($Pokemon::Effects::ValidModStats, %stat) == -1)
		return false;

	%new = %battle.modStage(%this, %stat, %mod);
}

function PokemonEffect_DEBUG(%stage, %battle, %user, %target, %targetType, %params)
{
	PokeDebug("DEBUG EFFECT", %battle, %user, %target);
	PokeDebug("#--BATTLE:" SPC %battle, %battle, %user, %target);
	PokeDebug("#--USER:" SPC %user, %battle, %user, %target);
	PokeDebug("#--TARGET:" SPC %target, %battle, %user, %target);
	PokeDebug("#--TARGETTYPE:" SPC %targetType, %battle, %user, %target);
	PokeDebug("#--PARAMS: " SPC %params, %battle, %user, %target);
}