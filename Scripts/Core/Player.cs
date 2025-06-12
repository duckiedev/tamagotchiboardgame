// Player.cs
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Node
{
    public string PlayerName { get; private set; }
    public TamagotchiType TamagotchiType { get; private set; }
    public PlayerToken Token { get; private set; }
    public int GrowthMeterPosition { get; private set; } = 0;
    public int MaxGrowthPosition { get; private set; } = 10; // Default value, may change based on game mode
    public bool SkipNextTurn { get; set; } = false;
    
    private List<Card> _cards = new();
    
    // Create a player with name and tamagotchi type
    public Player(string name, TamagotchiType type)
    {
        PlayerName = name;
        TamagotchiType = type;
    }
    
    // Set the player's token
    public void SetToken(PlayerToken token)
    {
        Token = token;
    }
    
    // Initialize growth meter based on game mode
    public void InitializeGrowthMeter(GameMode gameMode)
    {
        GrowthMeterPosition = 0;
        
        // In Game 1, players race to center (full-grown)
        // In Game 2, players try to avoid returning to start
        MaxGrowthPosition = gameMode == GameMode.Game1 ? 10 : 10;
    }
    
    // Advance on growth meter (positive for forward, negative for backward)
    public void AdvanceGrowthMeter(int steps)
    {
        int newPosition = Mathf.Clamp(GrowthMeterPosition + steps, 0, MaxGrowthPosition);
        GrowthMeterPosition = newPosition;
    }
    
    // Check if tamagotchi is fully grown (Game 1 win condition)
    public bool IsFullyGrown()
    {
        return GrowthMeterPosition >= MaxGrowthPosition;
    }
    
    // Check if tamagotchi is at start position (Game 2 elimination condition)
    public bool IsAtStart()
    {
        return GrowthMeterPosition <= 0;
    }
    
    // Add a card to player's hand
    public void AddCard(Card card)
    {
        _cards.Add(card);
    }
    
    // Remove a specific card from hand
    public bool RemoveCard(Card card)
    {
        return _cards.Remove(card);
    }
    
    // Count cards in hand
    public int GetCardCount()
    {
        return _cards.Count;
    }
    
    // Get count of specific card type
    public int GetCardCount(CardType type)
    {
        return _cards.Count(card => card.CardType == type);
    }
    
    // Check if player has enough cards of each type
    public bool HasCards(int foodCount, int disciplineCount, int medicineCount)
    {
        int foodCards = GetCardCount(CardType.Food);
        int disciplineCards = GetCardCount(CardType.Discipline);
        int medicineCards = GetCardCount(CardType.Medicine);
        
        return foodCards >= foodCount && 
               disciplineCards >= disciplineCount && 
               medicineCards >= medicineCount;
    }
    
    // Remove specific number of cards by type
    public void RemoveCards(int foodCount, int disciplineCount, int medicineCount)
    {
        // Remove food cards
        for (int i = 0; i < foodCount; i++)
        {
            Card foodCard = _cards.Find(card => card.CardType == CardType.Food);
            if (foodCard != null)
                _cards.Remove(foodCard);
        }
        
        // Remove discipline cards
        for (int i = 0; i < disciplineCount; i++)
        {
            Card disciplineCard = _cards.Find(card => card.CardType == CardType.Discipline);
            if (disciplineCard != null)
                _cards.Remove(disciplineCard);
        }
        
        // Remove medicine cards
        for (int i = 0; i < medicineCount; i++)
        {
            Card medicineCard = _cards.Find(card => card.CardType == CardType.Medicine);
            if (medicineCard != null)
                _cards.Remove(medicineCard);
        }
    }
    
    // Get all cards in hand
    public List<Card> GetCards()
    {
        return _cards;
    }
    
    // Move token to a position on the board
    public virtual void MoveToken(Vector2 position)
    {
        if (Token != null)
        {
            // Tween the movement for smooth animation
            var tween = Token.CreateTween();
            tween.TweenProperty(Token, "global_position", position, 0.5f);
        }
    }
}
