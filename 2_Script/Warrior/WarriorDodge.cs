using Godot;
using System;

public partial class WarriorDodge : Node, IState
{
    [Export] WarriorMove _warriorMove;
    [Export] string _id = "dodge";
    [Export] int _priorityLevel = 5;
    [Export] int _staggerImmune = 3;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState = IState.Cancelable.HigherLevel;
    IState.Cancelable _curCancelState;

    IUnitMove _baseMove;
    FiniteStateMachine _fsm;

    bool flagAnimate = false;
    bool flagDownDodge = false;

    [Export] float _duration = 0.5f;
    float _curDuration;
    [Export] float speed = 400f;

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
        _baseMove = _warriorMove;
        _fsm = GetParent<FiniteStateMachine>();

        _curCancelState = _cancelState;
        _currentStaggerImmune = _staggerImmune;
        _curDuration = _duration;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (_warriorMove.IsDead)
            return false;

        if (_warriorMove.CurAiState == IUnitMove.AIState.ATTACK)
            return false;

        flagDownDodge = false;
        if (_warriorMove.IsTryingDodge)
        {
            if (_warriorMove.IsDown)
                flagDownDodge = true;

            if (flagDownDodge)
            {
                if (_warriorMove.HasDownDodge)
                    return true;
            }
            else
            {
                if (_warriorMove.HasDodge)
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
            _baseMove.SetAnimation("dodge", false);
        }
    }

    public void Move()
    {
        bool flagActivateDodge = false;
        if (flagDownDodge)
        {
            if (_warriorMove.HasDownDodge)
            {
                _warriorMove.HasDownDodge = false;
                flagActivateDodge = true;
            }
        }
        else
        {
            if (_warriorMove.HasDodge)
            {
                _warriorMove.HasDodge = false;
                flagActivateDodge = true;
            }
        }

        if (flagActivateDodge)
        {
            _curDuration = _duration;
            _warriorMove.IsDodging = true;
            if (_warriorMove.GlobalPosition.X < _warriorMove.TargetUnit.GlobalPosition.X)
            {
                _warriorMove.CurDirection = DirectionH.LEFT;
                _baseMove.CurVelocity = Vector2.Left * speed;
            }
            else
            {
                _warriorMove.CurDirection = DirectionH.RIGHT;
                _baseMove.CurVelocity = Vector2.Right * speed;
            }
        }
        else
        {
            _curDuration -= (float)GetPhysicsProcessDeltaTime();
            if (_curDuration < 0)
                _curDuration = 0;
            _baseMove.CurVelocity = new Vector2(Mathf.Lerp(_baseMove.PreVelocity.X, 0, 0.15f), _baseMove.PreVelocity.Y);
            _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;
        }

        if (_warriorMove.IsDead
            || _curDuration <= 0)
            _curCancelState = IState.Cancelable.Always;
        else
            _curCancelState = _cancelState;
    }

    public string Change()
    {
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
        _warriorMove.IsDodging = false;
        _curCancelState = CancelState;
        _curDuration = 0;
    }
}
