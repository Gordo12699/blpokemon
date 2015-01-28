//This is a table of values which dictates the modification to a pokemon's stat when affected by moves such as growl, leer, swords dance, etc.
%mods = -7;
$Pokemon::Battle::ModTable[%mods++] = 0.25; //-6
$Pokemon::Battle::ModTable[%mods++] = 0.28; //-5
$Pokemon::Battle::ModTable[%mods++] = 0.33; //-4
$Pokemon::Battle::ModTable[%mods++] = 0.40; //-3
$Pokemon::Battle::ModTable[%mods++] = 0.50; //-2
$Pokemon::Battle::ModTable[%mods++] = 0.66; //-1
$Pokemon::Battle::ModTable[%mods++] = 1;	//0
$Pokemon::Battle::ModTable[%mods++] = 1.5;  //+1
$Pokemon::Battle::ModTable[%mods++] = 2;	//+2
$Pokemon::Battle::ModTable[%mods++] = 2.5;	//+3
$Pokemon::Battle::ModTable[%mods++] = 3;	//+4
$Pokemon::Battle::ModTable[%mods++] = 3.5;	//+5
$Pokemon::Battle::ModTable[%mods++] = 4;	//+6

%mods = -7;
$Pokemon::Battle::ModTableAE[%mods++] = 0.33; 	//-6
$Pokemon::Battle::ModTableAE[%mods++] = 0.375; 	//-5
$Pokemon::Battle::ModTableAE[%mods++] = 0.42; 	//-4
$Pokemon::Battle::ModTableAE[%mods++] = 0.5; 	//-3
$Pokemon::Battle::ModTableAE[%mods++] = 0.6; 	//-2
$Pokemon::Battle::ModTableAE[%mods++] = 0.75; 	//-1
$Pokemon::Battle::ModTableAE[%mods++] = 1;		//0
$Pokemon::Battle::ModTableAE[%mods++] = 1.33;  	//+1
$Pokemon::Battle::ModTableAE[%mods++] = 1.66;	//+2
$Pokemon::Battle::ModTableAE[%mods++] = 2;		//+3
$Pokemon::Battle::ModTableAE[%mods++] = 2.33;	//+4
$Pokemon::Battle::ModTableAE[%mods++] = 2.66;	//+5
$Pokemon::Battle::ModTableAE[%mods++] = 3;		//+6
$Pokemon::Battle::ModTableMin = -6;
$Pokemon::Battle::ModTableMax = 6;

$Pokemon::Battle::CritTable0 = 0.0625;
$Pokemon::Battle::CritTable1 = 0.125;
$Pokemon::Battle::CritTable2 = 0.25;
$Pokemon::Battle::CritTable3 = 0.333;
$Pokemon::Battle::CritTable4 = 0.5;
$Pokemon::Battle::CritMax = 4;

$Pokemon::ActionError::InvalidAction = 0;
$Pokemon::ActionError::InvalidMove = -1;
$Pokemon::ActionError::InvalidTarget = -2;
$Pokemon::ActionError::NoPP = -3;
$Pokemon::ActionError::InvalidUser = -4;
$Pokemon::ActionError::NoRepeat = -5;

function Pokemon_SolveDamage(%attackerLvl, %attackerAttk, %attackPwr, %defendDef, %mods, %rand)
{
	//Note that this is the end formula; modifications must be made to the individual inputs in real battle for things like crits, buffs, etc.
	// %r1 = mFloor((2 * %attackerLvl) / 5 + 2);
	// %r2 = %r1 * %attackerAttk * %attkPwr;
	// %r3 = mFloor(%r2 / %defendDef);
	// %r4 = mFloor(%r3 / 50) + 2;
	// %r5 = %r4 * %mods;
	// %r6 = %r5 * getRandom(85, 100) / 100;

	%mods *= (%rand !$= "" ? %rand : getRandom(85, 100) / 100);

	%r1 = (2 * %attackerLvl + 10) / 250;
	%r2 = %attackerAttk / %defendDef;
	%r3 = %r1 * %r2 * %attackPwr + 2;
	%r4 = %r3 * %mods;

	return %r4;
}

