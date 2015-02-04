if(!isObject(PokemonNameTextProfile))
{
	new GuiControlProfile(PokemonNameTextProfile)
	{
		opaque = false;
		fontType = "Impact";
		fontSize = 18;
		fontColor = "255 255 255";
		doFontOutline = true;
		fontOutlineColor = "0 0 0";
		justify = "left";
		border = false;
	};
}

if(!isObject(PokemonDialogueTextProfile))
{
	new GuiControlProfile(PokemonDialogueTextProfile)
	{
		opaque = false;
		fontType = "Impact";
		fontSize = 26;
		fontColor = "255 255 255";
		doFontOutline = true;
		fontOutlineColor = "0 0 0";
		justify = "left";
		border = false;
	};
}

if(!isObject(PokemonDialogueCentredTextProfile))
{
	new GuiControlProfile(PokemonDialogueCentredTextProfile)
	{
		opaque = false;
		fontType = "Impact";
		fontSize = 26;
		fontColor = "255 255 255";
		doFontOutline = true;
		fontOutlineColor = "0 0 0";
		justify = "center";
		border = false;
	};
}


if(!isObject(PokemonNumerical1TextProfile))
{
	new GuiControlProfile(PokemonNumerical1TextProfile)
	{
		opaque = false;
		fontType = "Impact";
		fontSize = 20;
		fontColor = "255 255 255";
		doFontOutline = true;
		fontOutlineColor = "0 0 0";
		justify = "right";
		border = false;
	};
}
if(!isObject(PokemonNumerical2TextProfile))
{
	new GuiControlProfile(PokemonNumerical2TextProfile)
	{
		opaque = false;
		fontType = "Impact";
		fontSize = 20;
		fontColor = "255 255 255";
		doFontOutline = true;
		fontOutlineColor = "0 0 0";
		justify = "left";
		border = false;
	};
}

if(!isObject(PokemonXPBarProfile))
{
	new GuiControlProfile(PokemonXPBarProfile)
	{
		fillColor = "72 144 248 255";
		border = 0; 
	};
}

if(!isObject(PokemonHPBarGreenProfile))
{
	new GuiControlProfile(PokemonHPBarGreenProfile)
	{
		fillColor = "0 255 74 181";
		border = 0; 
	};
}

if(!isObject(PokemonHPBarYellowProfile))
{
	new GuiControlProfile(PokemonHPBarYellowProfile)
	{
		fillColor = "234 255 0 181";
		border = 0; 
	};
}

if(!isObject(PokemonHPBarRedProfile))
{
	new GuiControlProfile(PokemonHPBarRedProfile)
	{
		fillColor = "255 0 0 181";
		border = 0; 
	};
}

if(!isObject(CUBE_WindowProfile))
{
	new GuiControlProfile(CUBE_WindowProfile : GuiWindowProfile)
	{
		opaque = 1;

		hasBitmapArray = 1;
		bitmap = $Pokemon::UIRoot @ "cubeWindow.png";

		fillColor = "128 128 128 255";
		fillColorHL = "128 128 128 255";
		fillColorNA = "128 128 128 255";

		fontColor = "225 225 225 255";
		fontColorHL = "225 225 225 255";
		fontColorNA = "0 0 0 255";
		fontColorSEL = "170 170 170 255";
		fontColorLink = "0 0 200 255";
		fontColorLinkHL = "85 25 140 255";
		
		fontType = "Monospace";
		fontSize = 18;
		textOffset = "10 5";
		justify = "left";

		modal = true;
	};
}