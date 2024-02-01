using Godot;
using System;
using System.Collections.Generic;

public partial class PublicRangeProjectile : Area2D, IAttackBox, IProjectile
{
    IUnitMove _baseMove => _atkList.BaseMove;
    AttackList _atkList;
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
    [Export] float _projectileSpeed;
    [Export] float _duration;
    float _curDuration;
    [Export] bool _isActivate;
    bool flagActivate;
    [Export] Sprite2D _Sprite;
    [Export] AnimationPlayer _AnimPlayer;
    [Export] DirectionH _spriteDirection = DirectionH.RIGHT;
    [Export] bool _hasFinishMotion = false;
    Vector2 _destination;
    Vector2 _curVelocity;
    Vector2 _preVelocity;
    bool _hasEffect;
    bool flagEffect;
    bool flagFirstMove;
    [Export] bool _isFinishMotionOver;
    bool flagFinishMotion;

    public string ID => _id;

    public int Damage => _damage;

    public float AtkCooltime => _atkCooltime;

    public float CurAtkCooltime => _curAtkCooltime;

    public Vector2 PushPower => _pushPower;

    public int Stagger => _stagger;

    public bool IsActivate { get => _isActivate; set => _isActivate = value; }

    public Vector2 Destination => _destination;

    public Vector2 CurVelocity { get => _curVelocity; set => _curVelocity = value; }

    public Vector2 PreVelocity => _preVelocity;

    public float ProjectileSpeed => _projectileSpeed;

    public override void _Ready()
    {
        TopLevel = true;

        _atkList = GetParent<AttackList>();
        Position = _atkList.GlobalPosition;

        if (!IsConnected(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered)))
            Connect(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered));

        if (!IsConnected(SignalName.BodyExited, new Callable(this, MethodName.OnBodyExited)))
            Connect(SignalName.BodyExited, new Callable(this, MethodName.OnBodyExited));

        if (_Sprite != null && _AnimPlayer != null)
        {
            _hasEffect = true;
            _Sprite.Visible = false;

            if (_atkList.AttackDirection == _spriteDirection)
                _Sprite.Scale = new Vector2(1, 1);
            else
                _Sprite.Scale = new Vector2(-1, 1);
        }
        else
            _hasEffect = false;

        flagEffect = false;
        _isFinishMotionOver = true;
        flagFinishMotion = true;

        _isActivate = false;
        flagActivate = false;
        Monitoring = false;

        _destination = Vector2.Inf;
        _curVelocity = Vector2.Zero;
        _preVelocity = Vector2.Zero;

        flagFirstMove = true;

        _curDuration = _duration;

        GD.Print(this.Name);
    }

    public void Initialize()
    {
        _UnitAndAttacktedInZone.Clear();
        _curVelocity = Vector2.Zero;
        _preVelocity = Vector2.Zero;
        Position = _atkList.GlobalPosition;
        _curDuration = _duration;
        flagFirstMove = true;
        _isFinishMotionOver = false;
        flagFinishMotion = false;
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body == _baseMove)
            return;

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
        if (body == _baseMove)
            return;

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
        bool flagActivate = CheckActivate();
        CheckEffect();
        CheckMove();
        if(flagActivate)
        {
            CheckAttack();
            CheckCooltime();
            CheckDuration();
        }
    }

    private bool CheckActivate()
    {
        bool flagValue;

        if (_isActivate)
        {
            //isFinishMotionOver이 false인 경우, 초기화 작업이 안되있어 해줘야함
            Activate();
            flagValue = true;
        }
        else
        {
            Disable();
            flagValue = false;
        }

        return flagValue;
    }

    void CheckEffect()
    {
        if (!_hasEffect)
            return;

        if (_isActivate)
        {
            if (!flagEffect)
            {
                flagEffect = true;
                _AnimPlayer.CurrentAnimation = "effect";
            }
        }
        else
        {
            if (flagEffect)
            {
                flagEffect = false;
                if(_hasFinishMotion)
                    _AnimPlayer.CurrentAnimation = "finish";
                else
                    _AnimPlayer.CurrentAnimation = "RESET";
            }
        }
    }

    public void CheckMove()
    {
        //발동조건:
        //         1. _hasFinishMotion = true >> !(_isActivate = false && _isFinishMotionOver = false)
        //         2. _hasFinishMotion = false >> !(_isActivate = false)

        if (_hasFinishMotion)
        {
            if (!_isActivate && !_isFinishMotionOver)
                return;
        }
        else
        {
            if (!_isActivate)
                return;
        }

        if (flagFirstMove)
        {
            flagFirstMove = false;
            _curVelocity = (_destination - _baseMove.GlobalPosition).Normalized();
            _curVelocity *= _projectileSpeed;
        }
        else
        {
            _curVelocity = _preVelocity;
        }

        GlobalPosition += _curVelocity * (float)GetPhysicsProcessDeltaTime();
        _preVelocity = _curVelocity;
        _curVelocity = Vector2.Zero;
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

    public void CheckDuration()
    {
        _curDuration -= (float)GetPhysicsProcessDeltaTime();
        if(_curDuration <= 0 )
            Disable();
    }

    public void Activate()
    {
        if (_destination == Vector2.Inf)
            return;

        if (!flagActivate)
        {
            flagActivate = true;

            Monitoring = true;
            _isActivate = true;
            Initialize();
        }
    }

    public void Disable()
    {
        if(flagActivate)
        {
            flagActivate = false;

            Monitoring = false;
            _isActivate = false;
        }
    }

    public void SetDestination(Vector2 point)
    {
        _destination = point;
        _Sprite.GlobalRotation = Mathf.Atan2(_destination.Y - _baseMove.GlobalPosition.Y, _destination.X - _baseMove.GlobalPosition.X);
    }
}