function Pokemon_InitBattle(%wild)
{
	if(!isObject(PokemonBattleGroup))
	{
		new SimGroup(PokemonBattleGroup);
		MissionCleanup.add(PokemonBattleGroup);
	}

	%this = new ScriptObject(PokemonBattle)
			{
				combatants = -1;
				type = 0;

				turn = -1;
				weather = -1;

				wild = (%wild ? true : false);
			};
	PokemonBattleGroup.add(%this);
	MissionCleanup.add(PokemonBattleGroup);

	return %this;
}

function PokemonBattle::setCombatants(%this, %teamA, %teamB)
{
	%teamA = nameToID(%teamA);
	%teamB = nameToID(%teamB);

	if(%teamA.superClass !$= "Pokemon" || %teamB.superClass !$= "Pokemon")
		return false;

	%this.combatant0 = %teamA;
	%this.combatant1 = %teamB;
	%this.combatantIndex[%teamA] = 0;
	%this.combatantIndex[%teamB] = 1;
	%this.combatants = 2;


	for(%i = 0; %i < %this.combatants; %i++)
	{
		if(isObject(%cl = findClientByBL_ID(%this.combatant[%i].owner)))
		{
			%this.client[%i] = %cl;
			%cl.battle = %this;
		}

		%this.initCombatant(%this.combatant[%i]);
	}

	// %this.entry[%teamA] = 0;
	// %this.entry[%teamB] = 0;

	// for(%i = 1; %i < 6; %i++)
	// {
	// 	%stat = $Pokemon::IV[%i];

	// 	%this.modStagePos[%teamA, %stat] = 0;
	// 	%this.modStagePos[%teamB, %stat] = 0;
	// 	%this.modStageNeg[%teamA, %stat] = 0;
	// 	%this.modStageNeg[%teamB, %stat] = 0;

	// 	%this.baseVal[%teamA, %stat] = %teamA.getStat(%stat);
	// 	%this.baseVal[%teamB, %stat] = %teamB.getStat(%stat);
	// }

	// %this.volStatus[%teamA] = "";
	// %this.volStatus[%teamB] = "";
	// %this.volBattle[%teamA] = "";
	// %this.volBattle[%teamB] = "";
	// %this.critStage[%teamA] = 0;
	// %this.critStage[%teamB] = 0;

	return true;
}

function PokemonBattle::initCombatant(%this, %comb, %from)
{
	%comb = nameToID(%comb);
	%from = nameToID(%from);
	PokeDebug("POKEMON BATTLE " @ %this @ " INITIALISING COMBATANT " @ %comb, %this, %comb, %from);
	%this.entry[%comb] = (%this.turn > 0 ? %this.turn : 0);
	if(%this.findCombatant(%from) == -1)
	{
		for(%i = 1; %i < 6; %i++)
		{
			%stat = $Pokemon::IV[%i];

			%this.modStagePos[%comb, %stat] = 0;
			%this.modStageNeg[%comb, %stat] = 0;
		}
		%this.modStagePos[%comb, "Accuracy"] = 0;
		%this.modStagePos[%comb, "Evasion"] = 0;
		%this.modStageNeg[%comb, "Accuracy"] = 0;
		%this.modStageNeg[%comb, "Evasion"] = 0;
		%this.volStatus[%comb] = "";
		%this.volBattle[%comb] = "";
		%this.critStage[%comb] = 0;
	}
	else
	{
		PokeDebug("+--FROM:" SPC %from, %this, %comb, %from);
		for(%i = 1; %i < 6; %i++)
		{
			%stat = $Pokemon::IV[%i];

			%this.modStagePos[%comb, %stat] = %this.modStagePos[%from, %stat];
			%this.modStageNeg[%comb, %stat] = %this.modStageNeg[%from, %stat];
		}
		%this.modStagePos[%comb, "Accuracy"] = %this.modStagePos[%from, "Accuracy"];
		%this.modStagePos[%comb, "Evasion"] = %this.modStagePos[%from, "Evasion"];
		%this.modStageNeg[%comb, "Accuracy"] = %this.modStageNeg[%from, "Accuracy"];
		%this.modStageNeg[%comb, "Evasion"] = %this.modStageNeg[%from, "Evasion"];
		%this.volStatus[%comb] = %this.volStatus[%from];
		%this.volBattle[%comb] = %this.volBattle[%from];
		%this.critStage[%comb] = 0;
	}
}

