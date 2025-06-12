using Godot;
using System;

public partial class HealthBar : ProgressBar
{
    private Label healthText;

    public override void _Ready()
    {
        healthText = GetNode<Label>("HealthText");
        Character character = GetParent<Character>();
        MaxValue = character.MaxHealth;
        UpdateValue(character.Health);
        character.OnHeal += UpdateValue;
        character.OnTakeDamage += UpdateValue;
    }

    public void UpdateValue(int health)
    {
        GD.Print("boop");
        GD.Print(health);
        Value = health;
        healthText.Text = health.ToString() + " / " + MathF.Round((float)MaxValue).ToString();
    }

}
