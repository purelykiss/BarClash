using Godot;
using System;
using System.Collections.Generic;

public partial class WallCollision : Node
{
    Dictionary<Direction4, bool>_isColliding = new Dictionary<Direction4, bool>();
    Dictionary<Direction4, OneSideWallCollision> _DirAndCol = new Dictionary<Direction4, OneSideWallCollision>();
    [Export] OneSideWallCollision CollisionUp;
    [Export] OneSideWallCollision CollisionDown;
    [Export] OneSideWallCollision CollisionLeft;
    [Export] OneSideWallCollision CollisionRight;

    public Dictionary<Direction4, bool> IsColliding => _isColliding;


    public override void _Ready()
    {
        if (CollisionUp != null)
        {
            _isColliding.Add(Direction4.UP, false);
            _DirAndCol.Add(Direction4.UP, CollisionUp);
        }
        if (CollisionDown != null)
        {
            _isColliding.Add(Direction4.DOWN, false);
            _DirAndCol.Add(Direction4.DOWN, CollisionDown);
        }
        if (CollisionLeft != null)
        {
            _isColliding.Add(Direction4.LEFT, false);
            _DirAndCol.Add(Direction4.LEFT, CollisionLeft);
        }
        if (CollisionRight != null)
        {
            _isColliding.Add(Direction4.RIGHT, false);
            _DirAndCol.Add(Direction4.RIGHT, CollisionRight);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach(var item in _DirAndCol)
        {
            _isColliding[item.Key] = item.Value.IsColliding;
        }
    }
}
