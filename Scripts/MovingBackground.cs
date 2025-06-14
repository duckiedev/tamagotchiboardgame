using Godot;
using System;

public partial class MovingBackground : TextureRect
{
    [Export] private float speed = 200f;
    [Export] private float extents = 1024f;
    private Vector2 startPos;

    [Export] private Gradient colorLerp;

    public override void _Ready()
    {
        startPos = Position;
    }

    public override void _Process(double delta)
    {
        Position = new Vector2((float)(Position.X + speed * delta), Position.Y);

        if (Position.X - startPos.X >= extents)
        {
            Position = startPos;
        }

        var time = Mathf.Sin(Time.GetUnixTimeFromSystem());
        time = (time + 1) / 2;
        
        Modulate = colorLerp.Sample((float)time);
    }


}
