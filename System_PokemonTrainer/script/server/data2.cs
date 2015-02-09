function PokemonData_Init()
{
	if(isObject(PokemonDataGroup))
		PokemonDataGroup.delete();

	$PokemonData = new ScriptGroup(PokemonDataGroup);
	MissionCleanup.add(PokemonDataGroup);

	// $PokemonData.types = new ScriptGroup(PokemonTypes);
	$PokemonData.pokemon = new ScriptGroup(PokemonList);
	$PokemonData.moves = new ScriptGroup(PokemonMoves);
	$PokemonData.add($PokemonData.pokemon, $PokemonData.moves);

	return PokemonDataGroup;
}

function PokemonDataGroup::loadDataJSON(%this, %file, %type)
{
	if(!isFile(%file) || fileExt(%file) !$= ".json")
		return -1;

	switch$(%type)
	{
		case "Types":
				if(isObject(%this.typesJSON))
					%this.typesJSON.delete();

				%obj = %this.typesJSON = loadJSON(%file);
		case "Pokemon":
				if(isObject(%this.pokemonJSON))
					%this.pokemonJSON.delete();

				%obj = %this.pokemonJSON = loadJSON(%file);
		case "Moves":
				if(isObject(%this.movesJSON))
					%this.movesJSON.delete();

				%obj = %this.movesJSON = loadJSON(%file);
	}
	if(!isObject(%obj))
		return -2;
	return %obj;
}

function PokemonTypes::newType(%this, %name, %eff, %imm, %ineff, %null, %res, %weak)
{
	if(isObject(%objn = "PokemonType" @ %name))
		%objn.delete();

	%obj = new ScriptObject(%objn)
			{
				class = "PokemonType";

				Name = %name;
				Effective = %eff;
				Immune = %imm;
				Ineffective = %ineff;
				Nullified = %null;
				Resistant = %res;
				Weak = %weak;
			};
	%this.add(%obj);
	$Pokemon::DataType[%name] = %obj;

	return %obj;
}

function PokemonList::newPokemon(%this, %name, %type, %baseStats, %catchRate, %desc, %evs, %evolve, %dex, %moves, %learn, %ability, %hability)
{
	if(isObject(%objn = "Pokemon" @ %name @ "Data"))
		%objn.delete();

	%obj = new ScriptObject(%objn)
			{
				class = "PokemonData";

				name = %name;
				type1 = getWord(%type, 0);
				type2 = getWord(%type, 1);

				baseAtk = getWord(%baseStats, 0);
				baseDef = getWord(%baseStats, 1);
				baseSpAtk = getWord(%baseStats, 2);
				baseSpDef = getWord(%baseStats, 3);
				baseSpeed = getWord(%baseStats, 4);
				baseHP = getWord(%baseStats, 5);
				baseEXP = getWord(%baseStats, 6);

				catchRate = %catchRate;

				description = %desc;

				evYieldAtk = getWord(%evs, 0);
				evYieldDef = getWord(%evs, 1);
				evYieldSpAtk = getWord(%evs, 2);
				evYieldSpDef = getWord(%evs, 3);
				evYieldSpeed = getWord(%evs, 4);
				evYieldHP = getWord(%evs, 5);

				evolvesInto = getField(%evolve, 0);
				evolveLevel = getField(%evolve, 1);
				evolveStone = getField(%evolve, 2);
				expGroup = getField(%evolve, 3);

				dexNum = getWord(%dex, 0);
				height = getWord(%dex, 1);
				weight = getWord(%dex, 2);
				genderRatio = getWord(%dex, 3);

				ability = %ability;
				hiddenAbility = %hability;
			};
	%this.add(%obj);
	$Pokemon::DataPokemon[%name] = %obj;

	%j = -1;
	%moveCt = getFieldCount(%moves);
	for(%i = 0; %i < %moveCt; %i++)
	{
		%field = getField(%moves, %i);
		if(trim(%field) $= "")
			continue;

		%lvl = firstWord(%field);
		%move = restWords(%field);
		if(%lvl == -1)
			%lvl = "START";

		%obj.levelMoves[%lvl] = trim(%obj.levelMoves[%lvl] TAB %move);
		%obj.moves[%j++] = %field;
	}
	%obj.totalMoves = %j + 1;

	%j = -1;
	%learnCt = getFieldCount(%learn);
	for(%i = 0; %i < %learnCt; %i++)
	{
		%field = getField(%learn, %i);
		if(trim(%field) $= "")
			continue;

		%obj.learns[%j++] = %field;
	}
	%obj.totalLearns = %j + 1;

	return %obj;
}

