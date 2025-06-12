using Godot;
using System;

public partial class PlayerToken : Node2D
{
    [Export] private Texture2D[] _tamagotchiTextures = new Texture2D[4]; // Blue, Red, Pink, Yellow
    
    private Player _owner;
    private Sprite2D _sprite;
    private AnimationPlayer _animPlayer;
    private Label _nameLabel;
    
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("%TokenSprite");
        _animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        _nameLabel = GetNode<Label>("%NameLabel");
        
        // Start idle animation
        PlayIdleAnimation();
    }
    
    // Initialize with player and tamagotchi type
    public void Initialize(Player player, int colorIndex)
    {
        _owner = player;
        
        // Set the token sprite based on tamagotchi type
        if (_sprite != null && colorIndex >= 0 && colorIndex < _tamagotchiTextures.Length)
        {
            _sprite.Texture = _tamagotchiTextures[colorIndex];
        }
        
        // Set the name label
        if (_nameLabel != null)
        {
            _nameLabel.Text = player.PlayerName;
        }
    }
    
    // Play idle animation
    private void PlayIdleAnimation()
    {
        if (_animPlayer != null && _animPlayer.HasAnimation("idle"))
        {
            _animPlayer.Play("idle");
        }
    }
    
    // Play celebration animation (e.g., when advancing on growth meter)
    public void PlayCelebrationAnimation()
    {
        if (_animPlayer != null && _animPlayer.HasAnimation("celebrate"))
        {
            _animPlayer.Play("celebrate");
            
            // Return to idle afterward
            _animPlayer.Queue("idle");
        }
    }
    
    // Play sad animation (e.g., when retreating on growth meter)
    public void PlaySadAnimation()
    {
        if (_animPlayer != null && _animPlayer.HasAnimation("sad"))
        {
            _animPlayer.Play("sad");
            
            // Return to idle afterward
            _animPlayer.Queue("idle");
        }
    }
}