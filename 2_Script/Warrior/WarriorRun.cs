using Godot;
using System;

public partial class WarriorRun : Node, IState
{
    [Export] WarriorMove _warriorMove;
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
        _baseMove = _warriorMove;
        _fsm = GetParent<FiniteStateMachine>();

        _curCancelState = _cancelState;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (!_warriorMove.IsGrounded)
            return false;

        if (!_warriorMove.IsRunning)
            return false;

        if (_warriorMove.IsTryingChase)
            return true;

        return false;
    }

    public void Move()
    {
        Vector2 direction = Vector2.Zero;

        if (_warriorMove.TargetUnit != null)
        {
            if (_baseMove.GlobalPosition.X > _warriorMove.TargetUnit.GlobalPosition.X)
            {
                _baseMove.CurDirection = DirectionH.LEFT;
                direction += Vector2.Left;
            }
            else
            {
                _baseMove.CurDirection = DirectionH.RIGHT;
                direction += Vector2.Right;
            }
        }

        if (direction.Length() < 0.1f
            || !_warriorMove.IsGrounded
            || !_warriorMove.IsRunning
            || _warriorMove.TargetUnit == null
            || !_warriorMove.IsTryingChase)
            _curCancelState = IState.Cancelable.Always;
        else
            _curCancelState = _cancelState;

        _baseMove.CurVelocity = new Vector2(_baseMove.CurRunSpeed * direction.X, _baseMove.PreVelocity.Y);

        if (!_baseMove.IsGrounded)
            _baseMove.CurVelocity += new Vector2(0, _baseMove.Gravity);
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

        if (_fsm.CheckCondition("slash1"))
            return "slash1";

        if (_fsm.CheckCondition("sweep1"))
            return "sweep1";

        if (_fsm.CheckCondition("beam1"))
            return "beam1";

        if (_fsm.CheckCondition("stamp1"))
            return "stamp1";

        if (_fsm.CheckCondition("melee1"))
            return "melee1";

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
