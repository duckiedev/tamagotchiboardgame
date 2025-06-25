using Godot;
using System;

public partial class PlayerCharacter : Node3D
{
    [Signal] delegate void FinishedMovingSpaceEventHandler();

    public int CurrentSpace = 0;

    [Export] private Node3D moveSpacesNode;
    private Godot.Collections.Array<BoardSpaceTile> moveSpaces = [];

    public override void _Ready()
    {
        GatherMoveSpaces();
    }

    private void GatherMoveSpaces()
    {
        foreach (var item in moveSpacesNode.GetChildren())
        {
            if (item is BoardSpaceTile)
            {
                moveSpaces.Add(item as BoardSpaceTile);
            }
        }
    }

    public async void MoveSpaces(int spaces)
    {
        var startingSpace = CurrentSpace;
        var endingSpace = Math.Clamp(CurrentSpace + spaces, 0, moveSpaces.Count - 1);

        if (spaces < 0)
        {
            for (int i = startingSpace; i > endingSpace; i--)
            {
                MoveSpace(Math.Sign(spaces));
                await ToSignal(this, SignalName.FinishedMovingSpace);
            }
        }
        else
        {
            for (int i = startingSpace; i < endingSpace; i++)
            {
                MoveSpace(Math.Sign(spaces));
                await ToSignal(this, SignalName.FinishedMovingSpace);
            }
        }
    }

    private async void MoveSpace(int space)
    {
        CurrentSpace += space;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(
                this,
                "global_position",
                moveSpaces[CurrentSpace].GlobalPosition, 0.5).
                SetEase(Tween.EaseType.InOut);
        await ToSignal(tween, Tween.SignalName.Finished);
        EmitSignal(SignalName.FinishedMovingSpace);
    }

    private void CannotMoveThere()
    {
        GD.Print("Can't move there.");
    }

}
