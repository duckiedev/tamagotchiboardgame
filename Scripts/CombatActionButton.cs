using Godot;
using System;

[GlobalClass]
public partial class CombatActionButton : Button
{
    private CombatAction combatAction;
    public CombatAction CombatAction {
        get => CombatAction;
        set
        {
            combatAction = value;
            Text = CombatAction.DisplayName;
        }
    }

}
