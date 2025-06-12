using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardDeck : Node
{
    private List<Card> _drawPile = new();
    private List<Card> _discardPile = new();
    private Random _random = new Random();
    private PackedScene _cardScene;
    
    // The number of each card type to include in the deck
    [Export] private int _cardsPerType = 24;
    
    public override void _Ready()
    {
        _cardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Card.tscn");
    }
    
    // Initialize the deck with cards
    public void Initialize()
    {
        _drawPile.Clear();
        _discardPile.Clear();
        
        // Create food cards
        for (int i = 0; i < _cardsPerType; i++)
        {
            CreateCard(CardType.Food);
        }
        
        // Create discipline cards
        for (int i = 0; i < _cardsPerType; i++)
        {
            CreateCard(CardType.Discipline);
        }
        
        // Create medicine cards
        for (int i = 0; i < _cardsPerType; i++)
        {
            CreateCard(CardType.Medicine);
        }
        
        // Shuffle the draw pile
        ShuffleDrawPile();
    }
    
    // Create a card of specified type and add to draw pile
    private void CreateCard(CardType type)
    {
        Card card = new();
        card.Initialize(type);
        _drawPile.Add(card);
    }
    
    // Shuffle the draw pile
    private void ShuffleDrawPile()
    {
        // Fisher-Yates shuffle
        int n = _drawPile.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (_drawPile[i], _drawPile[j]) = (_drawPile[j], _drawPile[i]);
        }
    }
    
    // Draw a card from the top of the draw pile
    public Card DrawCard()
    {
        // If draw pile is empty, shuffle discard pile into draw pile
        if (_drawPile.Count == 0)
        {
            ReshuffleDiscardPile();
        }
        
        // If still empty after reshuffling, something's wrong
        if (_drawPile.Count == 0)
        {
            GD.PushError("Error: Attempted to draw from an empty deck with no discard pile.");
            return null;
        }
        
        // Take the top card
        Card drawnCard = _drawPile[0];
        _drawPile.RemoveAt(0);
        
        // Instantiate the visual card
        Card cardInstance = _cardScene.Instantiate<Card>();
        cardInstance.Initialize(drawnCard.CardType);
        
        return cardInstance;
    }
    
    // Draw a card of a specific type
    public Card DrawSpecificCard(CardType type)
    {
        // Look for a card of the specified type in the draw pile
        Card card = _drawPile.FirstOrDefault(c => c.CardType == type);
        
        // If not found in draw pile, check discard pile
        if (card == null)
        {
            card = _discardPile.FirstOrDefault(c => c.CardType == type);
            
            // If found in discard pile, remove it
            if (card != null)
            {
                _discardPile.Remove(card);
            }
        }
        else
        {
            // Remove from draw pile
            _drawPile.Remove(card);
        }

        // If still not found, create a new card of this type
        if (card == null)
        {
            card = new Card();
            card.Initialize(type);
        }
        
        // Instantiate the visual card
        Card cardInstance = _cardScene.Instantiate<Card>();
        cardInstance.Initialize(card.CardType);
        
        return cardInstance;
    }
    
    // Discard a card
    public void DiscardCard(Card card)
    {
        // Add to discard pile
        _discardPile.Add(card);
        
        // Animate the card being discarded
        card.PlayCard();
    }
    
    // Reshuffle discard pile into draw pile
    private void ReshuffleDiscardPile()
    {
        // Move all cards from discard to draw pile
        _drawPile.AddRange(_discardPile);
        _discardPile.Clear();
        
        // Shuffle the draw pile
        ShuffleDrawPile();
    }
}