using Godot;
using System;

public partial class InitializeState : State
{
    public override void Enter()
    {
        stateMachine.TransitionTo("WhoGoesFirstState");
    }

}
