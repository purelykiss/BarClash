using Godot;
using System;

public partial class SuperArmerBar : Control
{
    [Export] WarriorMove _warriorMove;
    [Export] ColorRect _baseRect;
    [Export] ColorRect _curRect;



    public override void _Ready()
    {
        if (_warriorMove == null)
        {
            GD.PrintErr("SuperArmerBar: IUnitMove Missing");
            this.SetPhysicsProcess(false);
        }


        _baseRect.Size = Size;
        _curRect.Size = Size;
    }

    public override void _PhysicsProcess(double delta)
    {
        float rate = _warriorMove.CurSuperArmer / _warriorMove.SuperArmer;

        _curRect.Size = new Vector2(Mathf.Lerp(0, Size.X, rate), _curRect.Size.Y);
    }
}
