$Pokemon::Root = "config/scripts/mod/System_PokemonTrainer/";
if(!isFile($Pokemon::Root @ "server.cs"))
	$Pokemon::Root = "Add-Ons/System_PokemonTrainer/";

function Pokemon_ExecClient()
{
	exec("./script/clientMain.cs");
}
Pokemon_ExecClient();