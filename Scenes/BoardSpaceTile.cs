using Godot;
using System;

public partial class BoardSpaceTile : Area3D
{
    public enum TYPE
    {
        START,
        STANDARD,
        END
    }

    [Export(PropertyHint.Enum)] public TYPE type;

    private CsgMesh3D mesh;
    private StandardMaterial3D material;
    private Color defaultColor;
    [Export] private Color highlightedColor;

    public override void _Ready()
    {
        BodyEntered += OnPlayerEntered;
        BodyExited += OnPlayerExited;
        mesh = GetNode<CsgMesh3D>("CSGMesh3D");
        material = mesh.Material.Duplicate(true) as StandardMaterial3D;
        mesh.Material = material;
        defaultColor = material.AlbedoColor;
    }

    private void OnPlayerEntered(Node3D node3d)
    {
        GD.Print("something there");
        if (node3d is PlayerCharacter)
        {
            material.AlbedoColor = highlightedColor;
        }
    }

    private void OnPlayerExited(Node3D node3d)
    {
        if (node3d is PlayerCharacter)
        {
            material.AlbedoColor = defaultColor;
        }
    }
}

