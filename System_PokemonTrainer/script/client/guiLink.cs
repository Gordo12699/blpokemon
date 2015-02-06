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

function GuiControl::pokemonActive(%this)
{
	//Empty, just here to prevent errors.
}

function GuiControl::pokemonInactive(%this)
{
	//Empty, just here to prevent errors.
}

function PokemonActionWaitingContent::pokemonActive(%this)
{
	PokemonWaitingText.setText("Waiting");

	%this.stage = 0;
	%this.sched = %this.schedule(1000, textLoop);
}

function PokemonActionWaitingContent::textLoop(%this)
{
	if(isEventPending(%this.sched))
		cancel(%this.sched);

	%this.stage = %this.stage++ % 4;

	for(%i = 0; %i < %this.stage; %i++)
	{
		%pre = %pre @ " ";
		%post = %post @ ".";
	}

	PokemonWaitingText.setText(%pre @ "Waiting" @ %post);

	%this.sched = %this.schedule(1000, textLoop);
}

function PokemonActionWaitingContent::pokemonInactive(%this)
{
	if(isEventPending(%this.sched))
		cancel(%this.sched);
}

function GuiControl::centreInParent(%this)
{
	%e = %this.getExtent();
	%w = getWord(%e, 0);
	%h = getWord(%e, 1);

	%g = %this.getGroup();
	%r = (isObject(%g) ? %g.getExtent() : getRes());
	%rw = getWord(%r, 0);
	%rh = getWord(%r, 1);

	%nX = %rw / 2 - %w / 2;
	%nY = %rh / 2 - %h / 2;

	%this.resize(%nX, %nY, %w, %h);
}

function GuiControl::centreToRes(%this)
{
	%e = %this.getExtent();
	%w = getWord(%e, 0);
	%h = getWord(%e, 1);

	%g = %this.getGroup();
	%r = getRes();
	%rw = getWord(%r, 0);
	%rh = getWord(%r, 1);

	%nX = %rw / 2 - %w / 2;
	%nY = %rh / 2 - %h / 2;

	%this.resize(%nX, %nY, %w, %h);
}