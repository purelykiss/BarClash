using Godot;
using System;

public partial class downInAir : Node, IState, IStagger
{
    [Export] PlayerMove _playerMove;
    [Export] string _id = "downInAir";
    [Export] int _priorityLevel = 5;
    [Export] int _staggerImmune = 0;
    int _currentStaggerImmune;
    [Export] IState.Cancelable _cancelState = IState.Cancelable.SameLevel;
    IState.Cancelable _curCancelState;

    IUnitMove _baseMove;
    FiniteStateMachine _fsm;

    bool flagAnimate;
    bool flagFirstCycle;
    bool flagDownFront;

    [Export] float _duration = 3;
    float _curDuration;

    HitData _curHitData;

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

        flagAnimate = false;
        flagFirstCycle = false;
        flagDownFront = false;

        _currentStaggerImmune = _staggerImmune;
        _curCancelState = _cancelState;

        _curDuration = 0;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        //다운(지상)조건:
        //-사망시 상시
        //-생존시 다운x:
        //1. 피격면역 < 피격이상
        //2. 피격이상 == 3
        //-생존시 다운o:
        //모든 피격이상 3 고정
        if (_playerMove.IsGrounded)
            return false;

        if (_playerMove.IsDead)
        {
            if (!flagFirstCycle)
                return true;
        }
        else
        {
            if(_playerMove.IsDown && !flagFirstCycle)
                return true;

            if (_playerMove.CurStaggerImmune < _playerMove.HitList[0].Stagger)
            {
                if (_playerMove.HitList[0].Stagger == 3)
                {
                    _curHitData = _playerMove.HitList[0];
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
            if (flagDownFront)
                _baseMove.SetAnimation("downInAirFront", true);
            else
                _baseMove.SetAnimation("downInAirBack", true);
        }
    }

    public void Move()
    {
        if (flagFirstCycle)
        {
            flagFirstCycle = false;
            _curDuration = _duration;
            _baseMove.IsDown = true;

            if (_curHitData.PushDir == _playerMove.CurDirection)
                flagDownFront = false;
            else
                flagDownFront = true;

            switch (_curHitData.PushDir)
            {
                case DirectionH.LEFT:
                    _baseMove.CurVelocity = Vector2.Left * _curHitData.PushPower.X + Vector2.Up * _curHitData.PushPower.Y;
                    break;
                case DirectionH.RIGHT:
                    _baseMove.CurVelocity = Vector2.Right * _curHitData.PushPower.X + Vector2.Up * _curHitData.PushPower.Y;
                    break;
            }
        }
        else
        {
            _curDuration -= (float)GetPhysicsProcessDeltaTime();

            _baseMove.CurVelocity += new Vector2(Mathf.Lerp(_baseMove.PreVelocity.X, 0, 0.1f), _baseMove.PreVelocity.Y);

            if (!_baseMove.IsGrounded)
                _baseMove.CurVelocity += Vector2.Down * _baseMove.Gravity;
        }

        if (_curDuration <= 0 && !_baseMove.IsDead)
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

        _currentStaggerImmune = _staggerImmune;
        _curCancelState = _cancelState;

        _curDuration = 0;

        _baseMove.IsDown = false;
    }
}
