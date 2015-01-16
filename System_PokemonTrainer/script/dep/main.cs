function PokeDebug(%msg, %o0, %o1, %o2, %o3, %o4, %o5)
{
	%msg = $PokeDebug::Indent @ %msg;
	%highest = 0;
	for(%i = 0; %i < 6; %i++)
	{
		if(!isObject(%o[%i]))
			continue;

		if(%o[%i].pokeDebug > %highest)
			%highest = %highest | %o[%i].pokeDebug;
	}

	switch($PokemonDebug | %highest)
	{
		case 0: return;
		case 1: echo(%msg);
		case 2:	talk(%msg);
		case 3: 
			echo(%msg);
			talk(%msg);
		default:
			warn(%msg);
	}
}

exec("./jettison.cs");
exec("./string.cs");

$Pokemon::Exec::Dependency = true;