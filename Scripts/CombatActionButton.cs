using Godot;
using System;

[GlobalClass]
public partial class CombatActionButton : Button
{
    private CombatAction combatAction;
    public CombatAction CombatAction {
        get => combatAction;
        set
        {
            combatAction = value;
            Text = CombatAction.DisplayName;
        }
    }

}
