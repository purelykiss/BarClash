using Godot;
using System;

public partial class dodge : Node, IState
{
    [Export] PlayerMove _playerMove;
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

    [Export] float _duration = 0.35f;
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
        _baseMove = _playerMove;
        _fsm = GetParent<FiniteStateMachine>();

        _curCancelState = _cancelState;
        _currentStaggerImmune = _staggerImmune;
        _curDuration = _duration;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (_playerMove.IsDead)
            return false;

        flagDownDodge = false;
        if (Input.IsActionPressed("player_dodge"))
        {
            if (_playerMove.IsDown)
                flagDownDodge = true;

            if(flagDownDodge)
            {
                if (_playerMove.HasDownDodge)
                    return true;
            }
            else
            {
                if (_playerMove.HasDodge)
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
        if(flagDownDodge)
        {
            if(_playerMove.HasDownDodge)
            {
                _playerMove.HasDownDodge = false;
                flagActivateDodge = true;
            }
        }
        else
        {
            if(_playerMove.HasDodge)
            {
                _playerMove.HasDodge = false;
                flagActivateDodge = true;
            }
        }

        if(flagActivateDodge)
        {
            _curDuration = _duration;
            _playerMove.IsDodging = true;
            if (Input.IsActionPressed("player_left") && !Input.IsActionPressed("player_right"))
            {
                _playerMove.CurDirection = DirectionH.LEFT;
                _baseMove.CurVelocity = Vector2.Left * speed;
            }
            else if (Input.IsActionPressed("player_right") && !Input.IsActionPressed("player_left"))
            {
                _playerMove.CurDirection = DirectionH.RIGHT;
                _baseMove.CurVelocity = Vector2.Right * speed;
            }
            else
            {
                switch(_playerMove.CurDirection)
                {
                    case DirectionH.LEFT:
                        _baseMove.CurVelocity = Vector2.Left * speed;
                        break;
                    case DirectionH.RIGHT:
                        _baseMove.CurVelocity = Vector2.Right * speed;
                        break;
                }
            }
        }
        else
        {
            _curDuration -= (float)GetPhysicsProcessDeltaTime();
            if (_curDuration < 0)
                _curDuration = 0;
            _baseMove.CurVelocity = new Vector2(Mathf.Lerp(_baseMove.PreVelocity.X, 0, 0.2f), _baseMove.PreVelocity.Y);
            _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;
        }

        if (_playerMove.IsDead
            || _curDuration <= 0
            || Input.IsActionPressed("player_jump"))
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
        _playerMove.IsDodging = false;
        _curCancelState = CancelState;
        _curDuration = 0;
    }
}
