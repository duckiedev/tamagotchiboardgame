using Godot;
using System;

[GlobalClass]
public partial class CombatAction : Resource
{
    [Export] public string DisplayName;
    [Export] public string Description;

    [Export] public int MeleeDamage = 0;
    [Export] public int HealAmount = 0;

    [Export] public int BaseWeight = 100;
}