function PokemonMoves::newMove(%this, %name, %type, %power, %acc, %contact, %eff, %tm, %hm, %pp, %desc)
{
	%objn = "PokemonMove" @ %name;
	%objn = strReplace(%objn, " ", "_");
	%objn = strReplace(%objn, "-", "DASH");
	if(isObject(%objn))
		%objn.delete();

	%obj = new ScriptObject(%objn)
			{
				class = "PokemonMove";

				name = %name;
				type = %type;
				power = %power;
				accuracy = %acc;
				makesContact = %contact;
				effect = %eff;
				tmNum = %tm;
				hmNum = %hm;
				pp = getWord(%pp, 0);
				ppMax = getWord(%pp, 1);
				description = %desc;
			};
	%this.add(%obj);
	$Pokemon::DataMove[%name] = %obj;

	return %obj;
}

function PokemonMoves::newMove2(%this, %name, %type, %power, %acc, %contact, %eff, %pp, %desc, %class)
{
	%objn = "PokemonMove" @ %name;
	%objn = strReplace(%objn, " ", "_");
	%objn = strReplace(%objn, "-", "DASH");
	if(isObject(%objn))
		%objn.delete();

	%obj = new ScriptObject(%objn)
			{
				class = "PokemonMove";

				name = %name;
				type = %type;
				power = %power;
				accuracy = %acc;
				makesContact = %contact;
				effect = %eff;
				pp = getWord(%pp, 0);
				description = %desc;
				damClass = %class;
			};
	%this.add(%obj);
	$Pokemon::DataMove[%name] = %obj;

	return %obj;
}

function PokemonMoves::addFromJSON2(%this, %json)
{
	if(!isJSONObject(%json) || %json.getJSONType() !$= "hash")
		return -1;

	if(%json.getKey(0) $= "Moves")
		%json = %json.get("Moves");

	%ct = %json.getLength();
	for(%i = 0; %i < %ct; %i++)
	{
		%key = %json.getKey(%i);
		%obj = %json.get(%key);

		%type = strUpr(%obj.get("Type"));
		%power = %obj.get("Power");
		%acc = %obj.get("Accuracy");
		%contact = %obj.get("Contact");
		%eff = strReplace(%obj.get("Effect"), ";", "\t");
		%pp = %obj.get("PP");
		%desc = %obj.get("Description");
		%class = %obj.get("Class");

		%this.newMove2(%key, %type, %power, %acc, %contact, %eff, %pp, %desc, %class);
		%j++;
	}
	return %j;
}

function PokemonMoves::addFromJSON(%this, %json)
{
	if(!isJSONObject(%json) || %json.getJSONType() !$= "hash")
		return -1;

	if(%json.getKey(0) $= "Moves")
		%json = %json.get("Moves");

	%ct = %json.getLength();
	for(%i = 0; %i < %ct; %i++)
	{
		%key = %json.getKey(%i);
		%obj = %json.get(%key);

		%type = strUpr(%obj.get("Type"));
		%power = %obj.get("Power");
		%acc = %obj.get("Accuracy");
		%contact = %obj.get("Contact");
		%eff = %obj.get("Effect");
		%tm = %obj.get("TM");
		%hm = %obj.get("HM");
		%pp = %obj.get("PP") SPC %obj.get("PPMax");
		%desc = %obj.get("Description");

		%this.newMove(%key, %type, %power, %acc, %contact, %eff, %tm, %hm, %pp, %desc);
		%j++;
	}
	return %j;
}

function PokemonTypes::addFromJSON(%this, %json)
{
	if(!isJSONObject(%json) || %json.getJSONType() !$= "hash")
		return -1;

	if(%json.getKey(0) $= "Types")
		%json = %json.get("Types");

	%ct = %json.getLength();
	for(%i = 0; %i < %ct; %i++)
	{
		%key = %json.getKey(%i);
		%obj = %json.get(%key);

		%eff = %obj.get("Effective");
		%imm = %obj.get("Ineffective");
		%ineff = %obj.get("Immune");
		%null = %obj.get("Nullified");
		%res = %obj.get("Resistant");
		%weak = %obj.get("Weak");

		%this.newType(%key, %eff, %imm, %ineff, %null, %res, %weak);
		%j++;
	}
	return %j + 0;
}

