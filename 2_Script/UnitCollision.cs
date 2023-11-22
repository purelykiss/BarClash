using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 유닛의 CharacterBody2D의 그룹을 확인하고 적이면 리스트에 넣는다.
/// </summary>
public partial class UnitCollision : Area2D
{
    IUnitMove _baseMove;
    string _enemyAffiliation;
    List<Node2D> _OverlapingUnits;

    public List<Node2D> OverlapingUnits => _OverlapingUnits;

    public override void _Ready()
    {
        _baseMove = GetParent<IUnitMove>();
        _enemyAffiliation = _baseMove.EnemyAffiliation;

        if (!IsConnected(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered)))
            Connect(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered));
        if (!IsConnected(SignalName.BodyExited, new Callable(this, MethodName.OnBodyExited)))
            Connect(SignalName.BodyExited, new Callable(this, MethodName.OnBodyExited));

        _OverlapingUnits = new List<Node2D>();
    }

    public void OnBodyEntered(Node2D body)
    {
        GD.Print("Overlaping!");
        if (body.Get("_affiliation").AsString() == _enemyAffiliation)
            OverlapingUnits.Add(body);
    }

    public void OnBodyExited(Node2D body)
    {
        GD.Print("Passed by");
        if (body.Get("_affiliation").AsString() == _enemyAffiliation)
            OverlapingUnits.Remove(body);
    }
}
