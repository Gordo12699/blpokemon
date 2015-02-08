function clientCanBattle(%obj)
{
	if(!isObject(%obj))
		return false;

	if(%obj.getNumPartyPokemon() <= 0)
		return false;

	return true;
}

function clientsCanBattle(%a, %b)
{
	if(!isObject(%clientA) || !isObject(%clientB) || nameToID(%clientA) == nameToID(%clientB))
		return false;

	if(!clientCanBattle(%a) || !clientCanBattle(%b))
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

	// %clientA.battle = %battle;
	// %clientB.battle = %battle;

	// %battle.clients = %clientA SPC %clientB;

	//Forcing zero for now because only single battles are possible.
	// commandToClient(%clientA, 'Pokemon_InitBattle', 0, %stage);
	// commandToClient(%clientB, 'Pokemon_InitBattle', 0, %stage);
	%this.commandToClients('Pokemon_InitBattle', 0, %stage);

	%clientA.waitType = 0;
	%clientB.waitType = 0;

	%battle.waitingClients = 2;

	return %battle;
}

function Pokemon_InitiateWildBattle(%client, %pokemon, %type, %stage)
{
	if(!clientCanBattle(%client) || !isPokemon(%pokemon))
		return false;

	%battle = Pokemon_InitBattle(true);

	%combA = %client.getPartyPokemon(0);
	%combB = %pokemon;

	%r = %battle.setCombatants(%combA, %combB);

	if(!%r)
	{
		%battle.delete();
		return false;
	}

	// %clientA.battle = %battle;
	// %clientB.battle = %battle;

	// %battle.clients = %clientA SPC %clientB;

	//Forcing zero for now because only single battles are possible.
	// commandToClient(%client, 'Pokemon_InitBattle', 0, %stage);
	%battle.commandToClients('Pokemon_InitBattle', 0, %stage);

	%client.waitType = 0;

	%battle.waitingClients = 1;

	return %battle;
}

function serverCmdPokemon_BattleReady(%this, %type)
{
	if(!isObject(%this.battle))
		return;

	%battle = %this.battle;
	switch(%type)
	{
		case 0:
			if(%this.waitType != %type)
				return;

			%battle.waitingClients--;
			%this.waitType = -1;

			if(%battle.waitingClients == 0)
			{
				%battle.pushCombatants();
				%battle.schedule(100, updateClientDisplays);
				%battle.schedule(500, Begin);
			}

		case 2:
			if(%this.waitType != %type)
				return;

			%battle.waitingClients--;
			%this.waitType = -1;

			if(%battle.waitingClients == 0)
			{
				%battle.pushCombatants();
				%battle.pushParties();
				%battle.schedule(250, updateClientDisplays);
				%battle.schedule(500, startTurn);
			}
	}
}

function serverCmdPokemon_SetAction(%this, %id, %action)
{
	if(!isObject(%this.battle))
		return;

	if(!isObject(%id))
	{
		commandToClient(%this, 'Pokemon_ReportError', 0, "Error finding specified combatant");
		return;
	}

	if(%id.owner != %this.getBLID())
	{
		commandToClient(%this, 'Pokemon_ReportError', 0, "Combatant owner does not match");
		return;
	}

	if((%err = %this.battle.setAction(%id, %action)) <= 0)
	{
		commandToClient(%this, 'Pokemon_ReportError', 0, "Illegal action: %1", $Pokemon::ActionError::Text[%err]);
		return;
	}

	//for now we won't do anything if it worked i guess
}

function GameConnection::pushPokemon(%this, %pokemon, %side, %ind)
{
	if(!isPokemon(%pokemon))
		return;

	// echo("f");

	%dex = %pokemon.data.dexNum;
	%level = %pokemon.getStat("Level");
	%name = %pokemon.nickname;
	%gender = %pokemon.gender - 1;
	%shiny = %pokemon.shiny;
	%id = %pokemon.getID();

	if(%side)
	{
		// echo("SIDE 1");
		%hp = %pokemon.getStat("HP") / %pokemon.getStat("MaxHP");
		%hpmax = "";
	}
	else
	{
		// echo("SIDE 0");
		%hp = %pokemon.getStat("HP");
		%hpmax = %pokemon.getStat("MaxHP");

		%xp = %pokemon.getStat("XP");

		for(%i = 0; %i < 4; %i++)
		{
			%move = %pokemon.getMove(%i);
			%pp = %pokemon.getPP(%i);
			%ppmax = %pokemon.getMaxPP(%i);

			commandToClient(%this, 'Pokemon_SetPokemonMove', %side, %ind, %i, %move.name, %move.type, %pp, %ppmax);
		}
	}

	// echo(%side SPC %ind SPC %dex SPC %level SPC %hp SPC %hpmax SPC %xp SPC %name SPC %gender SPC %shiny SPC %id);

	commandToClient(%this, 'Pokemon_SetPokemon', %side, %ind, %dex, %level, %hp, %hpmax, %xp, %name, %gender, %shiny, %id);
}

