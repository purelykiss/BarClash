using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

public partial class WarriorMove : CharacterBody2D, IUnitMove
{
    [Export] private FiniteStateMachine _fsm;
    [Export] private WallCollision _WallCol;
    [Export] private UnitCollision _UnitCol;
    [Export] private AttackList _AtkList;
    [Export] private EffectList _EfctList;
    [Export] private AnimationPlayer _AnimPlayer;
    [Export] private Sprite2D _Sprite;
    [Export] private EnemyDetectArea _DetectArea;
    Random _Random = new Random();

    [Export] string _affiliation = "enemy";
    [Export] string _enemyAffiliation = "player";
    [Export] int _hitPoint = 2000;
    int _curHitPoint;
    [Export] int _superArmer = 40;
    int _curSuperArmer;
    [Export] float _superArmerCooltime = 10;
    float _curSuperArmerCooltime;
    bool _isSuperArmerBreak;
    bool flagRestoreSuperArmer;
    bool _isDead;
    int _curStaggerImmune;

    [Export] float _walkSpeed = 100f;
    float _curWalkSpeed;
    [Export] float _runSpeed = 200f;
    float _curRunSpeed;
    [Export] float _jumpSpeed = 200f;
    float _curJumpSpeed;

    [Export] float _repulsion = 10f;
    float _curRepulsion;
    [Export] float _maxCrossingUnitSpeed = 20f;

    Vector2 _curVelocity;
    Vector2 _preVelocity;

    bool _hasGroundJump;
    [Export] int _airJumpCnt;
    int _curAirJumpCnt;

    bool _isTryingDodge;
    bool _isDodging;
    bool _isDownDodging;
    bool flagDodgingFirstCycle;
    bool _hasDodge;
    bool _hasDownDodge;
    [Export] float _dodgeCooltime = 5;
    float _curDodgeCooltime;
    [Export] float _downDodgeCooltime = 10;
    float _curDownDodgeCooltime;
    [Export] int _dodgeActiveDamage = 200;
    int _curDodgeActiveDamage;
    List<(int, float)> _dmgCoolHistory = new List<(int, float)>();
    [Export] float dodgeDamageCooltime = 5f;

    bool flagEnemyDetected;
    IUnitMove _targetUnit;

    IUnitMove.AIState _curAiState;
    [Export] float _aiWaitTime = 2;
    float _curAiWaitTime;
    [Export] float _aiChaseTime = 2;
    float _curAiChaseTime;
    bool _isTryingChase;
    bool _isTryingAttack;
    bool _isTryingSweepCombo;
    string _nextPatternID;
    bool _isAttacking;
    bool flagChaseFirstCycle;
    bool flagUnconditionalAttack;
    List<IEnemyPattern> _patternList = new List<IEnemyPattern>();
    List<IEnemyPattern> _curPatternList = new List<IEnemyPattern>();

    bool _isUsingSlash1;
    bool _isUsingBeam1;

    [Export] int _phaseCnt;
    int _curPhase;
    int flagPhase;

    bool _isGrounded;
    bool _isDown;
    bool _isRunning;

    /// <summary>
    /// 밑이 y축 방향 기준 양의 방향이므로 더해야 내려간다.
    /// </summary>
    float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    /// <summary>
    /// 현재 바라보는 방향
    /// </summary>
    DirectionH _curDirection;

    [Export] DirectionH _spriteDirection = DirectionH.RIGHT;

    List<HitData> _HitList = new List<HitData>();


    public FiniteStateMachine FSM => _fsm;

    public WallCollision WallCol => _WallCol;

    public UnitCollision UnitCol => _UnitCol;

    public AttackList AtkList => _AtkList;

    public EffectList EfctList => _EfctList;

    public Dictionary<Direction4, bool> IsColliding => _WallCol.IsColliding;

    public string Affiliation => _affiliation;

    public string EnemyAffiliation => _enemyAffiliation;

    public float HitPoint => _hitPoint;

    public float CurHitPoint => _curHitPoint;

    public float SuperArmer => _superArmer;

