function PokemonClient_BattleInit()
{
	if(isObject(PokemonClientBattle))
		PokemonClientBattle.delete();

	%this = new ScriptObject(PokemonClientBattle)
			{
				dialogueLen = 0;
			};
	%this.resetBattleData();
	%this.resetPokemonData();


	$Pokemon::ClientBattle = %this;

	return %this;
}

function PokemonClientBattle::resetPokemonData(%this)
{
	for(%i = 0; %i < 3; %i++)
	{
		for(%j = 0; %j < 2; %j++)
		{
			%this.pokemon[%j, %i, "ID"] = 0;
			%this.pokemon[%j, %i, "Name"] = "???";
			%this.pokemon[%j, %i, "HPP"] = 1;
			%this.pokemon[%j, %i, "Dex"] = 0;
			%this.pokemon[%j, %i, "Level"] = 0;
			%this.pokemon[%j, %i, "Gender"] = -1;
			%this.pokemon[%j, %i, "Shiny"] = false;
		}

		%this.pokemon[0, %i, "HPCurr"] = 0;
		%this.pokemon[0, %i, "HPMax"] = 0;
		%this.pokemon[0, %i, "XP"] = 0;
		for(%k = 0; %k < 4; %k++)
		{
			%this.pokemon[0, %i, "Move", %k] = "";
			%this.pokemon[0, %i, "MovePP", %k] = 0;
			%this.pokemon[0, %i, "MovePPMax", %k] = 0;
			%this.pokemon[0, %i, "MoveType", %k] = "NORMAL";
		}
	}
}

function PokemonClientBattle::resetBattleData(%this)
{
	%this.type = 0;
	%this.stage = "";
}

function PokemonClientBattle::dialogueClear(%this)
{
	for(%i = 0; %i < %this.dialogueLen; %i++)
		%this.dialogue[%i] = "";

	%this.dialogueLen = 0;
}

function PokemonClientBattle::dialoguePush(%this, %data)
{
	%this.dialogue[%this.dialogueLen] = %data;
	return %this.dialogueLen++;
}

function PokemonClientBattle::dialoguePop(%this)
{
	if(%this.dialogueLen == 0)
		return "";

	%idx = %this.dialogueLen - 1;
	%line = %this.dialogue[%idx];
	%this.dialogue[%idx] = "";
	%this.dialogueLen--;

	return %line;
}

function PokemonClientBattle::solvePokemonIndex(%this, %ind)
{
	switch(%this.type)
	{
		case 1: %id = ((%id % 2) == 1 ? 2 : 0);
		case 2: %id = %id % 3;
		default: %id = 1;
	}
}

function PokemonClientBattle::setPokemonData(%this, %side, %ind, %data, %val)
{
	%side %= 2;
	%ind = %this.solvePokemonIndex(%ind);

	%this.pokemon[%side, %ind, %data] = %val;
}

function PokemonClientBattle::displayPokemonData(%this)
{
	for(%i = 0; %i < 3; %i++)
	{
		for(%j = 0; %j < 2; %j++)
		{
			%n[%j] = %this.pokemon[%j, %i, "Name"];
			%dex[%j] = %this.pokemon[%j, %i, "Dex"];
			%lvl[%j] = %this.pokemon[%j, %i, "Level"];
			%g[%j] = %this.pokemon[%j, %i, "Gender"];
			%s[%j] = %this.pokemon[%j, %i, "Shiny"];
		}

		%hpp0 = %this.pokemon[0, %i, "HPCurr"];
		%hpp1 = %this.pokemon[1, %i, "HPP"];
		%hpm = %this.pokemon[0, %i, "HPMax"]
		%xp = %this.pokemon[0, %i, "XP"];

		PokemonBattleGui.setPokemon(0, %i, %dex0, %lvl0, %hpp0, %hpm, %xp, %n0, %g0, %s0);
		PokemonBattleGui.setPokemon(1, %i, %dex1, %lvl1, %hpp1, "", "", %n1, %g1, %s1);
	}
}

function PokemonClientBattle::displayMoveData(%this, %ind)
{
	%ind = %this.solvePokemonIndex(%ind);

	for(%i = 0; %i < 4; %i++)
	{
		%type = %this.pokemon[0, %ind, "MoveType", %i];
		%name = %this.pokemon[0, %ind, "Move", %i];
		%pp = %this.pokemon[0, %ind, "MovePP", %i];
		%ppmax = %this.pokemon[0, %ind, "MovePPMax", %i];
	}
}