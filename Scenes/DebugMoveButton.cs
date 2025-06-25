using Godot;
using System;

public partial class DebugMoveButton : Button
{
    private PlayerCharacter playerCharacter;
    private LineEdit lineEdit;

    public override void _Ready()
    {
        playerCharacter = GetNode<PlayerCharacter>("%PlayerCharacter1");
        lineEdit = GetNode<LineEdit>("LineEdit");
    }

    public override void _Pressed()
    {
        playerCharacter.MoveSpaces(lineEdit.Text.ToInt());
    }

}