function PokemonBattle::releaseCombatant(%this, %comb)
{
	%comb = nameToID(%comb);
	PokeDebug("POKEMON BATTLE" SPC %this SPC "RELEASING COMBATANT" SPC %comb, %this, %comb);
	%this.entry[%comb] = "";
	for(%i = 1; %i < 6; %i++)
	{
		%stat = $Pokemon::IV[%i];

		%this.modStagePos[%comb, %stat] = "";
		%this.modStageNeg[%comb, %stat] = "";
	}
	%this.modStagePos[%comb, "Accuracy"] = "";
	%this.modStagePos[%comb, "Evasion"] = "";
	%this.modStageNeg[%comb, "Accuracy"] = "";
	%this.modStageNeg[%comb, "Evasion"] = "";
	%this.volStatus[%comb] = "";
	%this.volBattle[%comb] = "";
	%this.critStage[%comb] = "";
}

function PokemonBattle::findCombatant(%this, %comb)
{
	%comb = nameToID(%comb);
	if(%this.combatantIndex[%comb] !$= "")
		return %this.combatantIndex[%comb];

	return -1;
}

function PokemonBattle::switchCombatant(%this, %comb, %to, %batonPass)
{
	%to = nameToID(%to);
	if((%i = %this.findCombatant(%to)) != -1)
		return false;

	PokeDebug("POKEMON BATTLE" SPC %this SPC "SWITCHING COMBATANT" SPC %comb SPC "TO" SPC %to @ (%batonPass ? " (BATONPASS)" : ""), %this, %comb, %to);
	%this.initCombatant(%to, (%batonPass ? %comb : ""));
	%this.releaseCombatant(%comb);

	%this.combatant[%i] = %to;
	%this.combatantIndex[%to] = %i;
	%this.combatantIndex[%comb] = "";

	return true;
}

function PokemonBattle::getModStage(%this, %comb, %stat, %excludePos, %excludeNeg)
{
	if(%this.findCombatant(%comb) == -1)
		return 0;

	%stageP = %this.modStagePos[%comb, %stat];
	%stageN = %this.modStageNeg[%comb, %stat];

	%stage = %stageP * !%excludePos + %stageN * !%excludeNeg;

	if(%stage > $Pokemon::Battle::ModTableMax)
		return $Pokemon::Battle::ModTableMax;
	else if(%stage < $Pokemon::Battle::ModTableMin)
		return $Pokemon::Battle::ModTableMin;

	return %stage;
}

function PokemonBattle::modStage(%this, %comb, %stat, %mod)
{
	if(%this.findCombatant(%comb) == -1)
		return "BADTARGET";


	%st = %this.getModStage(%comb, %stat);

	if(%st == $Pokemon::Battle::ModTableMin)
		return "TOOLOW";
	else if(%st == $Pokemon::Battle::ModTableMax)
		return "TOOHIGH";

	%newst = %st + %mod;
	if(%newst < $Pokemon::Battle::ModTableMin)
		%mod += $Pokemon::Battle::ModTableMin - %newst;
	else if(%newst > $Pokemon::Battle::ModTableMax)
		%mod += $Pokemon::Battle::ModTableMax - %newst;

	if(%mod < 0)
		%this.modStageNeg[%comb, %stat] += %mod;
	else if(%mod > 0)
		%this.modStagePos[%comb, %stat] += %mod;

	return %this.getModStage(%comb, %stat);
}

function PokemonBattle::getCritStage(%this, %comb)
{
	if(%this.findCombatant(%comb) == -1)
		return 0;

	%stage = %this.shortCritStage[%comb] + %this.critStage[%comb];
	if(%stage > $Pokemon::Battle::CritMax)
		return $Pokemon::Battle::CritMax;
	return %stage;
}

function PokemonBattle::damageCombatant(%this, %comb, %attacker, %amt, %etc)
{
	if(%this.findCombatant(%comb) == -1)
		return -1;

	%new = %comb.modHP(-%amt);
	return %new;
}

function PokemonBattle::Begin(%this)
{
	%teamA = %this.combatant0;
	%teamB = %this.combatant1;
	if(%teamA.superClass !$= "Pokemon" || %teamB.superClass !$= "Pokemon")
		return false;

	PokeDebug("POKEMON BATTLE" SPC %this SPC "BEGIN", %this);
	%this.turn = 0;
	%this.startTurn();

	return true;
}

