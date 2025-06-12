using Godot;
using System;
using System.Threading.Tasks;

public partial class BattleManager : Node2D
{
    [Export] private Character playerCharacter;
    [Export] private Character aiCharacter;
    private Character currentCharacter;

    private bool gameOver = false;

    private CombatActionsUI playerUI;

    public override void _Ready()
    {
        playerUI = GetNode<CombatActionsUI>("CanvasLayer/CombatActionsUI");
        NextTurn();
    }

    public async Task NextTurn()
    {
        if (gameOver) return;

        currentCharacter?.EndTurn();

        if (currentCharacter == aiCharacter || currentCharacter is null)
        {
            currentCharacter = playerCharacter;
        }
        else
        {
            currentCharacter = aiCharacter;
        }

        currentCharacter.BeginTurn();

        if (currentCharacter.IsPlayer)
        {
            playerUI.Visible = true;
            playerUI.SetCombatActions(playerCharacter.CombatActions);
        }
        else
        {
            playerUI.Visible = false;
            var waitTime = GD.RandRange(0.5f, 1.5f);
            await ToSignal(GetTree().CreateTimer(waitTime), "timeout");

            var actionToCast = AIDecideCombatAction();
            aiCharacter.CastCombatAction(actionToCast, playerCharacter);

            await ToSignal(GetTree().CreateTimer(0.5), "timeout");
            NextTurn();

        }
    }

    public async void PlayerCastCombatAction(CombatAction action)
    {
        if (playerCharacter != currentCharacter) return;

        playerCharacter.CastCombatAction(action, aiCharacter);
        playerUI.Visible = false;
        await ToSignal(GetTree().CreateTimer(0.5), "timeout");
        NextTurn();
    }

    public CombatAction AIDecideCombatAction()
    {
        return null;
    }
}
