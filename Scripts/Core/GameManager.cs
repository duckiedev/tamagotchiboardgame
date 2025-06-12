using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    [Export] public GameMode CurrentGameMode { get; private set; } = GameMode.Game1;
    [Export] public int PlayerCount { get; private set; } = 2;
    [Export] public bool UseAI { get; set; } = false;
    
    private List<Player> _players = new();
    private int _currentPlayerIndex = 0;
    private CardDeck _cardDeck;
    private Node _boardNode;
    private List<BoardSpace> _boardSpaces = new();
    private GameHUDController _hudController;
    private bool _gameOver = false;
    
    // Signals
    [Signal] public delegate void GameStartedEventHandler(GameMode mode, int playerCount);
    [Signal] public delegate void TurnChangedEventHandler(Player currentPlayer);
    [Signal] public delegate void GameOverEventHandler(Player winner);
    [Signal] public delegate void PlayerEliminatedEventHandler(Player player);
    [Signal] public delegate void CardPlayedEventHandler(Player player, CardType cardType);
    [Signal] public delegate void GrowthMeterChangedEventHandler(Player player, int oldValue, int newValue);
    
    public override void _Ready()
    {
        _cardDeck = GetNode<CardDeck>("%CardDeck");
        _boardNode = GetNode("%Board");
        _hudController = GetNode<GameHUDController>("%GameHUD");
        
        // Collect all board spaces
        foreach (var child in _boardNode.GetChildren())
        {
            if (child is BoardSpace space)
            {
                _boardSpaces.Add(space);
            }
        }
        
        // Connect to signals from UI for game setup
        var menuController = GetNode<MainMenuController>("%MainMenu");
        //menuController.GameSetupRequested += OnGameSetupRequested;
    }
    
    private void OnGameSetupRequested(GameMode mode, int playerCount, bool useAI)
    {
        CurrentGameMode = mode;
        PlayerCount = playerCount;
        UseAI = useAI;
        
        StartGame();
    }
    
    public void StartGame()
    {
        _gameOver = false;
        _players.Clear();
        _currentPlayerIndex = 0;
        
        // Initialize the card deck
        _cardDeck.Initialize();
        
        // Create players
        for (int i = 0; i < PlayerCount; i++)
        {
            Player player;
            
            // If using AI and not the first player, create AI players
            if (UseAI && i > 0)
            {
                player = new AIPlayer($"AI {i}", (TamagotchiType)i);
            }
            else
            {
                player = new Player($"Player {i+1}", (TamagotchiType)i);
            }
            
            _players.Add(player);
            
            // Spawn player tokens on board
            SpawnPlayerToken(player, i);
            
            // Initialize growth meters
            player.InitializeGrowthMeter(CurrentGameMode);
            
            // Deal initial cards based on game mode
            if (CurrentGameMode == GameMode.Game1)
            {
                // Game 1: Deal 6 random cards
                for (int j = 0; j < 6; j++)
                {
                    Card card = _cardDeck.DrawCard();
                    player.AddCard(card);
                }
            }
            else
            {
                // Game 2: Give exactly one of each card type
                player.AddCard(_cardDeck.DrawSpecificCard(CardType.Food));
                player.AddCard(_cardDeck.DrawSpecificCard(CardType.Discipline));
                player.AddCard(_cardDeck.DrawSpecificCard(CardType.Medicine));
            }
        }
        
        // Signal that the game has started
        EmitSignal(SignalName.GameStarted, (int)CurrentGameMode, PlayerCount);
        
        // Start first player's turn
        StartPlayerTurn(_players[0]);
    }
    
    private void SpawnPlayerToken(Player player, int index)
    {
        // Instantiate player token scene
        var tokenScene = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/PlayerToken.tscn");
        var tokenInstance = tokenScene.Instantiate<PlayerToken>();
        
        // Set token properties
        tokenInstance.Initialize(player, index);
        
        // Add to board at starting position
        var startSpace = _boardSpaces.Find(space => space.SpaceIndex == 0);
        _boardNode.AddChild(tokenInstance);
        tokenInstance.GlobalPosition = startSpace.GlobalPosition;
        
        player.SetToken(tokenInstance);
    }
    
    public void StartPlayerTurn(Player player)
    {
        if (_gameOver) return;
        
        // Set current player
        _currentPlayerIndex = _players.IndexOf(player);
        
        // Update the UI to show current player
        //_hudController.UpdatePlayerInfo(player);
        
        // Signal that a turn has changed
        EmitSignal(SignalName.TurnChanged, player);
        
        // If AI, start its turn logic
        if (player is AIPlayer aiPlayer)
        {
            aiPlayer.StartTurn(this);
        }
    }
    
    public void RollDiceAndMove(Player player)
    {
        if (_gameOver || _players[_currentPlayerIndex] != player) return;
        
        // Roll the white die (1-6)
        int diceRoll = RollDie();
        
        // Find current and target spaces
        BoardSpace currentSpace = GetSpaceAtPosition(player.Token.GlobalPosition);
        int targetIndex = (currentSpace.SpaceIndex + diceRoll) % _boardSpaces.Count;
        BoardSpace targetSpace = _boardSpaces.Find(space => space.SpaceIndex == targetIndex);
        
        // Move the player's token
        player.MoveToken(targetSpace.GlobalPosition);
        
        // Check if another player is already on this space
        CheckForOtherPlayers(player, targetSpace);
        
        // Handle the space effect
        HandleSpaceEffect(player, targetSpace);
    }
    
    private void CheckForOtherPlayers(Player arrivingPlayer, BoardSpace space)
    {
        foreach (var player in _players)
        {
            if (player != arrivingPlayer && IsPlayerOnSpace(player, space))
            {
                // The player already on the space steals a card from the arriving player
                if (arrivingPlayer.GetCardCount() > 0)
                {
                    // If AI player, let them choose which card to steal
                    if (player is AIPlayer aiPlayer)
                    {
                        aiPlayer.StealCard(arrivingPlayer);
                    }
                    else
                    {
                        // For human player, show UI for choosing a card
                        //_hudController.ShowStealCardDialog(player, arrivingPlayer);
                    }
                }
                
                break;
            }
        }
    }
    
    private bool IsPlayerOnSpace(Player player, BoardSpace space)
    {
        // Check if player token position matches this space position
        return player.Token.GlobalPosition.DistanceTo(space.GlobalPosition) < 10.0f;
    }
    
    private BoardSpace GetSpaceAtPosition(Vector2 position)
    {
        // Find the closest board space to this position
        BoardSpace closest = _boardSpaces[0];
        float minDistance = position.DistanceTo(closest.GlobalPosition);
        
        foreach (var space in _boardSpaces)
        {
            float distance = position.DistanceTo(space.GlobalPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = space;
            }
        }
        
        return closest;
    }
    
    private void HandleSpaceEffect(Player player, BoardSpace space)
    {
        switch (space.SpaceType)
        {
            case SpaceType.Collect:
                HandleCollectSpace(player, space.CollectCardCount);
                break;
                
            case SpaceType.Care:
                HandleCareSpace(player, space.RequiredFood, space.RequiredDiscipline, space.RequiredMedicine);
                break;
                
            case SpaceType.Play:
                HandlePlaySpace(player);
                break;
                
            case SpaceType.Sleep:
                HandleSleepSpace(player);
                break;
                
            case SpaceType.Droppings:
                HandleDroppingsSpace(player);
                break;
        }
    }
    
    private void HandleCollectSpace(Player player, int cardCount)
    {
        // Draw the specified number of cards
        for (int i = 0; i < cardCount; i++)
        {
            Card card = _cardDeck.DrawCard();
            player.AddCard(card);
            
            // Signal that a card was drawn
            //EmitSignal(SignalName.CardPlayed, player, card.CardType);
        }
        
        EndTurn(player);
    }


    private void HandleCareSpace(Player player, int reqFood, int reqDiscipline, int reqMedicine)
    {
        bool canProvideAllCards = player.HasCards(reqFood, reqDiscipline, reqMedicine);
        
        if (canProvideAllCards)
        {
            // Player has all required cards - advance on growth meter
            player.RemoveCards(reqFood, reqDiscipline, reqMedicine);
            
            int totalCardsUsed = reqFood + reqDiscipline + reqMedicine;
            int advancement = CurrentGameMode == GameMode.Game1 ? totalCardsUsed : 1;
            
            // Advance on growth meter
            AdvanceGrowthMeter(player, advancement);
        }
        else
        {
            // Player doesn't have required cards - move back on growth meter
            AdvanceGrowthMeter(player, -2);
        }
        
        EndTurn(player);
    }
    
    private void HandlePlaySpace(Player player)
    {
        // Play mini-game with both dice
        int netAdvancement = 0;
        
        // Roll both dice 5 times
        for (int i = 0; i < 5; i++)
        {
            int playerRoll = RollDie(); // White die
            int tamagotchiRoll = RollDie(); // Colored die
            
            if (playerRoll > tamagotchiRoll)
            {
                netAdvancement++;
            }
            else if (tamagotchiRoll > playerRoll)
            {
                netAdvancement--;
            }
            // Ties do nothing
        }
        
        // Apply the result to growth meter
        AdvanceGrowthMeter(player, netAdvancement);
        
        EndTurn(player);
    }
    
    private void HandleSleepSpace(Player player)
    {
        // Mark player to skip their next turn
        player.SkipNextTurn = true;
        
        EndTurn(player);
    }
    
    private void HandleDroppingsSpace(Player player)
    {
        // Roll the die once
        int roll = RollDie();
        
        // Odd: advance 1, Even: retreat 1
        int advancement = roll % 2 == 1 ? 1 : -1;
        AdvanceGrowthMeter(player, advancement);
        
        EndTurn(player);
    }
    
    private void AdvanceGrowthMeter(Player player, int steps)
    {
        int oldValue = player.GrowthMeterPosition;
        player.AdvanceGrowthMeter(steps);
        int newValue = player.GrowthMeterPosition;
        
        // Signal that growth meter changed
        EmitSignal(SignalName.GrowthMeterChanged, player, oldValue, newValue);
        
        // Check win condition for Game 1
        if (CurrentGameMode == GameMode.Game1 && player.IsFullyGrown())
        {
            HandleGameOver(player);
            return;
        }
        
        // Check elimination condition for Game 2
        if (CurrentGameMode == GameMode.Game2 && player.IsAtStart())
        {
            EliminatePlayer(player);
        }
    }
    
    private void EliminatePlayer(Player player)
    {
        EmitSignal(SignalName.PlayerEliminated, player);
        
        // Remove player from active list
        _players.Remove(player);
        
        // Check if only one player remains
        if (_players.Count == 1)
        {
            HandleGameOver(_players[0]);
        }
    }
    
    private void HandleGameOver(Player winner)
    {
        _gameOver = true;
        EmitSignal(SignalName.GameOver, winner);
    }
    
    private int RollDie()
    {
        // Generate random number between 1 and 6
        return new Random().Next(1, 7);
    }
    
    public void EndTurn(Player player)
    {
        if (_gameOver) return;
        
        // Move to next player
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        
        // Skip players marked to skip their turn
        while (_players[_currentPlayerIndex].SkipNextTurn)
        {
            _players[_currentPlayerIndex].SkipNextTurn = false; // Reset the flag
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        }
        
        // Start the next player's turn
        StartPlayerTurn(_players[_currentPlayerIndex]);
    }
    
    public Player GetCurrentPlayer()
    {
        return _players[_currentPlayerIndex];
    }
}

public enum GameMode
{
    Game1, // Grow-up race
    Game2  // Survival mode
}

public enum TamagotchiType
{
    Blue,
    Red,
    Pink,
    Yellow
}