function PokemonBattle::startTurn(%this)
{
	%teamA = %this.combatant0;
	%teamB = %this.combatant1;
	if(%teamA.superClass !$= "Pokemon" || %teamB.superClass !$= "Pokemon")
		return false;

	%this.turn++;
	PokeDebug("POKEMON BATTLE" SPC %this SPC "STARTING TURN" SPC %this.turn, %this);

	for(%i = 0; %i < %this.combatants; %i++)
	{
		%comb = %this.combatant[%i];
		%this.pendingActions = trim(%this.pendingActions SPC %comb);
		%this.action[%comb] = "";
		%this.shortCritStage[%comb] = 0;
	}

	%this.runSchedules(%this.turn, 0);
}

function PokemonBattle::isLegalAction(%this, %comb, %action)
{
	%comb = nameToID(%comb);
	if(%comb.superClass !$= "Pokemon" || %this.findCombatant(%comb) == -1)
		return $Pokemon::ActionError::InvalidUser;

	if(%this.action[%comb] !$= "")
		return $Pokemon::ActionError::NoRepeat;

	switch$(%cmd = firstWord(%action))
	{
		case "MOVE":
			%rest = restWords(%action);
			%moveIndex = firstWord(%rest) % 4;
			%move = %comb.getMove(%moveIndex);
			if(!isObject(%move) || %move.class !$= "PokemonMove")
				return $Pokemon::ActionError::InvalidMove;
			if(%comb.getPP(%moveIndex) <= 0)
				return $Pokemon::ActionError::NoPP;

			%target = getWord(%rest, 1);
			if(%this.findCombatant(%target) == -1 || %this.findCombatant(%target) == %comb)
				return $Pokemon::ActionError::InvalidTarget;

		case "SWITCH":
			%rest = restWords(%action);
			%rest = nameToID(%rest);
			if(!isObject(%rest) || %rest.superClass !$= "Pokemon")
				return $Pokemon::ActionError::InvalidTarget;
			if(%this.findCombatant(%rest) != -1)
				return $Pokemon::ActionError::InvalidTarget;

		case "NOTHING":
			return true;
		default:
			return $Pokemon::ActionError::InvalidAction;
	}
	return true;
}

function PokemonBattle::setAction(%this, %comb, %action)
{
	%comb = nameToID(%comb);
	if(striPos(%this.volBattle[%comb], "DISABLED") != -1)
		%action = "NOTHING";

	if((%id = %this.isLegalAction(%comb, %action)) != 1)
		return %id;

	%this.action[%comb] = %action;
	%index = searchWords(%this.pendingActions, %comb);
	if(%index > -1)
		%this.pendingActions = removeWord(%this.pendingActions, %index);

	PokeDebug("POKEMON BATTLE" SPC %this SPC "ACTION FOR" SPC %comb SPC "SET", %this, %comb);
	PokeDebug("+--ACTION: " SPC %action, %this, %comb);
	PokeDebug("+--PENDING:" SPC %this.pendingActions);

	%this.verifyTurn();
	return true;
}

function PokemonBattle::verifyTurn(%this)
{
	if(%this.pendingActions $= "")
	{
		for(%i = 0; %i < %this.combatants; %i++)
		{
			%comb = %this.combatant[%i];

			if(!(%id = %this.isLegalAction(%comb, %this.action[%comb])))
			{
				%this.action[%comb] = "";
				%this.pendingActions = trim(%this.pendingActions SPC %comb);
			}
		}

		if(%this.pendingActions $= "")
		{
			%this.executeTurn();
			return true;
		}
	}
	return false;
}

function PokemonBattle::determinePriority(%this)
{
	PokeDebug("+--DETERMINING PRIORITY", %this);
	for(%i = 0; %i < %this.combatants; %i++)
	{
		%comb = %this.combatant[%i];
		%action = %this.action[%comb];
		PokeDebug("   +--COMBATANT" SPC %comb @ ": " SPC %action, %this, %comb);
		switch$(firstWord(%action))
		{
			case "MOVE":
				%move = getWord(%action, 1);
				%move = %comb.getMove(%move);
				if((%ind = %move.searchEffects("PRIORITY")) != -1)
					%pri = %move.getEffectParameter(%ind, 0) + 0;
				else
					%pri = 0;
			case "SWITCH":
				%pri = 6;
			case "NOTHING":
				%pri = 0;
		}
		PokeDebug("   +--PRIORITY" SPC %pri, %this, %comb);

		%this.priority[%pri] = trim(%this.priority[%pri] SPC %comb);
	}
}

