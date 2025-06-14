using Godot;
using System;

public partial class CharacterVisual : Sprite2D
{

    private Vector2 baseOffset;
    private float shakeIntensity = 0.0f;
    private float shakeDamping = 10.0f;

    public override void _Ready()
    {
        baseOffset = Offset;

        var character = GetParent<Character>();
        character.OnTakeDamage += DamageVisual;
    }

    public override void _Process(double delta)
    {
        if (shakeIntensity > 0)
        {
            shakeIntensity = (float)Mathf.Lerp(shakeIntensity, 0, shakeDamping * delta);
            Offset = baseOffset + RandomOffset();
        }
    }


    private void DamageVisual(int health)
    {
        Modulate = Colors.Red;
        shakeIntensity = 10.0f;
        
        var tween = CreateTween();
        tween.TweenCallback(Callable.From(() => Modulate = Colors.White)).SetDelay(0.05f);
    }

    private Vector2 RandomOffset()
    {
        var x = GD.RandRange(-shakeIntensity, shakeIntensity);
        var y = GD.RandRange(-shakeIntensity, shakeIntensity);
        return new Vector2((float)x, (float)y);
    }
}