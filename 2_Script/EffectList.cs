using Godot;
using System;
using System.Collections.Generic;

public partial class EffectList : Node2D
{
    IUnitMove _baseMove;
    [Export] DirectionH _effectDirection = DirectionH.RIGHT;
    List<IEffect> _Effects = new List<IEffect>();

    public IUnitMove BaseMove => _baseMove;
    public DirectionH AttackDirection => _effectDirection;

    public override void _Ready()
    {
        _baseMove = GetParent<IUnitMove>();
        foreach (var item in GetChildren())
        {
            if (item is IEffect)
            {
                _Effects.Add(item as IEffect);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_baseMove.CurDirection == _effectDirection)
            Scale = new Vector2(1, 1);
        else
            Scale = new Vector2(-1, 1);
    }

    public IEffect GetEffect(string id)
    {
        foreach (var item in _Effects)
        {
            if (item.ID == id)
            {
                return item;
            }
        }
        return null;
    }

    public void ActivateEffect(string id)
    {
        foreach (var item in _Effects)
        {
            if (item.ID == id)
            {
                IEffect tmpEffect = (IEffect)item;
                tmpEffect.Activate();
                return;
            }
        }
    }

    public void DisableEffect(string id)
    {
        foreach (var item in _Effects)
        {
            if (item.ID == id)
            {
                IEffect tmpEffect = (IEffect)item;
                tmpEffect.Disable();
                return;
            }
        }
    }
}
