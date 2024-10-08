using Godot;
using System;

public partial class jump : Node, IState
{
    [Export] PlayerMove _playerMove;
    [Export] string _id = "jump";
    [Export] int _priorityLevel = 1;
    [Export] int _staggerImmune;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState = IState.Cancelable.HigherLevel;
    IState.Cancelable _curCancelState;

    IUnitMove _baseMove;
    FiniteStateMachine _fsm;

    bool flagAnimate = false;

    bool flagJumpable = false;

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
        flagJumpable = false;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (_playerMove.IsRunning)
            return false;

        if (_playerMove.IsTryingJump)
        {
            flagJumpable = true;
            return true;
        }

        if (!_playerMove.IsGrounded)
        {
            if (_playerMove.PreVelocity.Y <= -0.01f)
                return true;
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
        Vector2 direction = Vector2.Zero;

        _baseMove.CurVelocity += new Vector2(0, _baseMove.PreVelocity.Y);

        bool flagJumpThisFrame = false;
        if (_playerMove.IsTryingJump
            && _playerMove.CurJumpBufferTime > 0
            && !_playerMove.FlagJumpedThisPress)
        {
            flagJumpable = true;
        }

        if(flagJumpable)
        {
            if (_playerMove.HasGroundJump)
                _playerMove.HasGroundJump = false;
            else if (_playerMove.CurAirJumpCnt > 0)
                _playerMove.CurAirJumpCnt--;
            else
                flagJumpable = false;

            if(flagJumpable)
            {
                _playerMove.CurVelocity = Vector2.Up * _playerMove.JumpSpeed;

                flagJumpThisFrame = true;
                _playerMove.FlagJumpedThisPress = true;

                flagJumpable = false;
            }
        }

        if (!_baseMove.IsGrounded && !flagJumpThisFrame)
        {
            if(_playerMove.IsTryingQuickLanding)
                _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity * 2f;
            else
                _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;
        }

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

        _baseMove.CurVelocity += _baseMove.CurWalkSpeed * direction;

        if (_playerMove.CurVelocity.Y > -0.01f
            || _playerMove.IsRunning)
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

        if (_fsm.CheckCondition("guard"))
            return "guard";

        if (_fsm.CheckCondition("straight"))
            return "straight";

        if (_fsm.CheckCondition("stamp"))
            return "stamp";

        if (_fsm.CheckCondition("rush"))
            return "rush";

        if (_fsm.CheckCondition("jumpMelee"))
            return "jumpMelee";

        if (_fsm.CheckCondition("dashJumpMelee1"))
            return "dashJumpMelee1";

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
        _curCancelState = CancelState;
    }
}
