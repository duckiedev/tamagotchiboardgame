using Godot;
using System;

public partial class Menu : Control
{
    private void _on_play_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Battle.tscn");
    }

    private void _on_quit_button_pressed()
    {
        GetTree().Quit();
    }
}
