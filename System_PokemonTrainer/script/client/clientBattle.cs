$Pokemon::DialoguePattern::Move = "%1 used %2!";
$Pokemon::DialoguePattern::Miss = "%1's attack missed!";
$Pokemon::DialoguePattern::Fail = "But it failed!";

$Pokemon::ClientBattle::ActionQueuePeriod = 3000;

function PokemonClient_BattleInit()
{
	if(isObject(PokemonClientBattle))
		PokemonClientBattle.delete();

	%this = new ScriptObject(PokemonClientBattle)
			{
				actionLen = 0;
			};
	%this.resetBattleData();
	%this.resetPokemonData();
	%this.resetPartyData();

	%this.updateBattleDisplay();
	%this.updateBattleGuiDisplay();

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
			%this.pokemon[%j, %i, "Dex"] = -1;
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
	%this.stage = "grey";
}

function PokemonClientBattle::resetPartyData(%this)
{
	for(%i = 0; %i < 6; %i++)
	{
		%this.party[%i, "Name"] = -1;
		%this.party[%i, "Dex"] = -1;
		%this.party[%i, "Level"] = 0;
		%this.party[%i, "Gender"] = -1;
		%ths.party[%i, "Shiny"] = false;
		%this.party[%i, "HPCurr"] = 0;
		%this.party[%i, "HPMax"] = 0;
	}
}

function PokemonClientBattle::actionClear(%this)
{
	for(%i = 0; %i < %this.actionLen; %i++)
		%this.action[%i] = "";

	%this.actionLen = 0;
}

function PokemonClientBattle::actionPush(%this, %data)
{
	%this.action[%this.actionLen] = %data;
	return %this.actionLen++;
}

function PokemonClientBattle::actionPop(%this)
{
	if(%this.actionLen == 0)
		return "";

	%line = %this.action0;

	for(%i = 1; %i < %this.actionLen; %i++)
		%this.action[%i-1] = %this.action[%i];

	%this.action[%this.actionLen--] = "";

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
	return %id;
}

function PokemonClientBattle::setPokemonData(%this, %side, %ind, %data, %val)
{
	%side %= 2;
	%ind = %this.solvePokemonIndex(%ind);

	%this.pokemon[%side, %ind, %data] = %val;
}

function PokemonClientBattle::getPokemonData(%this, %side, %ind, %data)
{
	%side %= 2;
	%ind = %this.solvePokemonIndex(%ind);

	return %this.pokemon[%side, %ind, %data];
}

function PokemonClientBattle::setBattleData(%this, %type, %stage)
{
	if(%type !$= "")
		%this.type = %type % 3;

	if(%stage !$= "")
		%this.stage = firstWord(solveBattleStage(%stage));
}

function PokemonClientBattle::setPokemon(%this, %side, %ind, %dex, %level, %hp, %hpmax, %xp, %name, %gender, %shiny, %id)
{
	%side %= 2;
	%this.setPokemonData(%side, %ind, "ID", %id);
	%this.setPokemonData(%side, %ind, "Name", %name);
	%this.setPokemonData(%side, %ind, "Level", %level);
	%this.setPokemonData(%side, %ind, "Dex", %dex);
	%this.setPokemonData(%side, %ind, (%side == 0 ? "HPCurr" : "HPP"), %hp);
	%this.setPokemonData(%side, %ind, "HPMax", %hpmax);
	%this.setPokemonData(%side, %ind, "Gender", %gender);
	%this.setPokemonData(%side, %ind, "Shiny", %shiny);
	%this.setPokemonData(%side, %ind, "XP", %xp);
}

function PokemonClientBattle::setPokemonMove(%this, %side, %ind, %move, %name, %type, %pp, %ppmax)
{
	%this.setPokemonData(%side, %ind, "Move_" @ %move, %name);
	%this.setPokemonData(%side, %ind, "MoveType_" @ %move, %type);
	%this.setPokemonData(%side, %ind, "MovePP_" @ %move, %pp);
	%this.setPokemonData(%this, %ind, "MovePPMax_" @ %move, %ppmax);
}

