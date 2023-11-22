using Godot;
using System;
using System.Collections.Generic;

public partial class OneSideWallCollision : Node
{
    [Export] Direction4 _checkingDirection;
    List<RayCast2D> _rays = new List<RayCast2D>();
    bool _isColliding;

    public Direction4 CheckingDirection => _checkingDirection;
    public bool IsColliding => _isColliding;

    public override void _Ready()
    {
        foreach(var item in GetChildren())
        {
            if(item is RayCast2D)
            {
                _rays.Add((RayCast2D)item);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        _isColliding = false;
        foreach(var item in _rays)
        {
            if(item.IsColliding())
            {
                _isColliding = true;
                break;
            }
        }
    }
}
