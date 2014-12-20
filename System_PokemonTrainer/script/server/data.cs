function PokemonData_Init()
{
	if(isObject(PokemonData))
		PokemonData.delete();

	%this = new ScriptObject(PokemonData);
			{
				attackCt = 0;
				pokemonCt = 0;
				typeCt = 0;
			};
	MissionCleanup.add(PokemonData);
	return %this;
}

//ATTACK STRING SERIALISATION ('	^	' is one tab, emphasized for readability)
//-------------------------------------------------------------------------------------------------
//	NAME	^	TYPE CLASS POWER ACC EFF? EFF% EFFTYPE STAT? STAT% STATTYPE	^	SCRIPTNAME
//-------------------------------------------------------------------------------------------------
//PARAM			DATATYPE	DESCRIPTION
//NAME		:	String; 	Name of the attack
//TYPE		:	String; 	Type of the attack
//CLASS		:	Int; 		Physical 1	Special 2	or Other 3
//POWER		:	Int; 		Attack power
//ACC		:	Int; 		Attack accuracy
//EFF?		:	Boolean; 	Does the attack cause statistic mods?
//EFF%		:	Float;		Percentage chance of mod occuring
//EFFTYPE	:	Int;		Type of modifier
//								0	:	Attack
//								1	: 	Sp. Attack
//								2	:	Defence
//								3	:	Sp. Defence
//								4	:	Speed
//								5	:	Accuracy
//								6	:	Evasiveness
//STAT?		:	Boolean;	Does the attack cause status effects?
//STAT%		:	Float;		Percentage chance of status occuring
//STATTYPE	:	Int;		Type of status
//								0	:	Burn
//								1	:	Freeze
//								2	:	Paralysis
//								3	:	Poison
//								4	:	Sleep
//								5	:	Confusion
//SCRIPTNAME:	String;		Optional function name to be called when the attack has been used
//								Passed arguments: %battleSO, %attacker, %defender
//									%battleSO	:	ScriptObject regulating the current battle [See battle.cs]
//									%attacker	:	Attacking pokemon
//									%defender	:	Defending pokemon

function isValidAttackSerialisation(%string)
{
	if(getFieldCount(%string) < 2)
		return false;

	%name = trim(getField(%input, 0));
	%data = trim(getField(%input, 1));
	%script = trim(getField(%input, 2));

	if(%name $= "")
		return false;

	if(getWordCount(%data) < 10)
		return false;

	if(!isFunction(%script))
		warn("isValidAttackSerialisation - \'" SPC %script SPC "\' is not a function! (Attack name \'" SPC %name SPC "\'')");

	return true;
}

function PokemonData::RegisterAttack(%this, %input)
{
	if(!isValidAttackSerialisation(%input))
		return -1;

	%name = trim(getField(%input, 0));
	%data = trim(getField(%input, 1));
	%script = trim(getField(%input, 2));

	%this.attackName[%this.attackCt] = %name;

	%this.attackDataString[%this.attackCt] = %data;
	%this.attackData[%this.attackCt, "Type"] = getWord(%data, 0);
	%this.attackData[%this.attackCt, "Class"] = getWord(%data, 1);
	%this.attackData[%this.attackCt, "Power"] = getWord(%data, 2);
	%this.attackData[%this.attackCt, "Accuracy"] = getWord(%data, 3);
	%this.attackData[%this.attackCt, "HasEffect"] = getWord(%data, 4);
	%this.attackData[%this.attackCt, "EffectChance"] = getWord(%data, 5);
	%this.attackData[%this.attackCt, "EffectType"] = getWord(%data, 6);
	%this.attackData[%this.attackCt, "HasStatus"] = getWord(%data, 7);
	%this.attackData[%this.attackCt, "StatusChance"] = getWord(%data, 8);
	%this.attackData[%this.attackCt, "StatusType"] = getWord(%data, 9);

	%this.attackScript[%this.attackCt] = %script;
	%this.attackNames[%name] = %this.attackCt; //Could be bonus cool points if we get lazy and implement overlaps from newer generations; we wouldn't have to worry about older verions being looked up.

	return %this.attackCt++;
}

