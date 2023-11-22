using Godot;
using System;
using System.Collections.Generic;

public partial class StaggerTestAttack : Area2D, IAttackBox
{
    [Export] string _id;
    Dictionary<IUnitMove, (bool, bool)> _UnitAndAttacktedInZone = new Dictionary<IUnitMove, (bool, bool)>();
    [Export] int _damage;
    [Export] float _atkCooltime;
    float _curAtkCooltime;
    [Export] Vector2 _pushPower;
    /// <summary>
    /// 0: 없음 1: 약경직 2: 강경직 3: 다운
    /// </summary>
    [Export] int _stagger;
    [Export] bool _isActivate;

    public string ID => _id;

    public int Damage => _damage;

    public float AtkCooltime => _atkCooltime;

    public float CurAtkCooltime => _curAtkCooltime;

    public Vector2 PushPower => _pushPower;

    public int Stagger => _stagger;

    public bool IsActivate { get => _isActivate; set => _isActivate = value; }

    public override void _Ready()
    {
        if (!IsConnected(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered)))
            Connect(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered));

        if (!IsConnected(SignalName.BodyExited, new Callable(this, MethodName.OnBodyExited)))
            Connect(SignalName.BodyExited, new Callable(this, MethodName.OnBodyExited));

        Monitoring = true;
        GD.Print(this.Name);
    }

    public void Initialize()
    {
        _UnitAndAttacktedInZone.Clear();
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is IUnitMove)
        {
            IUnitMove unit = (IUnitMove)body;
            if (!_UnitAndAttacktedInZone.ContainsKey(unit))
            {
                _UnitAndAttacktedInZone.Add(unit, (false, true));
            }
            else
            {
                _UnitAndAttacktedInZone[unit] = (_UnitAndAttacktedInZone[unit].Item1, true);
            }
            return;
        }
    }

    public void OnBodyExited(Node2D body)
    {
        if (body is IUnitMove)
        {
            IUnitMove unit = (IUnitMove)body;
            if (_UnitAndAttacktedInZone.ContainsKey(unit))
            {
                _UnitAndAttacktedInZone[unit] = (_UnitAndAttacktedInZone[unit].Item1, false);
            }
            return;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!CheckActivate())
            return;
        CheckAttack();
        CheckCooltime();
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

    void CheckAttack()
    {
        foreach (var item in _UnitAndAttacktedInZone)
        {
            bool flagAttack = false;
            if (!item.Value.Item1 && item.Value.Item2)
            {
                _UnitAndAttacktedInZone[item.Key] = (true, item.Value.Item2);
                flagAttack = true;
            }

            if (flagAttack)
            {
                item.Key.OnHit(_damage, GlobalPosition, _pushPower, _stagger);
            }
        }
    }

    void CheckCooltime()
    {
        _curAtkCooltime -= (float)GetPhysicsProcessDeltaTime();

        bool flag = false;
        if (_curAtkCooltime < 0)
        {
            _curAtkCooltime = _atkCooltime;
            flag = true;
        }

        if (flag)
        {
            foreach (var item in _UnitAndAttacktedInZone)
            {
                _UnitAndAttacktedInZone[item.Key] = (false, item.Value.Item2);
            }
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