function PokemonClientBattle::setPartyPokemon(%this, %i, %name, %dex, %level, %gender, %shiny, %hpcurr, %hpmax)
{
	%i %= 6;

	%this.party[%i, "Name"] = %name;
	%this.party[%i, "Dex"] = %dex;
	%this.party[%i, "Level"] = %level;
	%this.party[%i, "Gender"] = %gender;
	%this.party[%i, "Shiny"] = %shiny;
	%this.party[%i, "HPCurr"] = %hpcurr;
	%this.party[%i, "HPMax"] = %hpmax;
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
		%hpm = %this.pokemon[0, %i, "HPMax"];
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
		%name = %this.pokemon[0, %ind, "Move", %i];
		if(%name $= "")
			%type = "none";
		else
			%type = %this.pokemon[0, %ind, "MoveType", %i];
		%pp = %this.pokemon[0, %ind, "MovePP", %i];
		%ppmax = %this.pokemon[0, %ind, "MovePPMax", %i];

		PokemonBattleGui.setMove(%i, %type, %name, %pp, %ppmax);
	}
}

function PokemonClientBattle::displayBattleData(%this)
{
	PokemonBattleGui.setBattleType(%this.type);
	PokemonBattleGui.setBattleStage(%this.stage);
}

function PokemonClientBattle::displayPartyData(%this)
{
	for(%i = 0; %i < 6; %i++)
	{
		// echo(%this.party[%i, "Name"]);
		PokemonBattleGui.setPartySlot(%i, %this.party[%i, "Name"], %this.party[%i, "Dex"], %this.party[%i, "Level"], %this.party[%i, "Gender"], %this.party[%i, "Shiny"], %this.party[%i, "HPCurr"], %this.party[%i, "HPMax"]);
	}
}

function PokemonClientBattle::updateBattleDisplay(%this)
{
	%this.displayBattleData();
	%this.displayPokemonData();
}

function PokemonClientBattle::updateBattleGuiDisplay(%this)
{
	%this.displayPartyData();
}

function PokemonClientBattle::processAction(%this, %action)
{
	%type = getField(%action, 0);
	%params = getFields(%action, 1, getFieldCount(%action)-1);

	switch$(%type)
	{
		case "TEXT":
			PokemonBattleGui.setDialogue(getField(%params, 0));

			return 1;

		case "MOVE":
			%user = getField(%params, 0);
			%name = getField(%params, 1);

			%side = getWord(%user, 0);
			%ind = getWord(%user, 1);
			%uname = %this.getPokemonData(%side, %ind, "Name");

			PokemonBattleGui.setDialogue($Pokemon::DialoguePattern::Move, %uname, %name);

			return 1;

		case "MISS":
			%user = getField(%params, 0);
			%name = getField(%params, 1);

			%side = getWord(%user, 0);
			%ind = getWord(%user, 1);
			%uname = %this.getPokemonData(%side, %ind, "Name");

			PokemonBattleGui.setDialogue($Pokemon::DialoguePattern::Miss, %uname, %name);

			return 1;

		case "FAIL":
			%user = getField(%params, 0);
			%name = getField(%params, 1);

			%side = getWord(%user, 0);
			%ind = getWord(%user, 1);
			%uname = %this.getPokemonData(%side, %ind, "Name");

			PokemonBattleGui.setDialogue($Pokemon::DialoguePattern::Fail, %uname, %name);

			return 1;

		case "DATA":
			%user = getField(%params, 0);
			%data = getField(%params, 1);

			%side = getWord(%user, 0);
			%ind = getWord(%user, 1);

			%dname = firstWord(%data);
			%dval = restWords(%data);

			%this.setPokemonData(%side, %ind, %dname, %dval);
			%this.displayPokemonData();

			return 0;
	}
	return 0;
}

function PokemonClientBattle::beginActionQueue(%this)
{
	%this.actionQueueIterating = true;

	%this.iterateActionQueue(%this);
}

