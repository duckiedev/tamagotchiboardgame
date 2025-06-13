using Godot;
using System.Threading.Tasks;

public partial class BattleManager : Node2D
{
    [Export] private Character playerCharacter;
    [Export] private Character aiCharacter;
    private Character currentCharacter;

    private bool gameOver = false;

    private CombatActionsUI playerUI;
    private EndScreen endScreen;

    public override void _Ready()
    {
        playerUI = GetNode<CombatActionsUI>("CanvasLayer/CombatActionsUI");
        endScreen = GetNode<EndScreen>("CanvasLayer/EndScreen");

        playerCharacter.OnTakeDamage += onPlayerTakeDamage;
        aiCharacter.OnTakeDamage += onAITakeDamage;

        endScreen.Visible = false;

        NextTurn();
    }

    private void onPlayerTakeDamage(int Health)
    {
        if (Health <= 0) endGame(aiCharacter);
    }

    private void onAITakeDamage(int Health)
    {
        if (Health <= 0) endGame(playerCharacter);
    }

    private void endGame(Character winner)
    {
        gameOver = true;
        endScreen.Visible = true;

        if (winner == playerCharacter)
        {
            endScreen.SetHeaderText("You have won!");
        }
        else
        {
            endScreen.SetHeaderText("You have lost!");
        }
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
            await NextTurn();

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
        if (aiCharacter != currentCharacter) return null;

        var ai = aiCharacter;
        var player = playerCharacter;

        var actions = ai.CombatActions;

        var weights = new Godot.Collections.Array<int>();
        var totalWeight = 0;

        var aiHealthPerc = (float)(ai.Health / ai.MaxHealth);
        var playerHealthPerc = (float)(player.Health / player.MaxHealth);

        foreach (CombatAction action in actions)
        {
            var weight = action.BaseWeight;
            if (player.Health <= action.MeleeDamage)
            {
                weight *= 3;
            }

            if (action.HealAmount > 0)
            {
                weight *= 1 + (1 - (int)aiHealthPerc);
            }

            weights.Add(weight);
            totalWeight += weight;

        }

        var cumulativeWeight = 0;
        var randWeight = GD.RandRange(0, totalWeight);

        for (int i = 0; i < actions.Count; i++)
        {
            cumulativeWeight += weights[i];

            if (randWeight < cumulativeWeight)
            {
                return actions[i];
            }
        }

        return null;
    }
}
