function clientsCanBattle(%a, %b)
{
	if(!isObject(%clientA) || !isObject(%clientB))
		return false;

	if(%clientA.getNumPartyPokemon() <= 0 || %clientB.getNumPartyPokemon() <= 0)
		return false;

	return true;
}

function Pokemon_InitiateClientBattle(%clientA, %clientB, %type, %stage)
{
	if(!clientsCanBattle(%clientA, %clientB))
		return false;

	%battle = Pokemon_InitBattle(false);

	%combA = %clientA.getPartyPokemon(0);
	%combB = %clientB.getPartyPokemon(0);

	%r = %battle.setCombatants(%combA, %combB);

	if(!%r)
	{
		%battle.delete();
		return false;
	}

	%clientA.battle = %battle;
	%clientB.battle = %battle;

	%battle.clients = %clientA SPC %clientB;

	//Forcing zero for now because only single battles are possible.
	commandToClient(%clientA, 'Pokemon_InitBattle', 0, %stage);
	commandToClient(%clientB, 'Pokemon_InitBattle', 0, %stage);

	%clientA.waitType = 0;
	%clientB.waitType = 0;

	%battle.waitingClients = 2;
}

function serverCmdPokemon_BattleReady(%this, %type)
{
	if(!isObject(%this.battle))
		return;

	switch(%type)
	{
		case 0:
			if(%this.waitType != %type)
				return;

			%battle.waitingClients--;
			%this.waitType = -1;

			if(%battle.waitingClients == 0)
			{
				//send client more info
			}
	}
}

function GameConnection::pushPokemon(%this, %pokemon, %side, %ind)
{
	if(!isPokemon(%pokemon))
		return;

	%dex = %pokemon.data.dexNum;
	%level = %pokemon.getStat("Level");
	%name = %pokemon.nickname;
	%gender = %pokemon.gender - 1;
	%shiny = %pokemon.shiny;
	%id = %pokemon.getID();

	if(%side)
	{
		%hp = %pokemon.getStat("HP"); / %pokemon.getStat("HPMax");
		%hpmax = "";
	}
	else
	{
		%hp = %pokemon.getStat("HP");
		%hpmax = %pokemon.getStat("HPMax");

		%xp = %pokemon.getStat("XP");
	}

	commandToClient(%this, 'Pokeon_SetPokemon', %side, %ind, %dex, %level, %hp, %hpmax, %xp, %name, %gender, %shiny, %id);
}