function PokemonGUI_SetMode(%i)
{
	if(!isObject(PokemonBattleGui))
		return;

	PokemonBattleGui.setMode(%i + 0);
}

function PokemonMoveMouse::onMouseDown(%this, %mod, %pos, %click)
{
	//Send battle command.
	%slot = %this.getGroup().slot;

	echo("CLIENT MOVE PRESS -" SPC %slot SPC "PARAMS:" SPC %mod SPC %pos SPC %click);
}

function PokemonPartyMouseEvent::onMouseDown(%this, %mod, %pos, %click)
{
	//Send battle command.
	%slot = %this.getGroup().slot;

	echo("CLIENT PARTY PRESS -" SPC %slot SPC "PARAMS:" SPC %mod SPC %pos SPC %click);
}