    public float CurSuperArmer => _curSuperArmer;

    public bool HasSuperArmer => !_isSuperArmerBreak;

    public bool IsDead => _isDead;

    public int CurStaggerImmune => _curStaggerImmune;

    public float WalkSpeed => _walkSpeed;

    public float CurWalkSpeed { get => _curWalkSpeed; set => _curWalkSpeed = value; }

    public float RunSpeed => _runSpeed;

    public float CurRunSpeed { get => _curRunSpeed; set => _curRunSpeed = value; }

    public float JumpSpeed => _jumpSpeed;

    public float CurJumpSpeed { get => _curJumpSpeed; set => _curJumpSpeed = value; }

    public float Gravity => _gravity;

    public float Repulsion => _repulsion;

    public float CurRepulsion { get => _curRepulsion; set => _curRepulsion = value; }

    public float MaxCrossingUnitSpeed => _maxCrossingUnitSpeed;

    public Vector2 CurVelocity { get => _curVelocity; set => _curVelocity = value; }

    public Vector2 PreVelocity => _preVelocity;

    public bool HasGroundJump { get => _hasGroundJump; set => _hasGroundJump = value; }

    public int AirJumpCnt => _airJumpCnt;

    public int CurAirJumpCnt { get => _curAirJumpCnt; set => _curAirJumpCnt = value; }

    public bool IsTryingDodge { get => _isTryingDodge; set => _isTryingDodge = value; }

    public bool IsDodging { get => _isDodging; set => _isDodging = value; }

    public bool IsDownDodging { get => _isDownDodging; set => _isDownDodging = value; }

    public bool HasDodge { get => _hasDodge; set => _hasDodge = value; }

    public bool HasDownDodge { get => _hasDownDodge; set => _hasDownDodge = value; }

    public IUnitMove TargetUnit => _targetUnit;

    public IUnitMove.AIState CurAiState => _curAiState;

    public string NextPatternID => _nextPatternID;

    public bool IsTryingChase => _isTryingChase;

    public bool IsTryingAttack => _isTryingAttack;