function GameConnection::pushParty(%this, %trainer)
{
	if(!isObject(%trainer) || %trainer.class !$= "PokemonTrainer")
		return;

	%ct = %trainer.party.getCount();
	for(%i = 0; %i < 6; %i++)
	{
		if(%i >= %ct)
		{
			commandToClient(%this, 'Pokemon_SetPartyPokemon', %i, "", -1);
			continue;
		}

		%p = %trainer.party.getObject(%i);
		if(!isObject(%p))
		{
			commandToClient(%this, 'Pokemon_SetPartyPokemon', %i, "", -1);
			continue;
		}

		%name = %p.nickname;
		%dex = %p.data.dexNum;
		%level = %p.getStat("Level");
		%gender = %p.gender - 1;
		%shiny = %p.shiny;
		%hpcurr = %p.getStat("HP");
		%hpmax = %p.getStat("MaxHP");

		commandToClient(%this, 'Pokemon_SetPartyPokemon', %i, %name, %dex, %level, %gender, %shiny, %hpcurr, %hpmax);
	}
}

function PokemonBattle::pushCombatants(%this)
{
	for(%i = 0; %i < %this.combatants; %i++)
	{
		%comb = %this.combatant[%i];

		%bl_id = %comb.owner;
		%obj = findClientByBL_ID(%bl_id);
		%posid = mFloor(%this.findCombatant(%comb) / 2); //Placement is determined by combatant index in an alternating pattern so that one team's pokemon will always be on every other index.
														 //ie team A's pokemon's indices could be 0, 2, and 4 in a triple battle, and team B's would be 1, 3, and 5.
														 //As such, we can divide the combatant index by two and round down to determine the placement ID of a specific combatant.

		//If the pokemon's owner is present, they are participating in the battle and we should send the client relevant information.
		if(isObject(%obj) || (%ind = searchWords(%this.clients, %obj)) != -1)
			%obj.pushPokemon(%comb, 0, %posid);

		//If there is an opposing player, we need to also send them some data, but we want to send it as a pokemon on the opposing team instead.
		if(isObject(%obj2 = getWord(%this.clients, !%ind))) //There will only ever be two clients battling, so we can count on the index being either zero or one.
			%obj2.pushPokemon(%comb, 1, %posid);			//Since this is the case, we can easily find the opposite client by doing a not operation on the word index cooresponding to a client in the list.
	}
}

function PokemonBattle::pushParties(%this)
{
	%ct = getWordCount(%this.clients);
	for(%i = 0; %i < %ct; %i++)
	{
		%cl = getWord(%this.clients, %i);
		if(!isObject(%cl) || !isObject(%cl.trainer))
			continue;
		%cl.pushParty(%cl.trainer);
	}
}

function PokemonBattle::commandToClients(%this, %cmd, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15)
{
	%ct = getWordCount(%this.clients);
	for(%i = 0; %i < %ct; %i++)
	{
		%cl = getWord(%this.clients, %i);
		if(!isObject(%cl))
			continue;
		commandToClient(%cl, %cmd, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15);
	}
}

function PokemonBattle::setWaitingType(%this, %id)
{
	%this.waitingClients = 0;

	%ct = getWordCount(%this.clients);
	for(%i = 0; %i < %ct; %i++)
	{
		%cl = getWord(%this.clients, %i);
		if(!isObject(%cl))
			continue;
		%cl.waitType = %id;

		if(%id >= 0)
			%this.waitingClients++;
	}
}

function PokemonBattle::updateClientDisplays(%this)
{
	%this.commandToClients('Pokemon_UpdateAllBattleDisplay');
}

function PokemonBattle::sendClientRequest(%this, %req)
{
	switch(%req)
	{
		case 0:
			%this.commandToClients('Pokemon_SetBattleMode', 0);
			%this.setWaitingType(1);

		case 1:
			%this.commandToClients('Pokemon_SetBattleMode', 4);
			%this.commandToClients('Pokemon_ActionStart', 2);

			%this.setWaitingType(2);
	}
}