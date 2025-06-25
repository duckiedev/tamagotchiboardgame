using Godot;
using System;

public partial class WhoGoesFirstState : State
{
    public override void Enter()
    {
        GameBoard.PLAYERNO playerNo = GameBoard.PLAYERNO.ONE;
        foreach (var player in GetNode<Node3D>("%PlayerSpaces").GetChildren())
        {
            
            playerNo += 1;
        }
    }

}
