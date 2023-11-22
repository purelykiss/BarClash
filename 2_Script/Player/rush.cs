using Godot;
using System;

public partial class rush : Node, IState
{
    [Export] PlayerMove _playerMove;
    [Export] string _id = "rush";
    [Export] int _priorityLevel = 3;
    [Export] int _staggerImmune = 3;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState = IState.Cancelable.HigherLevel;
    IState.Cancelable _curCancelState;

    IUnitMove _baseMove;
    FiniteStateMachine _fsm;

    bool flagAnimate = false;
    bool flagFirstCycle;

    [Export] float _duration = 2.6f;
    float _curDuration;

    [Export] bool _isTryingActivateAttack1;
    bool flagActivateAttack1;

    [Export] bool _isTryingActivateAttack2;
    bool flagActivateAttack2;


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

        _isTryingActivateAttack1 = false;
        flagActivateAttack1 = false;
        _isTryingActivateAttack2 = false;
        flagActivateAttack2 = false;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (_playerMove.IsTryingRush)
        {
            if (_playerMove.HasRush)
                return true;
        }

        return false;
    }

    public void Animation()
    {
        if (!flagAnimate)
        {
            flagAnimate = true;
            _baseMove.SetAnimation("rush", false);
        }
    }

    public void Move()
    {
        if(_isTryingActivateAttack1)
        {
            if(!flagActivateAttack1)
            {
                flagActivateAttack1 = true;
                _playerMove.AtkList.ActivateAttackBox("rush1");
            }
        }
        else
        {
            if(flagActivateAttack1)
            {
                flagActivateAttack1 = false;
                _playerMove.AtkList.DisableAttackBox("rush1");
            }
        }

        if (_isTryingActivateAttack2)
        {
            if (!flagActivateAttack2)
            {
                flagActivateAttack2 = true;
                _playerMove.AtkList.ActivateAttackBox("rush2");
            }
        }
        else
        {
            if (flagActivateAttack2)
            {
                flagActivateAttack2 = false;
                _playerMove.AtkList.DisableAttackBox("rush2");
            }
        }

        if (flagFirstCycle)
        {
            flagFirstCycle = false;
            _playerMove.IsTryingRush = false;
            _playerMove.HasRush = false;
            _curDuration = _duration;

            if (Input.IsActionPressed("player_left") && !Input.IsActionPressed("player_right"))
            {
                _playerMove.CurDirection = DirectionH.LEFT;
            }
            else if (Input.IsActionPressed("player_right") && !Input.IsActionPressed("player_left"))
            {
                _playerMove.CurDirection = DirectionH.RIGHT;
            }
        }
        else
        {
            _curDuration -= (float)GetPhysicsProcessDeltaTime();
            if (_curDuration < 0)
                _curDuration = 0;
        }

        switch (_baseMove.CurDirection)
        {
            case DirectionH.LEFT:
                _baseMove.CurVelocity = new Vector2(0, _baseMove.PreVelocity.Y);
                break;
            case DirectionH.RIGHT:
                _baseMove.CurVelocity = new Vector2(0, _baseMove.PreVelocity.Y);
                break;
        }

        if (!_baseMove.IsGrounded)
            _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;

        if (_playerMove.IsDead
            || _curDuration <= 0
            || (_curDuration <= 0.1f && (Input.IsActionPressed("player_left") || Input.IsActionPressed("player_right"))))
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

        if (_fsm.CheckCondition("guard"))
            return "guard";

        if (_fsm.CheckCondition("straight"))
            return "straight";

        if (_fsm.CheckCondition("stamp"))
            return "stamp";

        if (_fsm.CheckCondition("jumpMelee"))
            return "jumpMelee";

        if (_fsm.CheckCondition("dashJumpMelee1"))
            return "dashJumpMelee1";

        if (_fsm.CheckCondition("melee1"))
            return "melee1";

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

        _isTryingActivateAttack1 = false;
        flagActivateAttack1 = false;
        _isTryingActivateAttack2 = false;
        flagActivateAttack2 = false;

        _playerMove.AtkList.DisableAttackBox("rush1");
        _playerMove.AtkList.DisableAttackBox("rush2");
    }
}