function PokemonClientBattle::iterateActionQueue(%this)
{
	if(isEventPending(%this.actionQueueSchedule))
		cancel(%this.actionQueueSchedule);

	PokemonBattleGui.setDialogue();

	if(%this.actionLen <= 0)
	{
		%this.endActionQueue();
		return;
	}

	%mod = 0;

	%action = %this.actionPop();
	if(%action !$= "")
		%mod = %this.processAction(%action);

	if(%mod >= 0)
		%this.actionQueueSchedule = %this.schedule($Pokemon::ClientBattle::ActionQueuePeriod * %mod, iterateActionQueue);
	else if(%mod < 0)
		return;
}

function PokemonClientBattle::endActionQueue(%this)
{
	%this.actionQueueIterating = false;
	
	PokemonBattleGui.setDialogue();
	//pass, we'll send some info to the server and clean things up here.
}

function PokemonClientBattle::randomiseBattle(%this)
{
	for(%i = 0; %i < 3; %i++)
	{
		%p1 = getRandom(1, 151);
		%p2 = getRandom(1, 151);

		%hp1 = getRandom(1, 999);
		%hp2 = getRandom(1, 999);

		%hpp1 = mFloor(getRandom() * %hp1);
		%hpp2 = mFloor(getRandom() * %hp2);

		%g1 = getRandom(1);
		%g2 = getRandom(1);

		%s1 = getRandom() < 0.09;
		%s2 = getRandom() < 0.09;

		%lvl1 = getRandom(1, 100);
		%lvl2 = getRandom(1, 100);


		%this.setPokemon(0, %i, %p1, %lvl1, %hpp1, %hp1, getRandom(), generateWord(5), %g1, %s1, getRandom(0xFFFF));
		%this.setPokemon(1, %i, %p2, %lvl2, %hpp2, %hp2, 0, generateWord(5), %g2, %s2, getRandom(0xFFFF));
		// ("PokemonFar" @ %i).setBitmap(getPokemonImage(%p2, %g2, %s2, false));
		// ("PokemonClose" @ %i).setBitmap(getPokemonImage(%p1, %g1, %s1, true));

		// ("PokemonOppHPBar" @ %i).Pokemon_SetHPBar(%hpp2, %hp2);
		// ("PokemonPlayerHPBar" @ %i).Pokemon_SetHPBar(%hpp1, %hp1);

		// ("PokemonOppGender" @ %i).setBitmap(getGenderSymbolImage(%g2));
		// ("PokemonPlayerGender" @ %i).setBitmap(getGenderSymbolImage(%g1));

		// ("PokemonOppLevel" @ %i).setValue(%lvl2);
		// ("PokemonPlayerLevel" @ %i).setValue(%lvl1);

		// ("PokemonPlayerHPMax" @ %i).setValue(%hp1);
		// ("PokemonPlayerHPCurr" @ %i).setValue(%hpp1);

		// ("PokemonPlayerXPBar" @ %i).setValue(getRandom());

		// ("PokemonPlayerName" @ %i).setValue(generateWord(5));
		// ("PokemonOppName" @ %i).setValue(generateWord(5));
	}

	// %stageposs = "barren blue cave darksand dirt grass green grey ice pink pokeblue pokegreen pokered rock sand snow water wetlands white yellow";
	// %backgrounds = "mountain indoors cave1 afternoon/ocean field field indoors indoors snowy afternoon/snowy indoors indoors indoors afternoon/mountain ocean snowy ocean field indoors indoors";
	// %word = getRandom(getWordCount(%stageposs)-1);
	// %stage = getWord(%stageposs, %word) @ ".png";

	// PokemonCloseStage.setBitmap($Pokemon::StageRoot @ "close/" @ %stage);
	// PokemonFarStage.setBitmap($Pokemon::StageRoot @ "far/" @ %stage);

	// PokemonBattleBackground.setBitmap(getBattleBackground(getWord(%backgrounds, %word)));

	// %this.setBattleType(getRandom(2));

	return true;
}