using Godot;
using System;

public interface IStateMachine
{
    void TransitionTo(string targetStatePath, Godot.Collections.Dictionary msg);
    void TransitionTo(string targetStatePath);
}
