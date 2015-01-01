function solveTrainerID(%bl_id)
{
	%seed = getRandomSeed();
	setRandomSeed(%bl_id);
	%r = getRandom(65535);
	setRandomSeed(%seed);
	return %r;
}
function solveSecretID(%bl_id)
{
	%seed = getRandomSeed();
	setRandomSeed(%bl_id);
	%r = mAbs(getRandom(0xFFFFFFFF, 0x80000000) | 0) | 0;
	setRandomSeed(%seed);
	return %r;
}

$Pokemon::GenericTrainerID = solveTrainerID(888888);
$Pokemon::GenericSecretID = solveSecretID(888888);

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
			};

	%party = new ScriptGroup("PokemonParty_" @ %bl_id);
	%box = new ScriptGroup("PokemonBox_" @ %bl_id);

	%this.add(%party, %box);

	%this.party = %party;
	%this.box = %box;

	return %this;
}