using Godot;
using System;

public partial class CombatActionsUI : Panel
{
    private VBoxContainer buttonContainer;
    private Godot.Collections.Array<CombatActionButton> combatActionButtons = new();
    private RichTextLabel descriptionText;
    private BattleManager battleManager;

    public override void _Ready()
    {
        buttonContainer = GetNode<VBoxContainer>("ButtonContainer");
        descriptionText = GetNode<RichTextLabel>("Description");
        battleManager = GetNode<BattleManager>("/root/BattleManager");

        foreach (var button in buttonContainer.GetChildren())
        {
            if (button is not CombatActionButton) continue;
            CombatActionButton combatActionButton = button as CombatActionButton;

            combatActionButtons.Add(combatActionButton);
            combatActionButton.Pressed += () => _buttonPressed(combatActionButton);
            combatActionButton.MouseEntered += () => _buttonEntered(combatActionButton);
            combatActionButton.MouseExited += () => _buttonExited(combatActionButton);
        }
        GD.Print("Ready");
    }

    public void SetCombatActions(Godot.Collections.Array<CombatAction> combatActions)
    {
        GD.Print("SetCombatActions");
        for (int i = 0; i < combatActionButtons.Count; i++)
        {
            if (i >= combatActions.Count)
            {
                combatActionButtons[i].Visible = false;
                continue;
            }

            combatActionButtons[i].Visible = true;
            combatActionButtons[i].CombatAction = combatActions[i];
        }
    }

    private void _buttonPressed(CombatActionButton button)
    {
        battleManager.PlayerCastCombatAction(button.CombatAction);
    }

    private void _buttonEntered(CombatActionButton button)
    {
        var ca = button.CombatAction;
        descriptionText.Text = "[b]" + ca.DisplayName + "[/b]\n" + ca.Description;
    }

    private void _buttonExited(CombatActionButton button)
    {
        descriptionText.Text = "";
    }

    private void _on_pass_turn_button_pressed()
    {
        battleManager.NextTurn();
    }
}
