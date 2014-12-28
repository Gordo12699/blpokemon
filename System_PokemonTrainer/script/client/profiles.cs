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