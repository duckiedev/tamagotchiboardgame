using Godot;
using System;

public partial class Die : RigidBody3D
{

    [Signal] delegate void RollFinishedEventHandler(int oppositeSide);

    private Vector3 startPosition;
    private float rollStrength = 10.0f; // Added default value

    private Godot.Collections.Array<Node> raycasts;

    private bool isRolling = false;

    public override void _Ready()
    {
        startPosition = GlobalPosition;
        raycasts = GetNode<Node3D>("RayCasts").GetChildren();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && !isRolling)
        {
            Roll();
        }
    }

    private void Roll()
    {
        // Reset state
        Sleeping = false;
        Freeze = false;
        GlobalPosition = startPosition;
        LinearVelocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;

        // Method 1: Create a new transform and assign it
        var newTransform = Transform;
        newTransform.Origin = startPosition;

        // Apply random rotations
        var randomRotationX = Basis.FromEuler(new Vector3((float)GD.RandRange(0, 2 * Math.PI), 0, 0));
        var randomRotationY = Basis.FromEuler(new Vector3(0, (float)GD.RandRange(0, 2 * Math.PI), 0));
        var randomRotationZ = Basis.FromEuler(new Vector3(0, 0, (float)GD.RandRange(0, 2 * Math.PI)));

        newTransform.Basis = randomRotationX * randomRotationY * randomRotationZ;
        Transform = newTransform;

        // Random throw pulse
        var throwVector = new Vector3(
            GD.RandRange(-1, 1),
            0,
            GD.RandRange(-1, 0)
        ).Normalized();

        AngularVelocity = throwVector * rollStrength / 2;
        ApplyCentralImpulse(throwVector * rollStrength);
        isRolling = true;
    }

    private void _on_sleeping_state_changed()
    {
        if (Sleeping)
        {
            var landedOnSide = false;
            foreach (var item in raycasts)
            {
                var raycast = item as DieRayCast;
                if (raycast.IsColliding())
                {
                    EmitSignal(SignalName.RollFinished, raycast.OppositeSide);
                    isRolling = false;
                    landedOnSide = true;
                }
            }
            if (!landedOnSide) Roll();
        }
    }
}