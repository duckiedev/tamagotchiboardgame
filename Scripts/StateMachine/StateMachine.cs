using Godot;
using System;

public partial class StateMachine : Node, IStateMachine
{
    [Signal] public delegate void TransitionedEventHandler(string statePath);
    [Signal] public delegate void StateFinishedEventHandler();

    [Export] NodePath initialState;
    [Export] public State state;
    public State lastState;
    public string stateName = "";

    public StateMachine()
    {
        AddToGroup("stateMachine");
    }
    
    public async override void _Ready()
    {
        state = GetNode<State>(initialState);
        await ToSignal(Owner, "ready");
        state.Enter();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        state.UnhandledInput(@event);
    }

    public override void _Process(double delta)
    {
        state.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        state.PhysicsProcess(delta);
    }


    public void TransitionTo(string targetStatePath, Godot.Collections.Dictionary msg)
    {
        if (!HasNode(targetStatePath)) return;

        State targetState = GetNode<State>(targetStatePath);
        state.Exit();
        EmitSignal("StateFinished");
        lastState = state;
        state = targetState;
        if (msg.Count > 0) 
        {
            state.Enter(msg);
        }
        else
        {
            state.Enter();
        }

        EmitSignal("Transitioned", targetStatePath);
    }

    public void TransitionTo(string targetStatePath)
    {
        TransitionTo(targetStatePath, new Godot.Collections.Dictionary());
    }
}