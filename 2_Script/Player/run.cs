using Godot;
using System;

public partial class run : Node, IState
{
    [Export] PlayerMove _playerMove;
    [Export] string _id = "run";
    [Export] int _priorityLevel = 0;
    [Export] int _staggerImmune;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState = IState.Cancelable.HigherLevel;
    IState.Cancelable _curCancelState;

    IUnitMove _baseMove;
    FiniteStateMachine _fsm;

    bool flagAnimate = false;


    public string ID => _id;

    public IUnitMove BaseMove
    {
        get => _baseMove;
        set => _baseMove = value;
    }

    public IState.Cancelable CancelState => _cancelState;

    public IState.Cancelable CurCancelState
    {
        get => _curCancelState;
        set => _curCancelState = value;
    }

    public int PriorityLevel => _priorityLevel;

    public int StaggerImmune => _staggerImmune;

    public int CurStaggerImmune
    {
        get => _currentStaggerImmune;
        set => _currentStaggerImmune = value;
    }


    public override void _Ready()
    {
        _baseMove = _playerMove;
        _fsm = GetParent<FiniteStateMachine>();

        _curCancelState = _cancelState;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (!_playerMove.IsGrounded)
            return false;

        if (_playerMove.IsRunning)
            return true;

        return false;
    }

    public void Move()
    {
        Vector2 direction = Vector2.Zero;

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

        _baseMove.CurVelocity = new Vector2(_baseMove.CurRunSpeed * direction.X, _baseMove.PreVelocity.Y);

        if (!_baseMove.IsGrounded)
            _baseMove.CurVelocity += new Vector2(0, _baseMove.Gravity);

        if (direction.Length() < 0.1f
            || !_playerMove.IsRunning
            || !_playerMove.IsGrounded)
            _curCancelState = IState.Cancelable.Always;
        else
            _curCancelState = _cancelState;
    }

    public void Animation()
    {
        if (!flagAnimate)
        {
            flagAnimate = true;
            _baseMove.SetAnimation("run", false);
        }
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

        if (_fsm.CheckCondition("jumpMelee"))
            return "jumpMelee";

        if (_fsm.CheckCondition("dashJumpMelee1"))
            return "dashJumpMelee1";

        if (_fsm.CheckCondition("melee1"))
            return "melee1";

        if (_fsm.CheckCondition("dashMelee"))
            return "dashMelee";

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
        _curCancelState = _cancelState;
    }
}
