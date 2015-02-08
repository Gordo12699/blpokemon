$Pokemon::NumBoxes = 32;
$Pokemon::BoxSize = 30;
$Pokemon::BoxCapacity = $Pokemon::NumBoxes * $Pokemon::BoxSize;

function solveTrainerID(%bl_id)
{
	%seed = getRandomSeed();
	setRandomSeed(%bl_id);
	%r = mAbs(getRandom(0xFFFFFFFF, 0x80000000) | 0) | 0;
	%r %= 32768;
	setRandomSeed(%seed);
	return %r;
}
function solveSecretID(%bl_id)
{
	%seed = getRandomSeed();
	setRandomSeed(%bl_id);
	%r = mAbs(getRandom(0xFFFFFFFF, 0x80000000) | 0) | 0;
	%r /= 32768;
	setRandomSeed(%seed);
	return %r;
}

$Pokemon::GenericTrainerID = solveTrainerID(888888);
$Pokemon::GenericSecretID = solveSecretID(888888);

function fixDumbStrings(%name)
{
	%name = strReplace(%name, " ", "_");
	%name = strReplace(%name, "-", "DASH");

	return %name;
}

function unFixDumbStrings(%name)
{
	%name = strReplace(%name, "_", " ");
	%name = strReplace(%name, "DASH", "-");

	return %name;
}

function Trainer_New(%bl_id)
{
	%bl_id += 0;
	%objn = "PokemonTrainer_" @ %bl_id;

	if(isObject(%objn))
		return nameToID(%objn);

	%client = findClientByBL_ID(%bl_id);

	%this = new ScriptGroup(%objn)
			{
				class = "PokemonTrainer";

				bl_id = %bl_id;
				client = %client;

				trainerID = solveTrainerID(%bl_id);
				secretID = solveSecretID(%bl_id);

				invEntries = 0;
			};

	%party = new ScriptGroup("PokemonParty_" @ %bl_id)
				{
					class = "PokemonParty";
					superClass = "PokemonGroup";

					bl_id = %bl_id;
					trainer = %this;
				};
	%box = new ScriptGroup("PokemonBox_" @ %bl_id)
				{
					class = "PokemonBox";
					superClass = "PokemonGroup";

					bl_id = %bl_id;
					trainer = %this;
				};

	%this.add(%party, %box);

	%this.party = %party;
	%this.box = %box;

	return %this;
}

function PokemonTrainer::onAdd(%this)
{
	%cl = findClientByBL_ID(%this.bl_id);

	if(isObject(%cl))
		%cl.trainer = %this;

	MissionCleanup.add(%this);

	if(!isObject(PokemonTrainerGroup))
		new SimGroup(PokemonTrainerGroup);

	PokemonTrainerGroup.add(%this);
}

function PokemonTrainer::addInvEntry(%this, %name)
{
	%name = fixDumbStrings(%name);

	if(%this.hasEntry[%name])
		return;

	%this.invEntry[%this.invEntries] = %name;
	%id = %this.invKey[%name] = %this.invEntries;
	%this.invAmt[%this.invEntries] = 0;
	%this.hasEntry[%name] = true;

	%this.invEntries++;

	return %id;
}

function PokemonTrainer::getEntryID(%this, %name)
{
	%name = fixDumbStrings(%name);

	if(!%this.hasEntry[%name])
		return -1;

	return %this.invKey[%name];
}

function PokemonTrainer::setEntryAmt(%this, %id, %amt)
{
	%id += 0;
	if(%id < 0 || %id >= %this.invEntries)
		return false;

	%this.invAmt[%id] = %amt;

	return true;
}

function PokemonTrainer::getEntryAmt(%this, %id)
{
	%id += 0;
	if(%id < 0 || %id >= %this.invEntries)
		return false;

	return %this.invAmt[%id];
}

function PokemonTrainer::getInvAmt(%this, %name)
{
	%id = %this.getEntryID(%name);

	if(%id == -1)
		return -1;

	return %this.getEntryAmt(%id);
}

function PokemonTrainer::setInvAmt(%this, %name, %amt)
{
	%name = fixDumbStrings(%name);

	if(!%this.hasEntry[%name])
		%id = %this.addInvEntry(%name);
	else
		%id = %this.getEntryID(%name);

	%this.setEntryAmt(%id, %amt);
}

function PokemonTrainer::handleAddPokemon(%this, %pokemon)
{
	if(!isPokemon(%pokemon))
		return false;

	if(%this.party.getCount() < 6)
	{
		%this.party.add(%pokemon);
		return 1;
	}

	if(%this.box.getCount() > $Pokemon::BoxCapacity)
		return false;
	else
	{
		%this.box.add(%pokemon);
		return 2;
	}

	return false;
}

function GameConnection::getTrainer(%this)
{
	if(isObject(%this.trainer))
		return %this.trainer;

	return -1;
}

function GameConnection::getPartyPokemon(%this, %i)
{
	%t = %this.getTrainer();

	if(!isObject(%t))
		return -1;

	%i %= 6;

	if(%i > %t.party.getCount()-1)
		return -1;

	return %t.party.getObject(%i);
}

function GameConnection::getBoxPokemon(%this, %i)
{
	%t = %this.getTrainer();

	if(!isObject(%t))
		return -1;

	if(%i > %t.box.getCount()-1)
		return -1;

	return %t.box.getObject(%i);
}

function GameConnection::getNumPartyPokemon(%this)
{
	%t = %this.getTrainer();

	if(!isObject(%t))
		return -1;

	return %t.party.getCount();
}

function GameConnection::getNumBoxPokemon(%this)
{
	%t = %this.getTrainer();

	if(!isObject(%t))
		return -1;

	return %t.box.getCount();
}

package Pokemon_Trainer
{
	function GameConnection::autoAdminCheck(%this)
	{
		%r = parent::autoAdminCheck(%this);

		%bl_id = %this.getBLID();
		%objn = "PokemonTrainer_" @ %bl_id;
		if(!isObject(%objn))
			%trainer = Trainer_New(%bl_id);
		else
			%trainer = nameToID(%objn);

		%this.trainer = %trainer;
		%trainer.client = %this;

		return %r;
	}
};
activatePackage(Pokemon_Trainer);