//POKEMON STRING SERIALISATION ('	^	' is one tab, emphasized for readability)
//-------------------------------------------------------------------------------------------------
//NAME	^	ID TYPE1 TYPE2 HP ATT DEF SPATT SPDEF SPEED	^	EVTYPE EVID EVPARAM
//-------------------------------------------------------------------------------------------------
//NAME		:	Name of the pokemon
//ID		:	National ID
//TYPE1+2	:	Pokemon type
//HP		:	Base HP
//ATT		:	Base attack
//DEF		:	Base defence
//SPATT		:	Base special attack
//SPDEF		:	Base special defence
//SPEED		:	Base speed
//EVTYPE	:	Type of evolution
//					0	:	None
//					1	:	By level
//					2	:	By trading
//EVID		:	ID of pokemon to evolve to
//EVPARAM	:	Data for evolution, e.g. relevant level or item

function isValidPokemonSerialisation(%input)
{
	%name = trim(getField(%input, 0));
	%data = trim(getField(%input, 1));
	%evo = trim(getField(%input, 2));

	if(%name $= "")
		return false;

	if(getWordCount(%data) < 9)
		return false;

	if(getWordCount(%evo) < 3)
		return false;

	return true;
}

function PokemonData::RegisterPokemon(%this, %input)
{
	if(!isValidPokemonSerialisation(%input))
		return -1;

	%name = trim(getField(%input, 0));
	%data = trim(getField(%input, 1));
	%evo = trim(getField(%input, 2));

	%this.pokemon[%this.pokemonCt] = %name;

	%this.pokemonDataString[%this.pokemonCt] = %data;
	%this.pokemonData[%this.pokemonCt, "ID"] = %id = getWord(%data, 0);
	%this.pokemonData[%this.pokemonCt, "Type1"] = getWord(%data, 1);
	%this.pokemonData[%this.pokemonCt, "Type2"] = getWord(%data, 2);
	%this.pokemonData[%this.pokemonCt, "HP"] = getWord(%data, 3);
	%this.pokemonData[%this.pokemonCt, "Attack"] = getWord(%data, 4);
	%this.pokemonData[%this.pokemonCt, "Defence"] = getWord(%data, 5);
	%this.pokemonData[%this.pokemonCt, "SpAttack"] = getWord(%data, 6);
	%this.pokemonData[%this.pokemonCt, "SpDefence"] = getWord(%data, 7);
	%this.pokemonData[%this.pokemonCt, "Speed"] = getWord(%data, 8);

	%this.pokemonEvolutionSting[%this.pokemonCt] = %evo;
	%this.pokemonEvolution[%this.pokemonCt, "Type"] = getWord(%evo, 0);
	%this.pokemonEvolution[%this.pokemonCt, "Target"] = getWord(%evo, 1);
	%this.pokemonEvolution[%this.pokemonCt, "Parameters"] = getWords(%evo, getWordCount(%evo)-1);

	%this.pokemonIDs[%id] = %this.pokemonCt;
	%this.pokemonNames[%name] = %this.pokemonCt;

	return %this.pokemonCt++;
}

//TYPE STRING SERIALISATION (^ tabs again blugh)
//-------------------------------------------------------------------------------------------------
//	TYPENAME	^	EFFECTIVE	^	INEFFECTIVE	^	0x	^	WEAK	^	RESISTANT	^	IMMUNE
//-------------------------------------------------------------------------------------------------
//TYPENAME		:	Name of the type
//EFFECTIVE		:	Defending types which receive 2x damage
//INEFFECTIVE	:	Defending types which receive 0.5x damage
//0x			:	Defending types which receive 0x damage
//WEAK			:	Attacking types which deal 2x damage
//RESISTANT		:	Attacking types which deal 0.5x damage
//IMMUNE		:	Attacking types which deal 0x damage
//All types not listed are assumed to be normally effective.

