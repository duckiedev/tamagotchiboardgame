using Godot;
using System;

public interface IState
{
    void UnhandledInput(InputEvent @event);
    void Process(double delta);
    void PhysicsProcess(double delta);
    void Enter(Godot.Collections.Dictionary msg);
    void Enter();
    void Exit();
    Node GetStateMachine(Node node);
}