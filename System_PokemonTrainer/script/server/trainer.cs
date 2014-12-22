function solveTrainerID(%bl_id)
{
	setRandomSeed(%bl_id);
	return getRandom(65535);
}

$Pokemon::GenericTrainerID = solveTrainerID(888888);