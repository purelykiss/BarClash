using Godot;
using System;

public partial class WarriorDown : Node, IState, IStagger
{
    [Export] WarriorMove _warriorMove;
    [Export] string _id = "down";
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
    bool flagIsGround;

    [Export] float _duration = 2f;
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
        _baseMove = _warriorMove;
        _fsm = GetParent<FiniteStateMachine>();

        flagAnimate = false;
        flagFirstCycle = true;
        flagDownFront = false;
        flagIsGround = false;

        _currentStaggerImmune = _staggerImmune;
        _curCancelState = _cancelState;

        _curDuration = 0;
    }

    public bool Condition()
    {
        if (!_fsm.CheckCancelable(this))
            return false;

        if (!_warriorMove.IsDead && _warriorMove.HasSuperArmer)
            return false;

        if (_warriorMove.HitList.Count == 0)
            return false;
        //다운조건:
        //-사망시 상시
        //-생존시 다운x:
        //1. 피격면역 < 피격이상
        //2. 피격이상 == 3
        //-생존시 다운o:
        //모든 피격이상 3 고정
        if (_warriorMove.IsDown && !flagFirstCycle)
        {
            _curHitData = _warriorMove.HitList[0];
            return true;
        }

        if (_warriorMove.CurStaggerImmune < _warriorMove.HitList[0].Stagger)
        {
            if (_warriorMove.HitList[0].Stagger == 3)
            {
                _curHitData = _warriorMove.HitList[0];
                if (_warriorMove.IsDead)
                {

                }
                return true;
            }
        }

        if (_warriorMove.IsDead)
        {
            if (flagFirstCycle)
            {
                _curHitData = _warriorMove.HitList[0];
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
            if (flagIsGround)
            {
                _baseMove.SetAnimation("down", true);
            }
            else
            {
                if (flagDownFront)
                    _baseMove.SetAnimation("downInAirFront", true);
                else
                    _baseMove.SetAnimation("downInAirBack", true);
            }
        }
    }

    public void Move()
    {
        if (flagFirstCycle)
        {
            flagFirstCycle = false;
            _curDuration = _duration;
            _baseMove.IsDown = true;

            flagIsGround = _baseMove.IsGrounded;

            if (_curHitData.PushDir == _warriorMove.CurDirection)
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

            if (flagIsGround != _baseMove.IsGrounded)
            {
                flagIsGround = _baseMove.IsGrounded;
                flagAnimate = false;
            }
            float speed = Mathf.Lerp(_baseMove.PreVelocity.X, 0, 0.1f);
            if (Mathf.Abs(speed) < 0.01f)
                speed = 0;
            _baseMove.CurVelocity += new Vector2(speed, _baseMove.PreVelocity.Y);

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
