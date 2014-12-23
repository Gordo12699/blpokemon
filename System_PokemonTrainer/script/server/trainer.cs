function solveTrainerID(%bl_id)
{
	%seed = getRandomSeed();
	setRandomSeed(%bl_id);
	%r = getRandom(65535);
	setRandomSeed(%seed);
	return %r;
}
$Pokemon::GenericTrainerID = solveTrainerID(888888);