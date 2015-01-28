$Pokemon::ImageRoot = $Pokemon::Root @ "img/";
$Pokemon::BattleRoot = $Pokemon::ImageRoot @ "battle/";
$Pokemon::StageRoot = $Pokemon::BattleRoot @ "stage/";
$Pokemon::BackgroundRoot = $Pokemon::BattleRoot @ "backgrounds/";
$Pokemon::UIRoot = $Pokemon::ImageRoot @ "ui/";
$Pokemon::PokemonRoot = $Pokemon::ImageRoot @ "pokemon/";
$Pokemon::FrontRoot = $Pokemon::PokemonRoot @ "front/";
$Pokemon::BackRoot = $Pokemon::PokemonRoot @ "back/";
$Pokemon::IconRoot = $Pokemon::PokemonRoot @ "icons/";
$Pokemon::ButtonRoot = $Pokemon::UIRoot @ "buttons/";

function getPokemonImage(%dex, %female, %shiny, %back, %zz)
{
	if(%back == 1)
		%path = $Pokemon::BackRoot;
	else if(%back == 0)
		%path = $Pokemon::FrontRoot;
	else if(%back == -1)
		%path = $Pokemon::IconRoot;

	if(!isFile(%path @ %dex @ ".png"))
	{
		if(!%zz)
			return getPokemonImage(0, %female, %shiny, %back, 1);
		return 0;
	}

	if(%shiny && isFile(%path @ "shiny/" @ %dex @ ".png"))
		%path = %path @ "shiny/";

	if(%female && isFile(%path @ "female/" @ %dex @ ".png"))
		%path = %path @ "female/";

	%file = %path @ %dex @ ".png";
	if(!isFile(%file))
	{
		if(!%zz)
			return getPokemonImage(0, %female, %shiny, %back, 1);
		return 0;
	}

	return %file;
}

function getGenderSymbolImage(%gend, %small)
{
	%gend++;

	if(%gend == 0)
		return "";
	else if(%gend == 1)
		return $Pokemon::UIRoot @ "symbols/male" @ (%small ? "general" : "") @ ".png";
	else if(%gend == 2)
		return $Pokemon::UIRoot @ "symbols/female" @ (%small ? "general" : "") @ ".png";

	return "";
}

function getBattleBackground(%theme, %time)
{
	%f = $Pokemon::BackgroundRoot;
	%tf = %theme @ ".png";

	if(isFile(%f @ %time @ "/" @ %tf))
		%f = %f @ %time @ "/";

	%f = %f @ %tf;
	if(!isFile(%f))
	{
		if(isFile(%f = $Pokemon::BackgroundRoot @ "day/" @ %tf))
			return %f;
		return -1;
	}

	return %f;
}

function getMoveButtonImage(%type, %nonone)
{
	%f = $Pokemon::UIRoot @ "buttons/moves/";
	%tf = %type @ ".png";

	if(isFile(%fp = %f @ %tf))
		return %fp;

	return %f @ "none.png";
}

function getTypeImage(%type)
{
	%f = $Pokemon::UIRoot @ "types/";
	%tf = %type @ ".png";

	if(isFile(%fp = %f @ %tf))
		return %fp;

	return "";
}

exec("./dep/main.cs");
exec("./client/main.cs");

$Pokemon::Exec::ClientMain = true;