$Pokemon::Types = -1;
$Pokemon::Type[$Pokemon::Types++] = "NORMAL";
$Pokemon::TypeIndex["NORMAL"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "FIGHT";
$Pokemon::TypeIndex["FIGHT"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "FLYING";
$Pokemon::TypeIndex["FLYING"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "POISON";
$Pokemon::TypeIndex["POISON"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "GROUND";
$Pokemon::TypeIndex["GROUND"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "ROCK";
$Pokemon::TypeIndex["ROCK"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "BUG";
$Pokemon::TypeIndex["BUG"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "GHOST";
$Pokemon::TypeIndex["GHOST"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "STEEL";
$Pokemon::TypeIndex["STEEL"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "FIRE";
$Pokemon::TypeIndex["FIRE"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "WATER";
$Pokemon::TypeIndex["WATER"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "GRASS";
$Pokemon::TypeIndex["GRASS"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "ELECTRIC";
$Pokemon::TypeIndex["ELECTRIC"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "PSYCHIC";
$Pokemon::TypeIndex["PSYCHIC"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "ICE";
$Pokemon::TypeIndex["ICE"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "DRAGON";
$Pokemon::TypeIndex["DRAGON"] = $Pokemon::Types;
$Pokemon::Type[$Pokemon::Types++] = "DARK";
$Pokemon::TypeIndex["DARK"] = $Pokemon::Types;

$Pokemon::Type["-1"] = "TYPELESS";
$Pokemon::TypeIndex["TYPELESS"] = "-1";

//INDEX		TYPENAME
// 0		 NORMAL
// 1		 FIGHT
// 2		 FLYING
// 3		 POISON
// 4		 GROUND
// 5		 ROCK
// 6		 BUG
// 7		 GHOST
// 8		 STEEL
// 9		 FIRE
// 10		 WATER
// 11		 GRASS
// 12		 ELECTRIC
// 13		 PSYCHIC
// 14		 ICE
// 15		 DRAGON
// 16		 DARK
// -1		 TYPELESS

$Pokemon::TypeEffectiveness0 =	"1 1 1 1 1 0.5 1 0 0.5 1 1 1 1 1 1 1 1";
$Pokemon::TypeEffectiveness1 =	"2 1 0.5 0.5 1 2 0.5 0 2 1 1 1 1 0.5 2 1 2";
$Pokemon::TypeEffectiveness2 =	"1 2 1 1 1 0.5 2 1 0.5 1 1 2 0.5 1 1 1 1";
$Pokemon::TypeEffectiveness3 =	"1 1 1 0.5 0.5 0.5 1 0.5 0 1 1 2 1 1 1 1 1";
$Pokemon::TypeEffectiveness4 =	"1 1 0 2 1 2 0.5 1 2 2 1 0.5 2 1 1 1 1";
$Pokemon::TypeEffectiveness5 =	"1 0.5 2 1 0.5 1 2 1 0.5 2 1 1 1 1 2 1 1";
$Pokemon::TypeEffectiveness6 =	"1 0.5 0.5 0.5 1 1 1 0.5 0.5 0.5 1 2 1 2 1 1 2";
$Pokemon::TypeEffectiveness7 =	"0 1 1 1 1 1 1 2 0.5 1 1 1 1 2 1 1 0.5";
$Pokemon::TypeEffectiveness8 =	"1 1 1 1 1 2 1 1 0.5 0.5 0.5 1 0.5 1 2 1 1";
$Pokemon::TypeEffectiveness9 =	"1 1 1 1 1 0.5 2 1 2 0.5 0.5 2 1 1 2 0.5 1";
$Pokemon::TypeEffectiveness10 =	"1 1 1 1 2 2 1 1 1 2 0.5 0.5 1 1 1 0.5 1";
$Pokemon::TypeEffectiveness11 =	"1 1 0.5 0.5 2 2 0.5 1 0.5 0.5 2 0.5 1 1 1 0.5 1";
$Pokemon::TypeEffectiveness12 =	"1 1 2 1 0 1 1 1 1 1 2 0.5 0.5 1 1 0.5 1";
$Pokemon::TypeEffectiveness13 =	"1 2 1 2 1 1 1 1 0.5 1 1 1 1 0.5 1 1 0";
$Pokemon::TypeEffectiveness14 =	"1 1 2 1 2 1 1 1 0.5 0.5 0.5 2 1 1 0.5 2 1";
$Pokemon::TypeEffectiveness15 =	"1 1 1 1 1 1 1 1 0.5 1 1 1 1 1 1 2 1";
$Pokemon::TypeEffectiveness16 =	"1 0.5 1 1 1 1 1 2 0.5 1 1 1 1 2 1 1 0.5";

$Pokemon::TypeEffectiveness["-1"] = "1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1";

function Pokemon_GetTypeMod(%attType, %dType1, %dType2)
{
	%mod = 1;
	
	%attInd = $Pokemon::TypeIndex[%attType];
	%d1Ind = $Pokemon::TypeIndex[%dType1];
	%d2Ind = $Pokemon::TypeIndex[%dType2];

	%mod *= getWord($Pokemon::TypeEffectiveness[%attInd], %d1Ind);
	%mod *= getWord($Pokemon::TypeEffectiveness[%attInd], %d2Ind);

	return %mod;
}