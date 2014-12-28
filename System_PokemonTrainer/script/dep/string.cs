function searchWords(%sourceString, %searchString)
{
	if(%searchString $= "")
		return -1;

	%ct = getWordCount(%sourceString);
	for(%i = 0; %i < %ct; %i++)
	{
		if(getWord(%sourceString, %i) $= %searchString)
			return %i;
	}
	return -1;
}

function bubbleSort(%list)
{
	%ct = getFieldCount(%list);
	for(%i = 0; %i < %ct; %i++)
	{
		for(%k = 0; %k < (%ct - %i - 1); %k++)
		{
			if(firstWord(getField(%list, %k)) > firstWord(getField(%list, %k+1)))
			{
				%tmp = getField(%list, %k);
				%list = setField(%list, %k, getField(%list, %k+1));
				%list = setField(%list, %k+1, %tmp);
			}
		}
	}
	return %list;
}

function generateWord(%len)
{
	%vowels = "a e i o u ea ee oo ie ei";
	%cons = "b c d f g h j k l m n p qu r s t v w x y z bb ll gg tt pp rr ng th ch ph kn pl cl sh rh wh kl mp rk st sp";
	%type = getRandom(0, 1);
	for(%i = 0; %i < %len; %i++)
	{
		%lastType = %type;
		%type = !%lastType;
		switch(%type)
		{
			case 0: %add = getWord(%vowels, getRandom(0, getWordCount(%vowels) - 1));
			case 1: %add = getWord(%cons, getRandom(0, getWordCount(%cons) - 1));
			default: %add = getWord(%cons, getRandom(0, getWordCount(%cons) - 1));
		}
		%word = %word @ %add;
	}
	return %word;
}