using Godot;
using System;

public partial class Card : Node2D
{
    [Export] public CardType CardType { get; private set; }
    [Export] public Texture2D CardTexture { get; private set; }
    
    private Sprite2D _cardSprite;
    
    public override void _Ready()
    {
        _cardSprite = GetNode<Sprite2D>("%CardSprite");
        UpdateCardVisual();
    }
    
    public void Initialize(CardType type)
    {
        CardType = type;
        UpdateCardVisual();
    }
    
    private void UpdateCardVisual()
    {
        if (_cardSprite != null)
        {
            // Load the appropriate card texture based on type
            string texturePath = $"res://Assets/Graphics/Cards/{CardType.ToString().ToLower()}.png";
            CardTexture = ResourceLoader.Load<Texture2D>(texturePath);
            _cardSprite.Texture = CardTexture;
        }
    }
    
    // Animate the card being played or discarded
    public void PlayCard()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.1f);
        tween.TweenProperty(this, "scale", Vector2.Zero, 0.3f);
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 0.2f);
        tween.TweenCallback(Callable.From(() => QueueFree()));
    }
    
    // Animate the card being drawn
    public void DrawCard(Vector2 targetPosition)
    {
        Scale = Vector2.Zero;
        Modulate = new Color(1, 1, 1, 0);
        
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", Colors.White, 0.2f);
        tween.TweenProperty(this, "scale", Vector2.One, 0.3f);
        tween.TweenProperty(this, "global_position", targetPosition, 0.5f);
    }
}

public enum CardType
{
    Food,
    Discipline,
    Medicine
}