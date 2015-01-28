function clientCmdPokemon_InitBattle(%type, %stage)
{
	%this = PokemonClient_BattleInit();

	%this.setBattleData(%type, %stage);

	commandToServer('Pokemon_BattleReady', 0);
}

function clientCmdPokemon_DirectDialogue(%text, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8)
{
	PokemonBattleGui.setDialogue(%text, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8);
}

function clientCmdPokemon_EnqueueAction(%action)
{
	if(!isObject(PokemonClientBattle))
		return;

	PokemonClientBattle.actionPush(%action);
}