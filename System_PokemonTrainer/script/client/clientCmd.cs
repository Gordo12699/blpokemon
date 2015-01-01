function clientCmdPokemon_SetPokemon(%side, %id, %dex, %lvl, %hp, %xp, %name, %misc)
{
	if(!$Pokemon::Client::BattleActive)
		return;

	if(searchFields(%misc, "FEMALE") != -1)
		%gender = 1;
	else if(searchFields(%misc, "GENDERLESS") != -1)
		%gender = -1;
	else
		%gender = 0;

	if(searchFields(%misc, "SHINY") != -1)
		%shiny = true;

	if(getWordCount(%hp) > 1) //We're not going to send the max/current HP values for opponent pokemon.
	{
		%hpmax = getWord(%hp, 1);
		%hp = firstWord(%hp);
	}

	switch($Pokemon::Client::BattleType)
	{
		case 1: %id = ((%id % 2) == 1 ? 2 : 0);
		case 2: %id = %id % 3;
		default: %id = 1;
	}

	PokemonBattleGui.setPokemon(%side, %id, %dex, %lvl, %hp, %hpmax, %xp, %name, %gender, %shiny);
}

function clientCmdPokemon_SetBattleActive(%type)
{
	if($Pokemon::Client::BattleActive)
		return;

	$Pokemon::Client::BattleActive = true;
	$Pokemon::Client::BattleType = %type % 3;
}