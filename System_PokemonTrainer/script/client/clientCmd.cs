function clientCmdPokemon_InitBattle(%type, %stage)
{
	PokeDebug("GOT Pokemon_InitBattle" SPC %type SPC %stage);

	%this = PokemonClient_BattleInit();

	PokemonGUI_SetMode(4);

	%this.setBattleData(%type, %stage);

	%this.updateBattleDisplay();

	commandToServer('Pokemon_BattleReady', 0);
}

function clientCmdPokemon_SetBattleMode(%mode)
{
	PokemonGUI_SetMode(%mode);

	PokeDebug("GOT Pokemon_SetBattleMode" SPC %mode);
}

function clientCmdPokemon_DirectDialogue(%text, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8)
{
	PokemonBattleGui.setDialogue(%text, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8);

	PokeDebug("GOT Pokemon_DirectDialogue" SPC %text);
}

function clientCmdPokemon_EnqueueAction(%i, %action)
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.actionSet(%i, %action);

	PokeDebug("GOT Pokemon_EnqueueAction" SPC %action);
}

function clientCmdPokemon_SetPokemon(%side, %ind, %dex, %level, %hp, %hpmax, %xp, %name, %gender, %shiny, %id)
{
	// echo("s");
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.setPokemon(%side, %ind, %dex, %level, %hp, %hpmax, %xp, %name, %gender, %shiny, %id);

	PokeDebug("GOT Pokemon_SetPokemon" SPC %side SPC %ind SPC %dex SPC %level SPC %hp SPC %hpmax SPC %xp SPC %name SPC %gender SPC %shiny SPC %id);
}

function clientCmdPokemon_SetPokemonMove(%side, %ind, %move, %name, %type, %pp, %ppmax)
{
	// echo("s");
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.setPokemonMove(%side, %ind, %move, %name, %type, %pp, %ppmax);

	PokeDebug("GOT Pokemon_SetPokemonMove" SPC %side SPC %ind SPC %move SPC %name SPC %type SPC %pp SPC %ppmax);
}

function clientCmdPokemon_SetPartyPokemon(%i, %name, %dex, %level, %gender, %shiny, %hpcurr, %hpmax, %id)
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.setPartyPokemon(%i, %name, %dex, %level, %gender, %shiny, %hpcurr, %hpmax, %id);

	PokeDebug("GOT Pokemon_SetPartyPokemon" SPC %i SPC %name SPC %dex SPC %level SPC %gender SPC %shiny SPC %hpcurr SPC %hpmax SPC %id);
}

function clientCmdPokemon_UpdateBattleDisplay()
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.updateBattleDisplay();

	PokeDebug("GOT Pokemon_UpdateBattleDisplay");
}

function clientCmdPokemon_UpdateBattleGuiDisplay()
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.updateBattleGuiDisplay();

	PokeDebug("GOT Pokemon_UpdateBattleGuiDisplay");
}

function clientCmdPokemon_UpdateAllBattleDisplay()
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.updateBattleGuiDisplay();
	PokemonClientBattle.updateBattleDisplay();

	PokeDebug("GOT Pokemon_UpdateAllBattleDisplay");
}

function clientCmdPokemon_DisplayMoveData(%ind)
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.displayMoveData(%ind);

	PokeDebug("GOT Pokemon_DisplayMoveData" SPC %ind);
}

function clientCmdPokemon_ReportError(%err, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7)
{
	if(!isObject(PokemonClientBattle))
		return;

	switch(%err)
	{
		case -1: //Terminal error, stop the battle.
			PokemonClient_BattleEnd();

		case 0: //Generic error, set mode to zero (action menu) and display the given message
			PokemonGUI_SetMode(0);

			if(%a0 !$= "")
				PokemonBattleGui.setDialogue(%a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7);

		case 1: //Generic error, return client to a specific mode and display the given message
			PokemonGUI_SetMode(%a0);

			if(%a1 !$= "")
				PokemonBattleGui.setDialogue(%a1, %a2, %a3, %a4, %a5, %a6, %a7);

		default:
			//pass?
	}

	PokeDebug("GOT Pokemon_ReportError" SPC %err SPC %a0 SPC %a1 SPC %a2 SPC %a3 SPC %a4 SPC %a5 SPC %a6 SPC %a7);
}

function clientCmdPokemon_SetRequest(%type, %a0, %a1, %a2, %a3)
{
	if(!isObject(PokemonClientBattle))
		return;

	switch(%type)
	{
		case 0:
			PokemonGUI_SetMode(0);
			
			PokemonClientBattle.setCurrentCombatant(%a0);

		case 1: //request combatant
			PokemonGUI_SetMode(3);
			PokemonPartyBackButton.setVisible(false);
			commandToServer('Pokemon_BattleReady', 3);

			PokemonClientBattle.requestSwitch = true;

		case 2: //request bounce-back for fainting
			commandToServer('Pokemon_BattleReady', 3);

	}
}

function clientCmdPokemon_ActionStart(%request)
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.actionRequestType = %request | 0;
	PokemonClientBattle.beginActionQueue();
}