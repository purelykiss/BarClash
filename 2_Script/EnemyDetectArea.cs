using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyDetectArea : Area2D
{
    IUnitMove _baseMove;
    List<IUnitMove> _units = new List<IUnitMove>();
    [Export] bool _isActivate;

    public bool IsActivate { get => _isActivate; set => _isActivate = value; }

    public List<IUnitMove> Units => _units;

    public override void _Ready()
    {
        _baseMove = GetParent<IUnitMove>();

        if (!IsConnected(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered)))
            Connect(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered));

        if (!IsConnected(SignalName.BodyExited, new Callable(this, MethodName.OnBodyExited)))
            Connect(SignalName.BodyExited, new Callable(this, MethodName.OnBodyExited));

        _isActivate = true;
        Monitoring = true;
        GD.Print(this.Name);
    }

    public void Initialize()
    {
        _units.Clear();
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body == _baseMove)
            return;

        if (body is IUnitMove)
        {
            IUnitMove unit = (IUnitMove)body;
            if (!_units.Contains(unit))
            {
                _units.Add(unit);
            }
            return;
        }
    }

    public void OnBodyExited(Node2D body)
    {
        if (body == _baseMove)
            return;

        if (body is IUnitMove)
        {
            IUnitMove unit = (IUnitMove)body;
            _units.Remove(unit);
            return;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!CheckActivate())
            return;
    }

    private bool CheckActivate()
    {
        if (_isActivate)
        {
            Activate();
            return true;
        }
        else
        {
            Disable();
            return false;
        }
    }

    public void Activate()
    {
        Monitoring = true;
        _isActivate = true;
    }

    public void Disable()
    {
        Monitoring = false;
        _isActivate = false;
        Initialize();
    }
}