    public bool IsTryingSweepCombo { get => _isTryingSweepCombo; set => _isTryingSweepCombo = value; }
    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }

    public bool IsUsingSlash2 { get => _isUsingSlash1; set => _isUsingSlash1 = value; }

    public bool IsUsingBeam1 { get => _isUsingBeam1; set => _isUsingBeam1 = value; }

    public int CurPhase => _curPhase;

    public List<HitData> HitList { get => _HitList; set => _HitList = value; }

    public bool IsRunning => _isRunning;

    public bool IsGrounded => _isGrounded;

    public bool IsDown { get => _isDown; set => _isDown = value; }

    public DirectionH SpriteDirection => _spriteDirection;

    public DirectionH CurDirection { get => _curDirection; set => _curDirection = value; }

    public override void _Ready()
    {
        _curHitPoint = _hitPoint;
        _curSuperArmer = _superArmer;
        _curSuperArmerCooltime = _superArmerCooltime;
        _isSuperArmerBreak = false;
        flagRestoreSuperArmer = false;
        _isDead = false;
        _curStaggerImmune = 0;
        _curWalkSpeed = _walkSpeed;
        _curRunSpeed = _runSpeed;
        _curJumpSpeed = _jumpSpeed;
        _curRepulsion = _repulsion;
        _curVelocity = Vector2.Zero;
        _preVelocity = Vector2.Zero;
        _hasGroundJump = true;
        _curAirJumpCnt = _airJumpCnt;
        _isTryingDodge = false;
        _isDodging = false;
        _isDownDodging = false;
        flagDodgingFirstCycle = true;
        _hasDodge = true;
        _hasDownDodge = true;
        _curDodgeCooltime = _dodgeCooltime;
        _curDownDodgeCooltime = _downDodgeCooltime;
        _curDodgeActiveDamage = 0;
        _dmgCoolHistory = new List<(int, float)>();
        flagEnemyDetected = false;
        _targetUnit = null;
        _curAiState = IUnitMove.AIState.WAIT;
        _curAiWaitTime = _aiWaitTime;
        _curAiChaseTime = _aiChaseTime;
        flagChaseFirstCycle = true;
        _isTryingChase = false;
        _isTryingAttack = false;
        _isTryingSweepCombo = false;
        _nextPatternID = null;
        _isAttacking = false;
        flagUnconditionalAttack = false;
        _curPhase = 1;
        flagPhase = 1;
        _isGrounded = true;
        _isDown = false;
        _isRunning = false;

        foreach(var item in _fsm.States)
        {
            if(item is IEnemyPattern)
            {
                _patternList.Add((IEnemyPattern)item);
            }
        }
        foreach(var item in _patternList)
        {
            if(item.AvailablePhase <= _curPhase)
            {
                _curPatternList.Add(item);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        //외부 정보 수집
        //현재 상태
        //	작동(속도 등의 필드 변환)
        //	애니메이션
        //	변환
        //상태머신의 필드 변환과 외부 상황 취합
        //정리

        CheckCollision();
        ActivateFSM();
        CheckDirection();
        CheckRepulse();
        CheckMove();
        CheckField();
    }

    public void CheckCollision()
    {
        _isGrounded = IsColliding[Direction4.DOWN];
    }

    public void ActivateFSM()
    {
        _fsm.Activate();
    }

    public void CheckDirection()
    {
        if (_curDirection != _spriteDirection)
            _Sprite.Scale = new Vector2(-1, 1);
        else
            _Sprite.Scale = new Vector2(1, 1);
    }

    public void CheckRepulse()
    {
        if (_isDodging || _isDownDodging || IsUsingSlash2 || _isUsingBeam1)
            return;
        int onLeft = 0;
        int onRight = 0;

        foreach (var item in _UnitCol.OverlapingUnits)
        {
            if (item.Position.X < Position.X)
                onLeft++;
            else if (item.Position.X > Position.X)
                onRight++;
        }

        CurVelocity += Vector2.Right * (float)(onLeft - onRight) * _curRepulsion;

        switch (Mathf.Sign(_curVelocity.X))
        {
            case 1:     //오른쪽 진행중
                if (onRight > 0)
                {
                    float tmp = Mathf.Clamp(_curVelocity.X, 0, _maxCrossingUnitSpeed);
                    _curVelocity = new Vector2(tmp, _curVelocity.Y);
                }

                break;
            case -1:    //왼쪽 진행중
                if (onLeft > 0)
                {
                    float tmp = Mathf.Clamp(_curVelocity.X, -_maxCrossingUnitSpeed, 0);
                    _curVelocity = new Vector2(tmp, _curVelocity.Y);
                }
                break;
        }
    }

    public void CheckMove()
    {
        Velocity = _curVelocity;
        MoveAndSlide();
    }

    public void CheckField()
    {
        _preVelocity = Velocity;
        _curVelocity = Vector2.Zero;

        CheckEnemy();
        CheckAiState();
        CheckPhase();
        CheckSuperArmer();
        CheckStaggerFactor();
        CheckDodgeFactor();
    }

    private void CheckEnemy()
    {
        if (_DetectArea.Units.Count > 0)
        {
            bool flagSameTarget = false;
            foreach (var item in _DetectArea.Units)
            {
                if (item == _targetUnit)
                {
                    flagSameTarget = true;
                    break;
                }
            }
            if (!flagSameTarget)
            {
                _targetUnit = null;
                foreach (var item in _DetectArea.Units)
                {
                    Node tmp = (Node)item;
                    if (tmp.IsInGroup("player"))
                    {
                        _targetUnit = item;
                        break;
                    }
                }
            }
        }
        else
            _targetUnit = null;
    }

    private void CheckAiState()
    {
        if(_targetUnit == null)
        {
            ResetAiState();
            return;
        }

        if(_targetUnit.IsDead)
        {
            ResetAiState();
            return;
        }

        if(_isDead)
        {
            ResetAiState();
            return;
        }

        switch(_curAiState)
        {
            case IUnitMove.AIState.WAIT:
                _curAiWaitTime -= (float)GetPhysicsProcessDeltaTime();
                if( _curAiWaitTime <= 0 )
                {
                    _curAiWaitTime = _aiWaitTime;
                    _curAiState = IUnitMove.AIState.CHASE;
                }
                break;
            case IUnitMove.AIState.CHASE:
                _curAiChaseTime -= (float)GetPhysicsProcessDeltaTime();
                if(flagChaseFirstCycle)
                {
                    flagChaseFirstCycle = false;
                    int tmp = _Random.Next(0, _curPatternList.Count);// _patternList.Count
                    GD.Print(tmp);
                    _nextPatternID = _curPatternList[tmp].ID; //_nextPatternID = _patternList[tmp].ID;
                    _isTryingChase = true;
                    _isTryingAttack = true;
                }

                if (_curAiChaseTime <= 0 )
                {
                    _curAiChaseTime = 0;
                    if(!flagUnconditionalAttack)
                    {
                        flagUnconditionalAttack = true;
                        foreach (var item in _curPatternList)
                        {
                            if (item.IsUnconditional)
                            {
                                _nextPatternID = item.ID;
                                break;
                            }
                        }
                    }
                }
                else if (_curAiChaseTime < 1)
                    _isRunning = true;

                if (_isAttacking)
                {
                    flagChaseFirstCycle = true;
                    _curAiChaseTime = _aiChaseTime;
                    _isTryingAttack = false;
                    _isTryingChase = false;
                    _isRunning = false;
                    flagUnconditionalAttack = false;
                    _curAiState = IUnitMove.AIState.ATTACK;
                    //뒷정리
                }
                break;
            case IUnitMove.AIState.ATTACK:
                if(_isAttacking)
                    _isAttacking = false;
                if (_fsm.CurrentState.ID == "idle"
                    || _fsm.CurrentState.ID == "walk"
                    || _fsm.CurrentState.ID == "run"
                    || _fsm.CurrentState.ID == "stagger"
                    || _fsm.CurrentState.ID == "down")
                {
                    _isTryingSweepCombo = false;
                    _curAiState = IUnitMove.AIState.WAIT;
                }
                break;
        }
    }

    private void ResetAiState()
    {
        _curAiState = IUnitMove.AIState.WAIT;
        _curAiWaitTime = _aiWaitTime;
        _isAttacking = false;
        flagChaseFirstCycle = true;
        _curAiChaseTime = _aiChaseTime;
        _isTryingAttack = false;
        _isTryingChase = false;
        _isRunning = false;
    }

    private void CheckPhase()
    {
        if ((float)_curHitPoint / (float)_hitPoint < 0.5f)
            _curPhase = 2;
        else
            _curPhase = 1;

        if (_curPhase != flagPhase)
        {
            switch (_curPhase)
            {
                case 1:
                    flagPhase = 1;
                    break;
                case 2:
                    flagPhase = 2;
                    break;
            }

            _curPatternList.Clear();
            foreach (var item in _patternList)
            {
                if (item.AvailablePhase <= _curPhase)
                {
                    _curPatternList.Add(item);
                }
            }
        }
    }

    void CheckSuperArmer()
    {
        if(_isDead)
        {
            _curSuperArmer = 0;
            _isSuperArmerBreak = true;
            return;
        }

        if(flagRestoreSuperArmer)
        {
            _curSuperArmerCooltime -= (float)GetPhysicsProcessDeltaTime();
            if(_curSuperArmerCooltime <= 0)
            {
                _curSuperArmerCooltime = 0;
                _isSuperArmerBreak = false;
                flagRestoreSuperArmer = false;
                _curSuperArmer = _superArmer;
            }
        }
    }

    private void CheckStaggerFactor()
    {
        _curStaggerImmune = _fsm.CurrentState.CurStaggerImmune;

        if (_isDown)
        {
            if (_HitList.Count > 0)
                _HitList[0].Stagger = 3;
        }
        if (!_isSuperArmerBreak)
            _curStaggerImmune = 3;
    }

    private void CheckDodgeFactor()
    {
        //회피 발동시
        if(_isDodging && flagDodgingFirstCycle)
        {
            flagDodgingFirstCycle = false;
            _isTryingDodge = false;
            _hasDodge = false;
            ClearDodgeActiveDamage();
        }

        //다운회피 발동시
        if(_isDownDodging && flagDodgingFirstCycle)
        {
            flagDodgingFirstCycle = false;
            _isTryingDodge = false;
            _hasDownDodge = false;
            ClearDodgeActiveDamage();
        }

        //회피 및 다운회피 flag
        if(!_isDodging && !_isDownDodging)
        {
            flagDodgingFirstCycle = true;
        }

        //회피 쿨타임 계산
        if(!_hasDodge)
        {
            _curDodgeCooltime -= (float)GetPhysicsProcessDeltaTime();
            if(_curDodgeCooltime <= 0)
            {
                _curDodgeCooltime = _dodgeCooltime;
                _hasDodge = true;
            }
        }

        //다운회피 쿨타임 계산
        if(!_hasDownDodge)
        {
            _curDownDodgeCooltime -= (float)GetPhysicsProcessDeltaTime();
            if (_curDownDodgeCooltime <= 0)
            {
                _curDownDodgeCooltime = _downDodgeCooltime;
                _hasDownDodge = true;
            }
        }

        //회피 발동 데미지 리스트 정리
        if(_dmgCoolHistory.Count > 0)
        {
            for(int i = _dmgCoolHistory.Count -1; i >= 0; i--)
            {
                _dmgCoolHistory[i] = (_dmgCoolHistory[i].Item1, _dmgCoolHistory[i].Item2 - (float)GetPhysicsProcessDeltaTime());
                if (_dmgCoolHistory[i].Item2 <= 0)
                    SubtractDodgeActiveDamage(i);
            }
        }

        //회피 시도
        _isTryingDodge = (_curDodgeActiveDamage >= _dodgeActiveDamage);
    }

    void AddDodgeActiveDamage(int damage)
    {
        _dmgCoolHistory.Add((damage, dodgeDamageCooltime));
        _curDodgeActiveDamage += damage;
    }

    void SubtractDodgeActiveDamage(int index)
    {
        _curDodgeActiveDamage -= _dmgCoolHistory[index].Item1;
        _dmgCoolHistory.RemoveAt(index);
    }

    void ClearDodgeActiveDamage()
    {
        _curDodgeActiveDamage = 0;
        _dmgCoolHistory.Clear();
    }

    public void SetAnimation(string id, bool flagRepeat)
    {
        if (flagRepeat)
            _AnimPlayer.CurrentAnimation = id;
        else
        {
            if (_AnimPlayer.CurrentAnimation != id)
                _AnimPlayer.CurrentAnimation = id;
        }
    }

    public void OnHit(int damage, Vector2 origin, Vector2 pushPower, int stagger)
    {
        GD.Print("Warrior Hit!");

        DirectionH pushDir;
        if (GlobalPosition.X > origin.X)
            pushDir = DirectionH.RIGHT;
        else
            pushDir = DirectionH.LEFT;

        _HitList.Add(new HitData(damage, pushDir, pushPower, stagger));

        if (!_isSuperArmerBreak)
        {
            damage = (int)(damage * 1.2f);
        }

        _curHitPoint -= damage;

        if (_curHitPoint <= 0)
        {
            _curHitPoint = 0;
            _isDead = true;
        }

        if(!_isDead)
            AddDodgeActiveDamage(damage);

        _curSuperArmer--;
        if( _curSuperArmer <= 0)
        {
            _curSuperArmer = 0;
            _isSuperArmerBreak = true;
            if (!flagRestoreSuperArmer)
            {
                flagRestoreSuperArmer = true;
                _curSuperArmerCooltime = _superArmerCooltime;
            }
        }

        _EfctList.ActivateEffect("onHit");
    }
}