function PokemonList::addFromJSON(%this, %json)
{
	if(!isJSONObject(%json) || %json.getJSONType() !$= "hash")
		return -1;

	if(%json.getKey(0) $= "Pokemon")
		%json = %json.get("Pokemon");

	%ct = %json.getLength();
	for(%i = 0; %i < %ct; %i++)
	{
		%key = %json.getKey(%i);
		%obj = %json.get(%key);

		%type = %obj.get("Type1") SPC %obj.get("Type2");

		%bAtk = %obj.get("BaseAttack");
		%bDef = %obj.get("BaseDefence");
		%bSpAtk = %obj.get("BaseSpAttack");
		%bSpDef = %obj.get("BaseSpDefence");
		%bSpeed = %obj.get("BaseSpeed");
		%bHP = %obj.get("BaseHP");
		%bEXP = %obj.get("BaseExp");
		%base = %bAtk SPC %bDef SPC %bSpAtk SPC %bSpDef SPC %bSpeed SPC %bHP SPC %bEXP;

		%catchRate = %obj.get("CatchRate");
		%desc = %obj.get("Description");

		%evAtk = %obj.get("EVGainAtk");
		%evDef = %obj.get("EVGainDef");
		%evSpAtk = %obj.get("EVGainSpAtk");
		%evSpDef = %obj.get("EVGainSpDef");
		%evSpeed = %obj.get("EVGainSpeed");
		%evHP = %obj.get("EVGainHP");
		%evs = %evAtk SPC %efDef SPC %evSpAtk SPC %evSpDef SPC %evSpeed SPC %evHP;

		%evInto = %obj.get("EvolveInto");
		%evLevel = %obj.get("EvolveLevel");
		%evStone = %obj.get("EvolveStone");
		%expGroup = %obj.get("ExperienceGroup");
		%evolve = %evInto TAB %evLevel TAB %evStone TAB %expGroup;

		%dexNum = %obj.get("NationalPokedexNumber");
		%height = %obj.get("Height");
		%weight = %obj.get("Weight");
		%gender = %obj.get("MalePerc");
		%dex = %dexNum SPC %height SPC %weight;

		%moveStr = "";
		%moves = %obj.get("Moves");
		%moveCt = %moves.getLength();
		for(%j = 0; %j < %moveCt; %j++)
		{
			%lvl = %moves.getKey(%j);
			%move = %moves.get(%lvl);

			%moveStr = trim(%moveStr TAB %lvl SPC %move);
		}

		%learnStr = "";
		%learns = %obj.get("Learnset");
		%learnCt = %learns.getLength();
		for(%j = 0; %j < %learnCt; %j++)
			%learnStr = trim(%learnStr TAB %learns.item[%j]);

		%this.newPokemon(%key, %type, %base, %catchRate, %desc, %evs, %evolve, %dex, %moveStr, %learnStr);
		%k++;
	}
	return %k;
}

function PokemonList::addFromJSON2(%this, %json)
{
	if(!isJSONObject(%json) || %json.getJSONType() !$= "hash")
		return -1;

	if(%json.getKey(0) $= "Pokemon")
		%json = %json.get("Pokemon");

	%ct = %json.getLength();
	for(%i = 0; %i < %ct; %i++)
	{
		%key = %json.getKey(%i);
		%obj = %json.get(%key);

		%type = %obj.get("Type");

		%bAtk = %obj.get("BaseAtk");
		%bDef = %obj.get("BaseDef");
		%bSpAtk = %obj.get("BaseSpAtk");
		%bSpDef = %obj.get("BaseSpDef");
		%bSpeed = %obj.get("BaseSpeed");
		%bHP = %obj.get("BaseHP");
		%bEXP = %obj.get("BaseExp");
		%base = %bAtk SPC %bDef SPC %bSpAtk SPC %bSpDef SPC %bSpeed SPC %bHP SPC %bEXP;

		%catchRate = %obj.get("CatchRate");
		%desc = %obj.get("Description");

		%evAtk = %obj.get("EVGainAtk");
		%evDef = %obj.get("EVGainDef");
		%evSpAtk = %obj.get("EVGainSpAtk");
		%evSpDef = %obj.get("EVGainSpDef");
		%evSpeed = %obj.get("EVGainSpeed");
		%evHP = %obj.get("EVGainHP");
		%evs = %evAtk SPC %efDef SPC %evSpAtk SPC %evSpDef SPC %evSpeed SPC %evHP;

		%evInto = %obj.get("EvolveInto");
		%evLevel = %obj.get("EvolveLevel");
		%evStone = %obj.get("EvolveCondition");
		%expGroup = %obj.get("ExperienceGroup");
		%evolve = %evInto TAB %evLevel TAB %evStone TAB %expGroup;

		%dexNum = %obj.get("NationalPokedexNumber");
		%height = %obj.get("Height");
		%weight = %obj.get("Weight");
		%gender = %obj.get("MalePerc");
		%dex = %dexNum SPC %height SPC %weight SPC %gender;

		%moveStr = "";
		%moves = %obj.get("Moves");
		%moveCt = %moves.getLength();
		for(%j = 0; %j < %moveCt; %j++)
			%moveStr = trim(%moveStr TAB %moves.item[%j]);

		%learnStr = "";
		%learns = %obj.get("Learnset");
		%learnCt = %learns.getLength();
		for(%j = 0; %j < %learnCt; %j++)
			%learnStr = trim(%learnStr TAB %learns.item[%j]);

		%abilities = %obj.get("Ability");
		%act = %abilities.getLength();
		if(%act == 1)
			%ability = %abilities.item[0];
		else
			%ability = %abilities.item[0] TAB %abilties.item[1];

		%hability = %obj.get("HiddenAbility");

		%this.newPokemon(%key, %type, %base, %catchRate, %desc, %evs, %evolve, %dex, %moveStr, %learnStr, %ability, %hability);
		%k++;
	}
	return %k;
}

