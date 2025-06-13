using Godot;
using System;

public partial class EndScreen : Panel
{

    private Label headerText;

    public override void _Ready()
    {
        headerText = GetNode<Label>("HeaderText");
    }

    public void SetHeaderText(string textToDisplay)
    {
        headerText.Text = textToDisplay;
    }

    public void _on_play_again_button_pressed()
    {
        GetTree().ReloadCurrentScene();
    }
}
