using Godot;
using System;
using static Godot.TextServer;

public partial class jumpMelee : Node, IState
{
    [Export] PlayerMove _playerMove;
    [Export] string _id = "jumpMelee";
    [Export] int _priorityLevel = 2;
    [Export] int _staggerImmune = 1;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState = IState.Cancelable.HigherLevel;
    IState.Cancelable _curCancelState;

    IUnitMove _baseMove;
    FiniteStateMachine _fsm;

    bool flagAnimate = false;
    bool flagFirstCycle;

    [Export] bool _isTryingActivateAttack;
    bool flagActivateAttack;


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
        flagFirstCycle = true;

        _isTryingActivateAttack = false;
        flagActivateAttack = false;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (_playerMove.IsRunning)
            return false;

        if (!_baseMove.IsGrounded)
        {
            if (_playerMove.IsTryingAttacking)
                return true;
        }
        else
        {
            if (_playerMove.IsTryingJump)
            {
                if (_playerMove.IsTryingAttacking)
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
            _baseMove.SetAnimation("jumpMelee", false);
        }
    }

    public void Move()
    {
        if (_isTryingActivateAttack)
        {
            if (!flagActivateAttack)
            {
                flagActivateAttack = true;
                _playerMove.AtkList.ActivateAttackBox(_id);
            }
        }
        else
        {
            if (flagActivateAttack)
            {
                flagActivateAttack = false;
                _playerMove.AtkList.DisableAttackBox(_id);
            }
        }

        _baseMove.CurVelocity = new Vector2(0, _baseMove.PreVelocity.Y);

        Vector2 direction = Vector2.Zero;
        if (flagFirstCycle)
        {
            flagFirstCycle = false;
            _playerMove.IsTryingAttacking = false;

            if (Input.IsActionPressed("player_left"))
            {
                _baseMove.CurDirection = DirectionH.LEFT;
                direction += Vector2.Left;
            }
            if (Input.IsActionPressed("player_right"))
            {
                _baseMove.CurDirection = DirectionH.RIGHT;
                direction += Vector2.Right;
            }
        }
        else
        {
            switch (_playerMove.CurDirection)
            {
                case DirectionH.LEFT:
                    if (Input.IsActionPressed("player_left"))
                        direction += Vector2.Left;
                    break;
                case DirectionH.RIGHT:
                    if (Input.IsActionPressed("player_right"))
                        direction += Vector2.Right;
                    break;
            }
        }

        _baseMove.CurVelocity += _baseMove.CurWalkSpeed * direction;

        if (!_baseMove.IsGrounded)
            _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;

        if (_playerMove.IsDead
            || _playerMove.IsGrounded)
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

        if (_fsm.CheckCondition("rush"))
            return "rush";

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

        _isTryingActivateAttack = false;
        flagActivateAttack = false;

        _playerMove.AtkList.DisableAttackBox("jumpMelee");
    }
}
