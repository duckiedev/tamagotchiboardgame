using Godot;
using Godot.Collections;
using System;
using System.Runtime.InteropServices;

public partial class Character : Node2D
{
    [Signal] public delegate void OnTakeDamageEventHandler(int health);
    [Signal] public delegate void OnHealEventHandler(int health);

    [Export] public bool IsPlayer;
    [Export] public int Health;
    [Export] public int MaxHealth;

    [Export] public Array<CombatAction> CombatActions;

    private float targetScale = 1.0f;

    private AudioStreamPlayer2D audio;
    private AudioStream takeDamageSFX;
    private AudioStream healSFX;

    public override void _Ready()
    {
        audio = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        takeDamageSFX = GD.Load<AudioStream>("res://Assets/Audio/SFX/take_damage.wav");
        healSFX = GD.Load<AudioStream>("res://Assets/Audio/SFX/heal.wav");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }


    public void BeginTurn()
    {
        targetScale = 1.1f;

        if (IsPlayer)
        {
            GD.Print("Player turn has begun.");
        }
        else
        {
            GD.Print("AI turn has begun.");
        }
    }

    public void EndTurn()
    {
        targetScale = 0.9f;
    }

    public void TakeDamage(int amount)
    {

    }

    public void Heal(int amount)
    {

    }

    public void CastCombatAction(CombatAction action, Character opponent)
    {

    }

    public void PlayAudio(AudioStream stream)
    {
        
    }
}
