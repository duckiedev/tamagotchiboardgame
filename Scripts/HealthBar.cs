using Godot;
using System;

public partial class HealthBar : ProgressBar
{
    private Label healthText;
    private Timer timer;
    private Character character;
    private ProgressBar damageBar;

    public override void _Ready()
    {
        healthText = GetNode<Label>("HealthText");
        timer = GetNode<Timer>("Timer");
        character = GetParent<Character>();
        damageBar = GetNode<ProgressBar>("DamageBar");

        InitHealth();
        MaxValue = character.MaxHealth;
        UpdateValue(character.Health);
        character.OnHeal += UpdateValue;
        character.OnTakeDamage += UpdateValue;
    }

    public void InitHealth()
    {
        MaxValue = character.MaxHealth;
        Value = character.Health;
        damageBar.MaxValue = Value;
        damageBar.Value = Value;
    }

    public void UpdateValue(int _health)
    {
        var prevHealth = character.Health;
        var health = Math.Min(MaxValue, _health);

        Value = health;

        if (health <= 0) QueueFree();

        if (health < prevHealth)
        {
            timer.Start();
        }
        else
        {
            damageBar.Value = health;
        }

        healthText.Text = _health.ToString() + " / " + MathF.Round((float)MaxValue).ToString();
    }

    private void _on_timer_timeout()
    {
        damageBar.Value = Value;
    }

}
