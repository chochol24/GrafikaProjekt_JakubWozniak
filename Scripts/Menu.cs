using Godot;
using System;

public partial class Menu : Control
{
	string level = "res://Scenes/World.tscn";


	private void _on_play_btn_click_end()
	{
		var temp = GetTree().ChangeSceneToFile(level);
	}


	private void _on_exit_btn_click_end()
	{
		GetTree().Quit();
	}

}


