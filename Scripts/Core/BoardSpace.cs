using Godot;
using System;

public partial class BoardSpace : Node2D
{
    [Export] public int SpaceIndex { get; set; } = 0;
    [Export] public SpaceType SpaceType { get; set; } = SpaceType.Collect;
    
    // Properties for Collect spaces
    [Export] public int CollectCardCount { get; set; } = 1;
    
    // Properties for Care spaces
    [Export] public int RequiredFood { get; set; } = 0;
    [Export] public int RequiredDiscipline { get; set; } = 0;
    [Export] public int RequiredMedicine { get; set; } = 0;
    
    // Visual elements
    private Sprite2D _spaceSprite;
    private Label _spaceLabel;
    
    public override void _Ready()
    {
        _spaceSprite = GetNode<Sprite2D>("%SpaceSprite");
        _spaceLabel = GetNode<Label>("%SpaceLabel");
        
        UpdateVisuals();
    }
    
    // Update visual appearance based on space type
    private void UpdateVisuals()
    {
        if (_spaceSprite == null || _spaceLabel == null)
            return;
            
        // Set the space number
        _spaceLabel.Text = SpaceIndex.ToString();
        
        // Set color and icon based on space type
        switch (SpaceType)
        {
            case SpaceType.Collect:
                _spaceSprite.Modulate = Colors.Green;
                _spaceLabel.Text += $"\nCollect {CollectCardCount}";
                break;
                
            case SpaceType.Care:
                _spaceSprite.Modulate = Colors.Blue;
                string requirements = "";
                if (RequiredFood > 0) requirements += $"F:{RequiredFood} ";
                if (RequiredDiscipline > 0) requirements += $"D:{RequiredDiscipline} ";
                if (RequiredMedicine > 0) requirements += $"M:{RequiredMedicine}";
                _spaceLabel.Text += $"\nCare {requirements}";
                break;
                
            case SpaceType.Play:
                _spaceSprite.Modulate = Colors.Yellow;
                _spaceLabel.Text += "\nPlay";
                break;
                
            case SpaceType.Sleep:
                _spaceSprite.Modulate = Colors.Purple;
                _spaceLabel.Text += "\nSleep";
                break;
                
            case SpaceType.Droppings:
                _spaceSprite.Modulate = Colors.Brown;
                _spaceLabel.Text += "\nDroppings";
                break;
        }
    }
    
    // Get a description of this space
    public string GetDescription()
    {
        switch (SpaceType)
        {
            case SpaceType.Collect:
                return $"Collect {CollectCardCount} attention cards";
                
            case SpaceType.Care:
                string requirements = "";
                if (RequiredFood > 0) requirements += $"{RequiredFood} Food, ";
                if (RequiredDiscipline > 0) requirements += $"{RequiredDiscipline} Discipline, ";
                if (RequiredMedicine > 0) requirements += $"{RequiredMedicine} Medicine";
                
                // Remove trailing comma if needed
                if (requirements.EndsWith(", "))
                    requirements = requirements.Substring(0, requirements.Length - 2);
                    
                return $"Care for your pet: Requires {requirements}";
                
            case SpaceType.Play:
                return "Play a mini-game! Roll both dice 5 times.";
                
            case SpaceType.Sleep:
                return "Your pet needs to sleep. Skip next turn.";
                
            case SpaceType.Droppings:
                return "Clean up droppings. Roll a die: odd = success, even = fail.";
                
            default:
                return "Unknown space";
        }
    }
}

public enum SpaceType
{
    Collect,
    Care,
    Play,
    Sleep,
    Droppings
}