function PokemonBattle::endTurn(%this)
{
	PokeDebug("POKEMON BATTLE" SPC %this SPC "ENDING TURN" SPC %this.turn, %this);

	%this.runSchedules(%this.turn, 6);

	for(%i = 0; %i < %this.combatants; %i++)
	{
		%comb = %this.combatant[%i];

		// if((%eff = %comb.getStat("Eff")) !$= "")
		// 	%this.applyEffect(%comb, %eff);
	}

	%this.startTurn();
}

function PokemonBattle::executeAction(%this, %comb)
{
	%action = %this.action[%comb];

	PokeDebug("POKEMON BATTLE" SPC %this SPC "EXECUTING ACTION FOR" SPC %comb, %this, %comb);
	PokeDebug("+--ACTION: " SPC %action, %this, %comb);

	%r = false;
	switch$(firstWord(%action))
	{
		case "MOVE":
			%move = getWord(%action, 1);
			%move = %comb.getMove(%move);
			%targ = getWord(%action, 2);
			%r = %move.execute(%this, %comb, %targ);
		case "SWITCH":
			%targ = getWord(%action, 1);
			%r = %this.switchCombatant(%comb, %targ);
		case "NOTHING":
			%r = true;
			//pass
	}
	return %r;
}

function PokemonBattle::executeTurn(%this)
{
	PokeDebug("POKEMON BATTLE" SPC %this SPC "EXECUTING TURN", %this);
	%this.determinePriority();


	%this.runSchedules(%this.turn, 1);

	for(%i = 8; %i >= -8; %i--)
	{
		%todo = %this.priority[%i];

		PokeDebug("+--PRIORITY" SPC %i @ ": " SPC %todo, %this);

		if(%todo $= "")
			continue;

		%ct = getWordCount(%todo);
		if(%ct == 1)
			%this.executeAction(%todo);
		else
		{
			%sort = new GuiTextListCtrl();
			// %str = "";
			for(%j = 0; %j < %ct; %j++)
			{
				%comb = getWord(%todo, %j);

				%speed = %comb.getStat("Speed");
				%speed *= $Pokemon::Battle::ModTable[%this.getModStage(%comb, "Speed")];

				if(%comb.getStat("Eff") $= "PARALYSIS" && %comb.ability !$= "Quick Feet")
					%speed /= 4;

				if(%comb.ability $= "Quick Feet")
					%speed *= 1.5;

				%append = %speed TAB %comb;

				PokeDebug("   +--SPEED FOR" SPC %comb @ ":" SPC %speed, %this, %comb);

				%sort.addRow(%j, %append);

				// %str = trim(%str TAB %append);
			}

			// PokeDebug("   +--UNSORTED:" SPC %str, %battle);

			//Solve cases where speeds are equal.
			%fct = %sort.rowCount();
			%solved = false;
			while(!%solved)
			{
				deleteVariables("$pktemp::has*");
				%solved = true;
				for(%l = 0; %l < %fct; %l++)
				{
					%f = %sort.getRowText(%l);
					%s = getField(%f, 0);
					if($pktemp::has[%s])
					{
						%s += (getRandom(1) ? 1 : -1);
						%str = setField(%f, 0, %s);
						%sort.setRowByID(%sort.getRowID(%l), %str);
						%solved = false;
						continue;
					}
					$pktemp::has[%s] = true;
				}
			}
			// %str = bubbleSort(%str);

			%sort.sortNumerical(0, false);

			if($PokemonDebug)
			{
				for(%l = 0; %l < %fct; %l++)
					PokeDebug("   +--SORT" SPC %l @ ":" SPC %sort.getRowText(%l), %this);
			}

			for(%k = 0; %k < %fct; %k++)
			{
				%row = %sort.getRowText(%k);

				%s = getField(%row, 0);
				%c = getField(%row, 1);
				PokeDebug("   +--FINAL SPEED FOR" SPC %c @ ":" SPC %s, %this, %c);

				%this.executeAction(%c);
			}

			// PokeDebug("   +--SORTED:" SPC %str, %battle);

			// for(%k = %fct-1; %k >= 0; %k--)
			// {
			// 	%f = getField(%str, %k);
			// 	%s = getWord(%f, 0);
			// 	%c = getWord(%f, 1);
			// 	PokeDebug("   +--FINAL SPEED FOR" SPC %c @ ":" SPC %s, %this, %c);

			// 	%this.executeAction(getWord(%f, 1));
			// }
		}

		%this.priority[%i] = "";
	}

	%this.runSchedules(%this.turn, 5);

	%this.endTurn();
}

