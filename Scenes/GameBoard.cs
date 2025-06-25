using Godot;
using System;

public partial class GameBoard : Node3D
{

    private PlayerManager pm;

    public override void _Ready()
    {
        pm = GetTree().Root.GetNode<PlayerManager>("PlayerManager");
    }

    private StateMachine stateMachine;

    public enum ROLLSTATE
    {
        INIT,
        STARTER_MON,
        GO_FIRST,
        MOVE_BOARD,
        BATTLE_BONUS,
    }

    private ROLLSTATE currentRollState = ROLLSTATE.INIT;
    private ROLLSTATE prevRollState;

    public void SetCurrentActionTaker(PlayerManager.PLAYERNO player)
    {
        pm.CurrentActionTaker = pm.Players[(int)player];
    }

    public void SetRollAffects(PlayerManager.PLAYERNO player)
    {
        pm.CurrentActionAffects = pm.Players[(int)player];
    }

    public void SetRollState(ROLLSTATE state)
    {
        prevRollState = currentRollState;
        currentRollState = state;
    }

    private void _on_debug_move_button_pressed(PlayerManager.PLAYERNO player, int spaces)
    {
        pm.Players[(int)player].MoveSpaces(spaces);
    }

    private void _on_die_roll_finished(int result)
    {
        switch (currentRollState)
        {
            case ROLLSTATE.MOVE_BOARD:
                pm.CurrentActionAffects.MoveSpaces(result);
                break;
            default:
                break;
        }
    }
}
