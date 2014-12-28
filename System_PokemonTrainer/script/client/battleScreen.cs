$Pokemon::ImageRoot = $Pokemon::Root @ "img/";
$Pokemon::BattleRoot = $Pokemon::ImageRoot @ "battle/";
$Pokemon::StageRoot = $Pokemon::BattleRoot @ "stage/";
$Pokemon::BackgroundRoot = $Pokemon::BattleRoot @ "backgrounds/";
$Pokemon::UIRoot = $Pokemon::ImageRoot @ "ui/";
$Pokemon::PokemonRoot = $Pokemon::ImageRoot @ "pokemon/";
$Pokemon::FrontRoot = $Pokemon::PokemonRoot @ "front/";
$Pokemon::BackRoot = $Pokemon::PokemonRoot @ "back/";

$Pokemon::HPProfile0 = PokemonHPBarGreenProfile;
$Pokemon::HPProfile1 = PokemonHPBarYellowProfile;
$Pokemon::HPProfile2 = PokemonHPBarRedProfile;

function getPokemonImage(%dex, %female, %shiny, %back, %zz)
{
	if(%back)
		%path = $Pokemon::BackRoot;
	else
		%path = $Pokemon::FrontRoot;

	if(!isFile(%path @ %dex @ ".png"))
	{
		if(!%zz)
			return getPokemonImage(%dex, %female, %shiny, %back, 1);
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
			return getPokemonImage(%dex, %female, %shiny, %back, 1);
		return 0;
	}

	return %file;
}

function getGenderSymbolImage(%gend)
{
	%gend++;

	if(%gend == 0)
		return "";
	else if(%gend == 1)
		return $Pokemon::UIRoot @ "symbols/male.png";
	else if(%gend == 2)
		return $Pokemon::UIRoot @ "symbols/female.png";

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

function GuiProgressCtrl::Pokemon_SetHPBar(%this, %hp, %maxHP)
{
	if(%maxHP <= 0)
		%maxHP = 1;
	if(%hp < 0)
		%hp = 0;

	%hpPerc = %hp / %maxHP;
	if(%hpPerc > 1)
		%hpPerc = 1;

	if(%hpPerc > 0.5)
		%profile = $Pokemon::HPProfile0;
	else if(%hpPerc < 0.2)
		%profile = $Pokemon::HPProfile2;
	else
		%profile = $Pokemon::HPProfile1;

	%this.setProfile(%profile);
	%this.setValue(mFloor(%hpPerc * 48) / 48);

	return %hpPerc;
}

function PokemonBattleGui::setPokemon(%this, %side, %i, %dex, %level, %hp, %hpmax, %xp, %name, %gender, %shiny)
{
	%i = %i % 3;

	%bside = (%side ? "Far" : "Close");
	%pside = (%side ? "Opp" : "Player");
	if(%dex < 0)
	{
		("Pokemon" @ %bside @ %i).setVisible(false);
		("Pokemon" @ %pside @ "Data" @ %i).setVisible(false);
		return true;
	}

	%sprite = ("Pokemon" @ %bside @ %i);
	%sprite.setVisible(true);
	("Pokemon" @ %pside @ "Data" @ %i).setVisible(true);
	if(%dex $= "show")
		return;

	%sprite.setBitmap(getPokemonImage(%dex, %gender, %shiny, !%side));
	("Pokemon" @ %pside @ "Gender" @ %i).setBitmap(getGenderSymbolImage(%gender));

	("Pokemon" @ %pside @ "Name" @ %i).setValue(%name);
	("Pokemon" @ %pside @ "Level" @ %i).setValue(%level);
	("Pokemon" @ %pside @ "HPBar" @ %i).Pokemon_SetHPBar(%hp, %hpmax);

	if(!%side)
	{
		("PokemonPlayerHPMax" @ %i).setValue(%hpmax);
		("PokemonPlayerHPCurr" @ %i).setValue(%hp);
		("PokemonPlayerXPBar" @ %i).setValue(%xp);
	}
	return true;
}

function PokemonBattleGui::setBattleType(%this, %type)
{
	switch(%type)
	{
		case 1:
			for(%i = 0; %i < 3; %i++)
			{
				%this.setPokemon(0, %i, (%i == 1 ? "show" : -1));
				%this.setPokemon(1, %i, (%i == 1 ? "show" : -1));
			}
		case 2:
			for(%i = 0; %i < 3; %i++)
			{
				%this.setPokemon(0, %i, "show");
				%this.setPokemon(1, %i, "show");
			}
		default:
			for(%i = 0; %i < 3; %i++)
			{
				%this.setPokemon(0, %i, (%i != 1 ? "show" : -1));
				%this.setPokemon(1, %i, (%i != 1 ? "show" : -1));
			}
	}
	return true;
}

function PokemonBattleGui::randomiseBattle(%this)
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


		%this.setPokemon(0, %i, %p1, %lvl1, %hpp1, %hp1, getRandom(), generateWord(5), %g1, %s1);
		%this.setPokemon(1, %i, %p2, %lvl2, %hpp2, %hp2, 0, generateWord(5), %g2, %s2);
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

	%stageposs = "barren blue cave darksand dirt grass green grey ice pink pokeblue pokegreen pokered rock sand snow water wetlands white yellow";
	%backgrounds = "mountain indoors cave1 afternoon/ocean field field indoors indoors snowy afternoon/snowy indoors indoors indoors afternoon/mountain ocean snowy ocean field indoors indoors";
	%word = getRandom(getWordCount(%stageposs)-1);
	%stage = getWord(%stageposs, %word) @ ".png";

	PokemonCloseStage.setBitmap($Pokemon::StageRoot @ "close/" @ %stage);
	PokemonFarStage.setBitmap($Pokemon::StageRoot @ "far/" @ %stage);

	PokemonBattleBackground.setBitmap(getBattleBackground(getWord(%backgrounds, %word)));

	%this.setBattleType(getRandom(2));

	return true;
}