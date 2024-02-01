using Godot;
using System;

public partial class idle : Node, IState
{
    [Export] PlayerMove _playerMove;
    [Export] string _id = "idle";
    [Export] int _priorityLevel = 0;
    [Export] int _staggerImmune;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState;
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
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (_baseMove.IsGrounded)
        {
            return true;
        }

        return false;
    }

    public void Animation()
    {
        if(!flagAnimate)
        {
            flagAnimate = true;
            _baseMove.SetAnimation("idle", false);
        }
    }

    public void Move()
    {
        _baseMove.CurVelocity += new Vector2(0, _baseMove.PreVelocity.Y);
        if (!_baseMove.IsGrounded)
            _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;
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

        if (_fsm.CheckCondition("run"))
            return "run";

        if (_fsm.CheckCondition("walk"))
            return "walk";

        if(_fsm.CheckCondition("dashFall"))
            return "dashFall";

        if (_fsm.CheckCondition("fall"))
            return "fall";

        return null;
    }

    public void OnFinish()
    {
        flagAnimate = false;
    }
}
