using Godot;
using System;

public partial class PlayerManager : Node
{
    public enum PLAYERNO
    {
        ONE,
        TWO,
        THREE,
        FOUR
    }

    public Godot.Collections.Array<PlayerCharacter> Players = new();
    public PlayerCharacter CurrentActionTaker;
    public PlayerCharacter CurrentActionAffects;
}
