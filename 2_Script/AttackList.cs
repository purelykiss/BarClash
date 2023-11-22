using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// 공격 Area2D들을 모으고 담당하는 곳.<br/>반드시 모든 공격의 방향을 통일시키고 attackDirection에 기입할 것
/// </summary>
public partial class AttackList : Node2D
{
    IUnitMove _baseMove;
    [Export] DirectionH _attackDirection;
    List<IAttackBox> _AttackBoxes = new List<IAttackBox>();

    public IUnitMove BaseMove => _baseMove;
    public DirectionH AttackDirection => _attackDirection;

    public override void _Ready()
    {
        _baseMove = GetParent<IUnitMove>();
        foreach(var item in GetChildren())
        {
            if(item is IAttackBox)
            {
                _AttackBoxes.Add(item as IAttackBox);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if(_baseMove.CurDirection == _attackDirection)
            Scale = new Vector2(1, 1);
        else
            Scale = new Vector2(-1, 1);
    }

    public IAttackBox GetAttackBox(string id)
    {
        foreach(var item in _AttackBoxes)
        {
            if(item.ID == id)
            {
                return item;
            }
        }
        return null;
    }

    public void ActivateAttackBox(string id)
    {
        foreach (var item in _AttackBoxes)
        {
            if (item.ID == id)
            {
                IAttackBox AtkBox = (IAttackBox)item;
                AtkBox.Activate();
                return;
            }
        }
    }

    public void ActivateAttackBox(string id, Vector2 point)
    {
        foreach (var item in _AttackBoxes)
        {
            if (item.ID == id && item is IProjectile)
            {
                IProjectile AtkBox = (IProjectile)item;
                AtkBox.SetDestination(point);
                AtkBox.Activate();
                return;
            }
        }
    }

    public void DisableAttackBox(string id)
    {
        foreach (var item in _AttackBoxes)
        {
            if (item.ID == id)
            {
                IAttackBox AtkBox = (IAttackBox)item;
                AtkBox.Disable();
                return;
            }
        }
    }

    public void SetDestination(string id, Vector2 point)
    {
        foreach (var item in _AttackBoxes)
        {
            if (item.ID == id && item is IProjectile)
            {
                IProjectile AtkBox = (IProjectile)item;
                AtkBox.SetDestination(point);
                return;
            }
        }
    }
}
