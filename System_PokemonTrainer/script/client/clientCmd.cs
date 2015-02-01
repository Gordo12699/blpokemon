function clientCmdPokemon_InitBattle(%type, %stage)
{
	PokeDebug("GOT Pokemon_InitBattle" SPC %type SPC %stage);

	%this = PokemonClient_BattleInit();

	%this.setBattleData(%type, %stage);

	commandToServer('Pokemon_BattleReady', 0);
}

function clientCmdPokemon_DirectDialogue(%text, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8)
{
	PokemonBattleGui.setDialogue(%text, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8);

	PokeDebug("GOT Pokemon_DirectDialogue" SPC %text);
}

function clientCmdPokemon_EnqueueAction(%action)
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.actionPush(%action);

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

function clientCmdPokemon_SetPartyPokemon(%i, %name, %dex, %level, %gender, %shiny, %hpcurr, %hpmax)
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.setPartyPokemon(%i, %name, %dex, %level, %gender, %shiny, %hpcurr, %hpmax);

	PokeDebug("GOT Pokemon_SetPartyPokemon" SPC %i SPC %name SPC %dex SPC %level SPC %gender SPC %shiny SPC %hpcurr SPC %hpmax);
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