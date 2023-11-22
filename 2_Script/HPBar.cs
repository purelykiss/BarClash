using Godot;
using System;

public partial class HPBar : Control
{
    [Export] Node unitMove;
    [Export] ColorRect _baseRect;
    [Export] ColorRect _curRect;
    IUnitMove _unit;



    public override void _Ready()
    {
        if(unitMove is IUnitMove)
            _unit = unitMove as IUnitMove;
        else
        {
            GD.PrintErr("HPBar: IUnitMove Missing");
            this.SetPhysicsProcess(false);
        }

        _baseRect.Size = Size;
        _curRect.Size = Size;
    }

    public override void _PhysicsProcess(double delta)
    {
        float rate = _unit.CurHitPoint / _unit.HitPoint;

        _curRect.Size = new Vector2(Mathf.Lerp(0, Size.X, rate), _curRect.Size.Y);
    }
}
