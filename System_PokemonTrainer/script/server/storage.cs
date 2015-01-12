$Pokemon::NumBoxes = 8;
$Pokemon::BoxSize = 30;
$Pokemon::BoxCapacity = $Pokemon::NumBoxes * $Pokemon::BoxSize;


function PokemonBox::getFirstEmpty(%this, %box)
{
	for(%i = 0; %i < $Pokemon::BoxSize; %i++)
	{
		if(!isObject(%this.slot[%box, %i]))
			return %i;
	}
	return -1;
}

function PokemonBox::getNextEmptySlot(%this)
{
	for(%b = 0; %b < $Pokemon::BoxSize; %b++)
	{
		%i = %this.getFirstEmpty(%b);
		if(%i != -1)
			return %b SPC %i;
	}
	return -1;
}

function PokemonBox::assignSlot(%this, %pokemon, %box, %slot)
{
	if(!isPokemon(%pokemon))
		return -1;

	if(isObject(%obj = %this.slot[%box, %slot]))
		return %obj;

	%this.slot[%box, %slot] = %pokemon;
	%pokemon.boxPos = %box SPC %slot;

	return true;
}

function PokemonBox::assignToNextSlot(%this, %pokemon)
{
	if(!isPokemon(%pokemon))
		return false;

	%pos = %this.getNextEmptySlot();
	if(%pos == -1)
		return -1;

	return %this.assignSlot(%pokemon, getWord(%pos, 0), getWord(%pos, 1));
}

function PokemonBox::clearSlotValues(%this)
{
	for(%b = 0; %b < $Pokemon::NumBoxes; %b++)
	{
		for(%s = 0; %s < $Pokemon::BoxSize; %s++)
			%this.slot[%b, %s] = -1;
	}
}

function PokemonBox::assignSlotValues(%this)
{
	%ct = %this.getCount();
	if(%ct != 0)
	{
		%ct2 = 0;
		for(%i = 0; %i < %ct; %i++)
		{
			%obj = %this.getObject(%i);
			%pos = %obj.boxPos;

			if(%boxPos !$= "")
				%this.slot[getWord(%boxPos, 0), getWord(%boxPos, 1)] = %obj;
			else
			{
				%assign[%ct2] = %obj;
				%ct2++;
			}
		}

		for(%j = 0; %j < %ct2; %j++)
			%this.assignToNextSlot(%assign[%j]);
	}
}

function PokemonBox::resetSlotValues(%this)
{
	%this.clearSlotValues();
	%this.assignSlotValues();
}

package Pokemon_Box
{
	function PokemonBox::add(%this, %obj)
	{
		%r = parent::add(%this, %obj);
		if(%this.isMember(%obj) || !isObject(%obj))
			return %r;
	}

	function PokemonGroup::onAdd(%this)
	{
		%cl = findClientByBL_ID(%this.bl_id);
		if(isObject(%cl))
			%this.trainer = %cl;
	}

	function PokemonBox::onAdd(%this)
	{
		parent::onAdd(%this);

		%this.resetSlotValues();
	}
};
activatePackage(Pokemon_Box);