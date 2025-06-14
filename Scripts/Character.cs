using Godot;
using Godot.Collections;
using System;

public partial class Character : Node2D
{
    [Signal] public delegate void OnTakeDamageEventHandler(int health);
    [Signal] public delegate void OnHealEventHandler(int health);

    [Export] public bool IsPlayer;
    [Export] public int Health;
    [Export] public int MaxHealth;

    [Export] public Array<CombatAction> CombatActions;

    private float targetScale = 1.0f;

    private CharacterVisual characterVisual;
    [Export] private Texture2D displayTexture; 

    private AudioStreamPlayer2D audio;
    private AudioStream takeDamageSFX;
    private AudioStream healSFX;

    public override void _Ready()
    {
        characterVisual = GetNode<CharacterVisual>("CharacterVisual");
        characterVisual.FlipH = !IsPlayer;
        characterVisual.Texture = displayTexture;

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
        Health -= amount;
        EmitSignal(SignalName.OnTakeDamage, Health);
        PlayAudio(takeDamageSFX);
    }

    public void Heal(int amount)
    {
        Health += amount;
        Health = Math.Clamp(Health, 0, MaxHealth);
        EmitSignal(SignalName.OnHeal, Health);
        PlayAudio(healSFX);
    }

    public void CastCombatAction(CombatAction action, Character opponent)
    {
        if (action is null) return;

        if (action.MeleeDamage > 0)
        {
            opponent.TakeDamage(action.MeleeDamage);
        }

        if (action.HealAmount > 0)
        {
            GD.Print("healing");
            Heal(action.HealAmount);
        }
    }

    public void PlayAudio(AudioStream stream)
    {
        
    }
}
