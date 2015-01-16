if($Pokemon::Exec::GUI)
	return;

exec("./PokemonBattleControl.gui");
$Pokemon::Exec::GUI = true;