function isValidTypeSerialisation(%input)
{
	if(getFieldCount(%input) == 0)
		return false;

	%name = getField(%input, 0);

	if(getWordCount(%name) != 1)
		return false;

	return true;
}

function PokemonData::RegisterType(%this, %input)
{
	if(!isValidTypeSerialisation(%input))
		return -1;

	%name = getField(%input, 0);
	%eff = getField(%input, 1);
	%weak = getField(%input, 4);
	%nil = getField(%input, 3);
	%imm = getField(%input, 6);
	%res = getField(%input, 5);
	%ineff = getField(%input, 2);

	%this.type[%this.typeCt] = %name;

	%this.typeEffData[%this.typeCt] = %eff;
	%ct = getWordCount(%eff);
	for(%i = 0; %i < %ct; %i++)
	{
		%type = getWord(%eff, %i);
		%this.typeEff[%this.typeCt] = %type;
		%this.typeEffKey[%this.typeCt, %type] = true; 
	}

	%this.typeWeakData[%this.typeCt] = %Weak;
	%ct = getWordCount(%Weak);
	for(%i = 0; %i < %ct; %i++)
	{
		%type = getWord(%Weak, %i);
		%this.typeWeak[%this.typeCt] = %type;
		%this.typeWeakKey[%this.typeCt, %type] = true; 
	}

	%this.typeNilData[%this.typeCt] = %nil;
	%ct = getWordCount(%nil);
	for(%i = 0; %i < %ct; %i++)
	{
		%type = getWord(%nil, %i);
		%this.typeNil[%this.typeCt, %i] = %type;
		%this.typeNilKey[%this.typeCt, %type] = true; 
	}

	%this.typeImmData[%this.typeCt] = %imm;
	%ct = getWordCount(%imm);
	for(%i = 0; %i < %ct; %i++)
	{
		%type = getWord(%imm, %i);
		%this.typeImm[%this.typeCt, %i] = %type;
		%this.typeImmKey[%this.typeCt, %type] = true;
	}

	%this.typeResData[%this.typeCt] = %res;
	%ct = getWordCount(%res);
	for(%i = 0; %i < %ct; %i++)
	{
		%type = getWord(%res, %i);
		%this.typeRes[%this.typeCt, %i] = %type;
		%this.typeResKey[%this.typeCt, %type] = true;
	}

	%this.typeInEffData[%this.typeCt] = %ineff;
	%ct = getWordCount(%ineff);
	for(%i = 0; %i < %ct; %i++)
	{
		%type = getWord(%ineff, %i);
		%this.typeInEff[%this.typeCt, %i] = %type;
		%this.typeInEffKey[%this.typeCt, %type] = true;
	}

	%this.typeNames[%name] = %this.typeCt;

	%this.typeCt++;
}

function PokemonData::RegisterFromFile(%this, %path, %type)
{
	%valid = "Attack Attacks Pokemon Type Types";
	if(!isFile(%path) || searchWords(%valid, %type) == -1)
		return 0;

	%file = new FileObject();
	%file.openForRead(%path);
	for(%i = 0; !%file.isEOF(); %i++)
	{
		%line = %file.readLine();
		if(getSubStr(%line, 0, 1) $= "#") //comments i guess cus w/e
			continue;

		switch$(%type)
		{
			case "Attack":	%this.RegisterAttack(%line);
			case "Attacks":	%this.RegisterAttack(%line);
			case "Pokemon":	%this.RegisterPokemon(%line);
			case "Type":	%this.RegisterType(%line);
			case "Types":	%this.RegisterType(%line);
			default: break;
		}
		
	}
	%file.close();
	%file.delete();

	return %i;
}