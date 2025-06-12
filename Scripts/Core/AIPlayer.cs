using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class AIPlayer : Player
{
    private Random _random = new Random();
    
    // Create an AI player with name and tamagotchi type
    public AIPlayer(string name, TamagotchiType type) : base(name, type)
    {
    }
    
    // Start AI turn
    public async void StartTurn(GameManager gameManager)
    {
        // Wait a moment for player to see it's AI's turn
        await Task.Delay(1000);
        
        // Roll and move
        gameManager.RollDiceAndMove(this);
    }
    
    // Steal a card from another player
    public void StealCard(Player targetPlayer)
    {
        var targetCards = targetPlayer.GetCards();
        
        if (targetCards.Count == 0)
            return;
        
        // AI strategy: Try to maintain a balanced hand
        // Find which card type we have the least of
        int foodCount = GetCardCount(CardType.Food);
        int disciplineCount = GetCardCount(CardType.Discipline);
        int medicineCount = GetCardCount(CardType.Medicine);
        
        // Calculate which type we need most
        List<CardType> priorities = new() { CardType.Food, CardType.Discipline, CardType.Medicine };
        
        // Sort by count (lowest first)
        priorities.Sort((a, b) => {
            int countA = a == CardType.Food ? foodCount : 
                         a == CardType.Discipline ? disciplineCount : medicineCount;
            int countB = b == CardType.Food ? foodCount : 
                         b == CardType.Discipline ? disciplineCount : medicineCount;
            return countA.CompareTo(countB);
        });
        
        // Try to steal the priority card type
        foreach (var cardType in priorities)
        {
            Card cardToSteal = targetCards.Find(card => card.CardType == cardType);
            if (cardToSteal != null)
            {
                targetPlayer.RemoveCard(cardToSteal);
                AddCard(cardToSteal);
                return;
            }
        }
        
        // If we can't find a priority card, just take a random one
        int randomIndex = _random.Next(targetCards.Count);
        Card randomCard = targetCards[randomIndex];
        
        targetPlayer.RemoveCard(randomCard);
        AddCard(randomCard);
    }
    
    // Override the move method to add AI-specific animations or effects
    public override void MoveToken(Vector2 position)
    {
        base.MoveToken(position);
        
        // Could add AI-specific movement effects here
    }
}