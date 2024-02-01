using Godot;
using System;

/// <summary>
/// 나중에 방어력과 Onhit에 대한 것 추가할 것
/// </summary>
public partial class guard : Node, IState
{
    [Export] PlayerMove _playerMove;
    [Export] string _id = "guard";
    [Export] int _priorityLevel = 3;
    [Export] int _staggerImmune = 3;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState = IState.Cancelable.HigherLevel;
    IState.Cancelable _curCancelState;

    IUnitMove _baseMove;
    FiniteStateMachine _fsm;

    bool flagAnimate = false;
    bool flagFirstCycle;

    [Export] float _duration = 0.9f;
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


    public override void _Ready()
    {
        _baseMove = _playerMove;
        _fsm = GetParent<FiniteStateMachine>();

        _curCancelState = _cancelState;
        _currentStaggerImmune = _staggerImmune;
        _curDuration = _duration;
        flagFirstCycle = true;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (_baseMove.IsGrounded)
        {
            if (_playerMove.IsTryingGuard)
            {
                if(_playerMove.HasGuard)
                    return true;
            }
        }

        return false;
    }

    public void Animation()
    {
        if (!flagAnimate)
        {
            flagAnimate = true;
            _baseMove.SetAnimation("guard", false);
        }
    }

    public void Move()
    {
        _baseMove.CurVelocity = new Vector2(0, _baseMove.PreVelocity.Y);

        if (flagFirstCycle)
        {
            flagFirstCycle = false;
            _playerMove.IsTryingGuard = false;
            _playerMove.HasGuard = false;
            _playerMove.IsGuarding = true;
            _curDuration = _duration;
        }
        else
        {
            _curDuration -= (float)GetPhysicsProcessDeltaTime();
            if (_curDuration < 0)
                _curDuration = 0;
        }

        if (!_baseMove.IsGrounded)
            _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;

        if (_playerMove.IsDead
            || _curDuration <= 0
            || (_curDuration <= 0.5f && (Input.IsActionPressed("player_left") || Input.IsActionPressed("player_right"))))
            _curCancelState = IState.Cancelable.Always;
        else
            _curCancelState = _cancelState;
    }

    public string Change()
    {
        if (_fsm.CheckCondition("dodge"))
            return "dodge";

        if (_fsm.CheckCondition("dashJump"))
            return "dashJump";

        if (_fsm.CheckCondition("jump"))
            return "jump";

        if (_fsm.CheckCondition("straight"))
            return "straight";

        if (_fsm.CheckCondition("stamp"))
            return "stamp";

        if (_fsm.CheckCondition("jumpMelee"))
            return "jumpMelee";

        if (_fsm.CheckCondition("dashJumpMelee1"))
            return "dashJumpMelee1";

        if (_fsm.CheckCondition("melee1"))
            return "melee2";

        if (_fsm.CheckCondition("dashMelee"))
            return "dashMelee";

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
        _playerMove.IsGuarding = false;
    }
}