function PokemonBattle::setEffectSchedule(%this, %turn, %stage, %eff, %comb, %target)
{
	%turn = mFloor(mAbs(%turn));
	%scheds = %this.turnSchedules[%turn] += 0;

	if(%this.findCombatant(%comb) == -1)
		%comb = 0;
	else
	{
		if(searchWords(%this.combScheds[%comb], %turn) == -1)
			%this.combScheds[%comb] = trim(%this.combScheds[%comb] SPC %turn);
	}

	if(%this.findCombatant(%target) == -1)
		%target = 0;

	%this.turnSchedule[%turn, %scheds] = %eff;
	%this.turnScheduleStage[%turn, %scheds] = %stage;
	%this.turnScheduleComb[%turn, %scheds] = %comb;
	%this.turnScheduleTarget[%turn, %scheds] = %target;
	%this.turnSchedules[%turn]++;
	if(%turn > %this.scheduleGreatestTurn)
		%this.scheduleGreatestTurn = %turn;

	PokeDebug("POKEMON BATTLE" SPC %this SPC "SCHEDULING EFFECT FOR TURN" SPC %turn, %this, %comb, %target);
	PokeDebug("+--SCHEDULES:" SPC %scheds @ "-->" @ %this.turnSchedules[%turn], %this, %comb, %target);
	PokeDebug("+--STAGE:" SPC %stage, %this, %comb, %target);
	PokeDebug("+--COMBATANT:" SPC %comb, %this, %comb, %target);
	PokeDebug("+--TARGET:" SPC %target, %this, %comb, %target);
	PokeDebug("+--EFFECT: " SPC %eff, %this, %comb, %target);
}

function PokemonBattle::scheduleEffect(%this, %turnsAhead, %stage, %eff, %comb, %target)
{
	%turnsAhead = mFloor(mAbs(%turnsAhead));
	%turn = %this.turn + %turnsAhead;

	%this.setEffectSchedule(%turn, %stage, %eff, %comb, %target);
}

function PokemonBattle::runSchedule(%this, %turn, %i)
{
	%eff = %this.turnSchedule[%turn, %i];
	%stage = %this.turnScheduleStage[%turn, %i];
	%user = %this.turnScheduleComb[%turn, %i];
	%target = %this.turnScheduleTarget[%turn, %i];

	PokeDebug("POKEMON BATTLE" SPC %this SPC "RUNNING SCHEDULE ON TURN" SPC %turn, %this, %user, %target);
	PokeDebug("+--EFFECT: " SPC %eff, %this, %user, %target);
	PokeDebug("+--USER:" SPC %user, %this, %user, %target);
	PokeDebug("+--TARGET:" SPC %target, %this, %user, %target);
	PokeDebug("+--STAGE:" SPC %stage, %this, %user, %target);

	return Pokemon_ProcessEffect(%eff, %stage, %this, %user, %target);
}

function PokemonBattle::runSchedules(%this, %turn, %stage)
{
	%turn = mFloor(mAbs(%turn));

	PokeDebug("POKEMON BATTLE" SPC %this SPC "RUNNING SCHEDULES FOR" SPC %turn @ "." @ %stage);

	%j = 0;
	for(%i = 0; %i < %this.turnSchedules[%turn]; %i++)
	{
		if(%this.turnScheduleStage[%turn, %i] !$= %stage)
			continue;

		%this.runSchedule(%turn, %i);
		%j++;
	}
	return %j;
}