using Godot;
using System;

public partial class WarriorStamp1 : Node, IState, IEnemyPattern
{
    [Export] WarriorMove _warriorMove;
    [Export] string _id = "stamp1";
    [Export] int _priorityLevel = 2;
    [Export] int _staggerImmune = 1;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState = IState.Cancelable.HigherLevel;
    IState.Cancelable _curCancelState;

    IUnitMove _baseMove;
    FiniteStateMachine _fsm;

    [Export] int _availablePhase = 2;
    [Export] bool _isUnconditional = false;
    float _distance;
    float _curDistance;

    bool flagAnimate = false;
    bool flagFirstCycle;

    [Export] float _duration = 0.6f;
    float _curDuration;

    public string ID => _id;

    public IUnitMove BaseMove
    {
        get => _baseMove;
        set => _baseMove = value;
    }

    public FiniteStateMachine FSM
    {
        get => _fsm;
        set => _fsm = value;
    }

    public int PriorityLevel => _priorityLevel;

    public int StaggerImmune => _staggerImmune;

    int IState.CurStaggerImmune
    {
        get => _currentStaggerImmune;
        set => _currentStaggerImmune = value;
    }

    public IState.Cancelable CancelState => _cancelState;

    public IState.Cancelable CurCancelState
    {
        get => _curCancelState;
        set => _curCancelState = value;
    }

    public int AvailablePhase => _availablePhase;

    public bool IsUnconditional => _isUnconditional;

    public override void _Ready()
    {
        _baseMove = _warriorMove;
        _fsm = GetParent<FiniteStateMachine>();

        _curCancelState = _cancelState;
        _currentStaggerImmune = _staggerImmune;
        _curDuration = _duration;
        _distance = 0;
        _curDistance = 0;
        flagFirstCycle = true;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (!_warriorMove.FlagEnemyDetected)
            return false;

        if (_baseMove.IsGrounded)
        {
            if (_warriorMove.IsTryingAttack)
            {
                if (_warriorMove.NextPatternID == _id)
                {
                    if (Mathf.Abs(_warriorMove.TargetUnit.GlobalPosition.X - _baseMove.GlobalPosition.X) < 150)
                        return true;
                }
            }
        }

        return false;
    }

    public void Animation()
    {
        if (!flagAnimate)
        {
            flagAnimate = true;
            _baseMove.SetAnimation("jump", false);
        }
    }

    public void Move()
    {
        if (flagFirstCycle)
        {
            flagFirstCycle = false;
            _warriorMove.IsAttacking = true;
            _curDuration = _duration;
            _warriorMove.SndList.ActivateSound("dodge");

            CheckDistance();

            if (_warriorMove.TargetUnit.GlobalPosition.X < _baseMove.GlobalPosition.X)
                _warriorMove.CurDirection = DirectionH.LEFT;
            else
                _warriorMove.CurDirection = DirectionH.RIGHT;



            _baseMove.CurVelocity = Vector2.Up * _baseMove.JumpSpeed * 1.2f;
        }
        else
        {
            _curDuration -= (float)GetPhysicsProcessDeltaTime();
            if (_curDuration < 0)
                _curDuration = 0;

            CheckTravel();
            _baseMove.CurVelocity = new Vector2(0, _baseMove.PreVelocity.Y);
        }

        SetSpeed();

        if (!_baseMove.IsGrounded)
            _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;

        if (_warriorMove.IsDead
            || _curDuration <= 0)
            _curCancelState = IState.Cancelable.Always;
        else if (_curDuration <= 0.1f)
            _curCancelState = IState.Cancelable.SameLevel;
        else
            _curCancelState = _cancelState;
    }

    void CheckDistance()
    {
        if (!_warriorMove.FlagEnemyDetected)
            return;

        _distance = Mathf.Abs(_warriorMove.TargetUnit.GlobalPosition.X - _baseMove.GlobalPosition.X);

        float distance = Mathf.Abs(_warriorMove.TargetUnit.PreVelocity.X * 0.5f);

        _distance += distance;

        _curDistance = _distance;
    }

    void CheckTravel()
    {
        _curDistance -= Mathf.Abs(_baseMove.PreVelocity.X * (float)GetPhysicsProcessDeltaTime());
    }

    void SetSpeed()
    {
        float speed = _distance / 0.5f;
        switch (_warriorMove.CurDirection)
        {
            case DirectionH.LEFT:
                speed *= -1;
                break;
            case DirectionH.RIGHT:
                break;
        }

        if (_duration - _curDuration >= 0.5f)
            speed = 0;

        _baseMove.CurVelocity = new Vector2(speed, _baseMove.CurVelocity.Y);
    }

    public string Change()
    {
        if (_fsm.CheckCondition("dashJump"))
            return "dashJump";

        if (_fsm.CheckCondition("jump"))
            return "jump";

        if (_fsm.CheckCondition("stamp2"))
            return "stamp2";

        if (_fsm.CheckCondition("run"))
            return "run";

        if (_fsm.CheckCondition("walk"))
            return "walk";

        if (_fsm.CheckCondition("dashFall"))
            return "dashFall";

        if (_fsm.CheckCondition("fall"))
            return "fall";

        if (_fsm.CheckCondition("idle"))
            return "idle";

        return null;
    }

    public void OnFinish()
    {
        flagAnimate = false;
        flagFirstCycle = true;
        _curDuration = 0;
        _distance = 0;
        _curDistance = 0;
    }
}
