using Godot;
using System;

public partial class WarriorBeam1 : Node, IState, IEnemyPattern
{
    [Export] WarriorMove _warriorMove;
    [Export] string _id = "beam1";
    [Export] int _priorityLevel = 2;
    [Export] int _staggerImmune = 1;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState = IState.Cancelable.HigherLevel;
    IState.Cancelable _curCancelState;

    IUnitMove _baseMove;
    FiniteStateMachine _fsm;

    [Export] int _availablePhase = 1;
    [Export] bool _isUnconditional = true;  //원거리공격 없는 초기에 임의값. 나중에 원거리 공격이 추가되고 false로 바꿔야됨

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
        flagFirstCycle = true;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (_warriorMove.TargetUnit == null)
            return false;

        if (_baseMove.IsGrounded)
        {
            if (_warriorMove.IsTryingAttack)
            {
                if (_warriorMove.NextPatternID == _id)
                {
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
            _baseMove.SetAnimation("beam1", false);
        }
    }

    public void Move()
    {

        if (flagFirstCycle)
        {
            flagFirstCycle = false;
            _warriorMove.IsAttacking = true;
            _curDuration = _duration;

            _warriorMove.IsUsingBeam1 = true;
            _warriorMove.EfctList.ActivateEffect("charging");

            _baseMove.CurVelocity = new Vector2(0, _baseMove.PreVelocity.Y);

            if (_warriorMove.TargetUnit.GlobalPosition.X < _baseMove.GlobalPosition.X)
            {
                _warriorMove.CurDirection = DirectionH.LEFT;
                _warriorMove.CurVelocity += Vector2.Left * _warriorMove.CurRunSpeed * 2f;
            }
            else
            {
                _warriorMove.CurDirection = DirectionH.RIGHT;
                _warriorMove.CurVelocity += Vector2.Right * _warriorMove.CurRunSpeed * 2f;
            }
        }
        else
        {
            _baseMove.CurVelocity = new Vector2(Mathf.Lerp(_baseMove.PreVelocity.X, 0, 0.1f), _baseMove.PreVelocity.Y);

            _curDuration -= (float)GetPhysicsProcessDeltaTime();
            if (_curDuration < 0)
                _curDuration = 0;
        }

        if (!_baseMove.IsGrounded)
            _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;

        if (_warriorMove.IsDead
            || _curDuration <= 0
            || !_warriorMove.IsGrounded)
            _curCancelState = IState.Cancelable.Always;
        else if (_curDuration <= 0.1f)
            _curCancelState = IState.Cancelable.SameLevel;
        else
            _curCancelState = _cancelState;
    }

    public string Change()
    {
        if (_fsm.CheckCondition("dashJump"))
            return "dashJump";

        if (_fsm.CheckCondition("jump"))
            return "jump";

        if (_fsm.CheckCondition("beam2"))
            return "beam2";

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

        _warriorMove.IsUsingBeam1 = false;
        _warriorMove.EfctList.DisableEffect("charging");
    }
}