function PokemonList::addFromString(%this, %data)
{
	%i = -1;
	while(%data !$= "")
	{
		%data = nextToken(%data, "l", "\n");
		%l = ltrim(%l);

		if(getField(%l, 0) $= "DATA")
			%line0 = %l;
		if(getField(%l, 0) $= "MOVES")
			%line1 = %l;
		else if(getField(%l, 0) $= "CANLEARN")
			%line2 = %l;
		else if(getField(%l, 0) $= "DEX")
			%line3 = %l;
		else if(getField(%l, 0) $= "EVOLVE")
			%line4 = %l;
	}
	%this.addFromStrings(%line0, %line1, %line2, %line3, %line4);
}

function PokemonList::addFromStrings(%this, %data, %moves, %learns, %dex, %evolve)
{
	%data = removeField(%data, 0);
	%moves = removeField(%moves, 0);
	%learns = removeField(%learns, 0);
	%dex = removeField(%dex, 0);
	%evolve = removeField(%evolve, 0);
	if(%data $= "") //we can honestly do without the rest, but we need data
		return -1;

	%name = getField(%data, 0);
	%type = getField(%data, 1);
	%baseStats = getField(%data, 2);
	%catchRate = getField(%data, 3);
	%evs = getField(%data, 4);
	if(%type $= "" || %baseStats $= "" || %evs $= "" || %catchRate $= "")
		return -1;

	%desc = getField(%dex, 0);
	%passDex = getField(%dex, 1);

	return %this.newPokemon(%name, %type, %baseStats, %catchRate, %desc, %evs, %evolve, %passDex, %moves, %learns);
}

function pkmnDebugPokemon(%t1, %t2, %n1, %n2)
{
	$PokemonDebug = 3;
	PokemonData_Init();

	echo(PokemonList.addFromJSON2($PokemonData.loadDataJSON($Pokemon::Root @ "data/Gen1Revision.json", "Pokemon")));
	echo(PokemonMoves.addFromJSON2($PokemonData.loadDataJSON($Pokemon::Root @ "data/MovesNew.json", "Pokemon")));

	if(isObject($bulb1))
		$bulb1.delete();
	if(isObject($bulb2))
		$bulb2.delete();

	if(isObject($a1))
		$a1.delete();
	$a1 = Trainer_New((%t1 !$= "" ? %t1 : 999999));

	if(isObject($a2))
		$a2.delete();
	$a2 = Trainer_New((%t2 !$= "" ? %t2 : 888888));

	$bulb1 = pkmnDebugBulbasaur((%t1 !$= "" ? %t1 : 999999), %n1);
	$bulb2 = pkmnDebugBulbasaur((%t2 !$= "" ? %t2 : 999999), %n2);
}

function pkmnDebugBulbasaur(%bl_id, %nick)
{
	$b[%bl_id] = Pokemon_New(PokemonBulbasaurData, %bl_id, %bl_id);

	$b[%bl_id].setMove(0, PokemonMoveRazor_Leaf);

	$b[%bl_id].setMove(1, PokemonMoveSeed_Bomb);

	$b[%bl_id].setStatsForLevel(50);

	$b[%bl_id].nickname = %nick;

	%t = "PokemonTrainer_" @ %bl_id;
	if(!isObject(%t))
		%t = Trainer_New(%bl_id);

	%t.handleAddPokemon($b[%bl_id]);

	return $b[%bl_id];
}