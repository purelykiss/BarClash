using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class PlayerMove : CharacterBody2D, IUnitMove
{
    [Export] private FiniteStateMachine _fsm;
    [Export] private WallCollision _WallCol;
    [Export] private UnitCollision _UnitCol;
    [Export] private AttackList _AtkList;
    [Export] private EffectList _EfctList;
    [Export] private SoundList _SndList;
    [Export] private AnimationPlayer _AnimPlayer;
    [Export] private Sprite2D _Sprite;
    [Export] private CollisionShape2D _CollisionShape;

    [Export] string _affiliation = "player";
    [Export] string _enemyAffiliation = "enemy";
    [Export] int _hitPoint = 500;
    int _curHitPoint;
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
    [Export] float _coyoteJumpTime = 0.05f;
    float _curCoyoteJumpTime;
    [Export] int _airJumpCnt;
    int _curAirJumpCnt;
    bool _flagJumpedThisPress;
    [Export]float _jumpBufferTime = 0.05f;
    float _curJumpBufferTime;
    bool _isTryingJump;
    bool _isTryingQuickLanding;

    [Export] float _dodgeCooltime = 3f;
    float _curDodgeCooltime;
    [Export] float _downDodgeCooltime = 5f;
    float _curDownDodgeCooltime;
    bool _hasDodge;
    bool _hasDownDodge;
    bool _isDodging;

    [Export] float _attackBuffer = 0.1f;
    float _curAttackBuffer;
    bool flagAttackPress;
    bool _isTryingAttack;

    [Export] float _skillBuffer = 0.1f;

    float _curGuardBuffer;
    bool flagGuardPress;
    bool _isTryingGuard;
    [Export] float _guardCooltime = 3f;
    float _curGuardCooltime;
    bool _hasGuard;
    bool _isGuarding;
    bool _hasGuardBonus;
    [Export] float _guardBonusTime = 3f;
    float _curGuardBonusTime;

    float _curStraightBuffer;
    bool flagStraightPress;
    bool _isTryingStraight;
    [Export] float _straightCooltime = 5f;
    float _curStraightCooltime;
    bool _hasStraight;

    float _curStampBuffer;
    bool flagStampPress;
    bool _isTryingStamp;
    [Export] float _stampCooltime = 5f;
    float _curStampCooltime;
    bool _hasStamp;

    float _curRushBuffer;
    bool flagRushPress;
    bool _isTryingRush;
    [Export] float _rushCooltime = 20f;
    float _curRushCooltime;
    bool _hasRush;

    bool _isGrounded;
    bool _isDown;

    /// <summary>
    /// 밑이 y축 방향 기준 양의 방향이므로 더해야 내려간다.
    /// </summary>
    float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    /// <summary>
    /// 현재 바라보는 방향
    /// </summary>
    DirectionH _curDirection;

    /// <summary>
    /// 마지막에 손에서 땐 키 방향
    /// </summary>
    DirectionH _lastKeyDirection;
    [Export] float _runActiveTime = 0.1f;
    float _curRunActiveTime;
    bool _isRunning;

    [Export] DirectionH _spriteDirection;

    List<HitData> _HitList = new List<HitData>();
    Dictionary<string, float> SkillAndCooltime = new Dictionary<string, float>();


    public FiniteStateMachine FSM => _fsm;

    public WallCollision WallCol => _WallCol;

    public AttackList AtkList => _AtkList;

    public EffectList EfctList => _EfctList;

    public SoundList SndList => _SndList;

    public Dictionary<Direction4, bool> IsColliding => _WallCol.IsColliding;

    public UnitCollision UnitCol => _UnitCol;

    public string Affiliation => _affiliation;

    public string EnemyAffiliation => _enemyAffiliation;

    public float HitPoint => _hitPoint;

    public float CurHitPoint => _curHitPoint;

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

    public Vector2 CurVelocity 
    {
        get => _curVelocity;
        set => _curVelocity = value;
    }

    public Vector2 PreVelocity => _preVelocity;

    public bool HasGroundJump { get => _hasGroundJump; set => _hasGroundJump = value; }

    public float CoyoteJumpTime => _coyoteJumpTime;

    public float CurCoyoteJumpTime => _curCoyoteJumpTime;

    public int AirJumpCnt => _airJumpCnt;

    public int CurAirJumpCnt { get => _curAirJumpCnt; set => _curAirJumpCnt = value; }

    public bool FlagJumpedThisPress { get => _flagJumpedThisPress; set => _flagJumpedThisPress = value; }

    public float CurJumpBufferTime => _curJumpBufferTime;

    public bool IsTryingJump => _isTryingJump;

    public bool IsTryingQuickLanding => _isTryingQuickLanding;

    public bool HasDodge { get => _hasDodge; set => _hasDodge = value; }

    public bool HasDownDodge { get => _hasDownDodge; set => _hasDownDodge = value; }

    public bool IsDodging { get => _isDodging; set => _isDodging = value; }

    public bool IsTryingAttacking { get => _isTryingAttack; set => _isTryingAttack = value; }

    public bool IsTryingGuard { get => _isTryingGuard; set => _isTryingGuard = value; }

    public bool HasGuard { get => _hasGuard; set => _hasGuard = value; }

    public bool IsGuarding { get => _isGuarding; set => _isGuarding = value; }

    public bool HasGuardBonus { get => _hasGuardBonus; set => _hasGuardBonus = value; }

    public bool IsTryingStraight { get => _isTryingStraight; set => _isTryingStraight = value; }

    public bool HasStraight { get => _hasStraight; set => _hasStraight = value; }

    public bool IsTryingStamp { get => _isTryingStamp; set => _isTryingStamp = value; }

    public bool HasStamp { get => _hasStamp; set => _hasStamp = value; }

    public bool IsTryingRush { get => _isTryingRush; set => _isTryingRush = value; }

    public bool HasRush { get => _hasRush; set => _hasRush = value; }

    public bool IsGrounded => _isGrounded;

    public bool IsDown { get => _isDown; set => _isDown = value; }

    public DirectionH SpriteDirection => _spriteDirection;

    public DirectionH CurDirection { get => _curDirection; set => _curDirection = value; }

    public DirectionH LastKeyDirection => _lastKeyDirection;

    public float RunActiveTime => _runActiveTime;

    public float CurRunActiveTime { get => _curRunActiveTime; set => _curRunActiveTime = value; }

    public bool IsRunning { get => _isRunning; set => _isRunning = value; }

    public List<HitData> HitList { get => _HitList; set => _HitList = value; }

    public override void _Ready()
    {
        _curHitPoint = _hitPoint;
        _curWalkSpeed = _walkSpeed;
        _curRunSpeed = _runSpeed;
        _curJumpSpeed = _jumpSpeed;
        _curRepulsion = _repulsion;
        _curVelocity = Vector2.Zero;
        _preVelocity = Vector2.Zero;
        _isGrounded = true;
        _isDown = false;
        _curRunActiveTime = 0;
        _curCoyoteJumpTime = _coyoteJumpTime;
        _hasGroundJump = false;
        _curAirJumpCnt = _airJumpCnt;
        _flagJumpedThisPress = false;
        _curJumpBufferTime = 0;
        _isTryingJump = false;
        _isTryingQuickLanding = false;
        _isRunning = false;
        _curDodgeCooltime = _dodgeCooltime;
        _curDownDodgeCooltime = _downDodgeCooltime;
        _curGuardCooltime = 0;
        _curStraightCooltime = 0;
        _curStampCooltime = 0;
        _curRushCooltime = 0;
        _hasDodge = true;
        _hasDownDodge = true;
        _isDodging = false;
        flagAttackPress = false;
        _curAttackBuffer = 0;
        _isTryingAttack = false;
        _isGuarding = false;
        _hasGuardBonus = false;
        _curGuardBonusTime = _guardBonusTime;
        _curDirection = _spriteDirection;

        SkillAndCooltime.Add("_dodgeCooltime",_dodgeCooltime);
        SkillAndCooltime.Add("_curDodgeCooltime", 0);
        SkillAndCooltime.Add("_downDodgeCooltime", _downDodgeCooltime);
        SkillAndCooltime.Add("_curDownDodgeCooltime", 0);
        SkillAndCooltime.Add("_guardCooltime", _guardCooltime);
        SkillAndCooltime.Add("_curGuardCooltime", 0);
        SkillAndCooltime.Add("_straightCooltime", _straightCooltime);
        SkillAndCooltime.Add("_curStraightCooltime", 0);
        SkillAndCooltime.Add("_stampCooltime", _stampCooltime);
        SkillAndCooltime.Add("_curStampCooltime", 0);
        SkillAndCooltime.Add("_rushCooltime", _rushCooltime);
        SkillAndCooltime.Add("_curRushCooltime", 0);
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
        FSM.Activate();
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
        if (_isDodging)
            return;

        int onLeft = 0;
        int onRight = 0;

        foreach(var item in _UnitCol.OverlapingUnits)
        {
            if (item.Position.X < Position.X)
                onLeft++;
            else if (item.Position.X > Position.X)
                onRight++;
        }

        CurVelocity += Vector2.Right * (float)(onLeft - onRight) * _curRepulsion;

        switch(Mathf.Sign(_curVelocity.X))
        {
            case 1:     //오른쪽 진행중
                if(onRight > 0)
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

        //2차 검사, 몹 뚫기 방지
        if(onLeft + onRight == 0 && _curVelocity != Vector2.Zero)
        {
            DirectionH curD;
            if (Mathf.Sign(_curVelocity.X) > 0)
                curD = DirectionH.RIGHT;
            else
                curD = DirectionH.LEFT;

            float distance = _curVelocity.X * (float)GetPhysicsProcessDeltaTime();

            var space = GetViewport().World2D.DirectSpaceState;
            PhysicsShapeQueryParameters2D query = new PhysicsShapeQueryParameters2D();
            query.Transform = new Transform2D(0, GlobalPosition);
            query.Shape = _CollisionShape.Shape;
            query.Motion = _curVelocity * (float)GetPhysicsProcessDeltaTime();
            var result = space.IntersectShape(query);

            bool flag = false;
            if (result.Count > 1)
            {
                foreach (var item in result)
                {
                    var id = (ulong)item["collider_id"];
                    if (GetInstanceId() != id)
                    {
                        var tmp = GodotObject.InstanceFromId(id);
                        var col = tmp.Get("Collision");
                        if (tmp.Get("_affiliation").AsString() == _enemyAffiliation)
                        {
                            flag = true;
                            var pos = tmp.Get("global_position").AsVector2();
                            var tmpDis = pos.X - GlobalPosition.X;
                            switch(curD)
                            {
                                case DirectionH.LEFT:
                                    if (distance < tmpDis + 5)
                                        distance = tmpDis + 5;
                                    break;
                                case DirectionH.RIGHT:
                                    if (distance > tmpDis - 5)
                                        distance = tmpDis - 5;
                                    break;
                            }
                        }
                    }
                }
            }
            if (flag)
            {
                switch (curD)
                {
                    case DirectionH.LEFT:
                        if (distance > 0)
                            distance = 0;
                        break;
                    case DirectionH.RIGHT:
                        if (distance < 0)
                            distance = 0;
                        break;
                }

                _curVelocity = new Vector2(distance / (float)GetPhysicsProcessDeltaTime(), _curVelocity.Y);
            }
        }
    }

    public void CheckMove()
    {
        Velocity = CurVelocity;
        MoveAndSlide(); //작동을 하면 결과에 따라 자동으로 Velocity를 맞춰주는 것 같다.
    }

    public void CheckField()
    {
        _preVelocity = Velocity;
        _curVelocity = Vector2.Zero;

        CheckRunFactor();
        CheckJumpFactor();
        CheckQuickLandingFactor();
        CheckDodgeFactor();
        CheckAttackFactor();
        CheckGuardFactor();
        CheckStraightFactor();
        CheckStampFactor();
        CheckRushFactor();
        CheckStaggerFactor();
        CheckSkillAndCooltime();
    }

    private void CheckStaggerFactor()
    {
        _curStaggerImmune = _fsm.CurrentState.CurStaggerImmune;
        if(_isDown)
        {
            if (_HitList.Count > 0)
                _HitList[0].Stagger = 3;
        }
        if(_hasGuardBonus)
        {
            if(_curStaggerImmune < 2)
                _curStaggerImmune = 2;
        }
    }

    private void CheckRushFactor()
    {
        if (Input.IsActionPressed("player_skillD"))
        {
            if (!flagRushPress)
            {
                flagRushPress = true;
                _curRushBuffer = _skillBuffer;
                _isTryingRush = true;
            }
        }
        else
        {
            if (flagRushPress)
            {
                flagRushPress = false;
                _curRushBuffer = 0;
                _isTryingRush = false;
            }
        }

        _curRushBuffer -= (float)GetPhysicsProcessDeltaTime();
        if (_curRushBuffer <= 0)
        {
            _curRushBuffer = 0;
            _isTryingRush = false;
        }

        if (!_hasRush)
        {
            _curRushCooltime -= (float)GetPhysicsProcessDeltaTime();
            if (_curRushCooltime <= 0)
            {
                _curRushCooltime = _rushCooltime;
                _hasRush = true;
            }
        }
    }

    private void CheckStampFactor()
    {
        if (Input.IsActionPressed("player_skillC"))
        {
            if (!flagStampPress)
            {
                flagStampPress = true;
                _curStampBuffer = _skillBuffer;
                _isTryingStamp = true;
            }
        }
        else
        {
            if (flagStampPress)
            {
                flagStampPress = false;
                _curStampBuffer = 0;
                _isTryingStamp = false;
            }
        }

        _curStampBuffer -= (float)GetPhysicsProcessDeltaTime();
        if (_curStampBuffer <= 0)
        {
            _curStampBuffer = 0;
            _isTryingStamp = false;
        }

        if (!_hasStamp)
        {
            _curStampCooltime -= (float)GetPhysicsProcessDeltaTime();
            if (_curStampCooltime <= 0)
            {
                _curStampCooltime = _stampCooltime;
                _hasStamp = true;
            }
        }
    }

    private void CheckStraightFactor()
    {
        if (Input.IsActionPressed("player_skillB"))
        {
            if (!flagStraightPress)
            {
                flagStraightPress = true;
                _curStraightBuffer = _skillBuffer;
                _isTryingStraight = true;
            }
        }
        else
        {
            if (flagStraightPress)
            {
                flagStraightPress = false;
                _curStraightBuffer = 0;
                _isTryingStraight = false;
            }
        }

        _curStraightBuffer -= (float)GetPhysicsProcessDeltaTime();
        if (_curStraightBuffer <= 0)
        {
            _curStraightBuffer = 0;
            _isTryingStraight = false;
        }

        if (!_hasStraight)
        {
            _curStraightCooltime -= (float)GetPhysicsProcessDeltaTime();
            if (_curStraightCooltime <= 0)
            {
                _curStraightCooltime = _straightCooltime;
                _hasStraight = true;
            }
        }
    }

    private void CheckGuardFactor()
    {
        if (Input.IsActionPressed("player_skillA"))
        {
            if (!flagGuardPress)
            {
                flagGuardPress = true;
                _curGuardBuffer = _skillBuffer;
                _isTryingGuard = true;
            }
        }
        else
        {
            if (flagGuardPress)
            {
                flagGuardPress = false;
                _curGuardBuffer = 0;
                _isTryingGuard = false;
            }
        }

        _curGuardBuffer -= (float)GetPhysicsProcessDeltaTime();
        if (_curGuardBuffer <= 0)
        {
            _curGuardBuffer = 0;
            _isTryingGuard = false;
        }

        if (!_hasGuard)
        {
            _curGuardCooltime -= (float)GetPhysicsProcessDeltaTime();
            if (_curGuardCooltime <= 0)
            {
                _curGuardCooltime = _guardCooltime;
                _hasGuard = true;
            }
        }

        if(_hasGuardBonus)
        {
            if(_HitList.Count > 0)
            {
                
            }

            _curGuardBonusTime -= (float)GetPhysicsProcessDeltaTime();
            if( _curGuardBonusTime <= 0)
            {
                _curGuardBonusTime = _guardBonusTime;
                _hasGuardBonus = false;
            }
        }
    }

    private void CheckAttackFactor()
    {
        if(Input.IsActionPressed("player_attack"))
        {
            if(!flagAttackPress)
            {
                flagAttackPress = true;
                _curAttackBuffer = _attackBuffer;
                _isTryingAttack = true;
            }
        }
        else
        {
            if(flagAttackPress)
            {
                flagAttackPress = false;
                _curAttackBuffer = 0;
                _isTryingAttack = false;
            }
        }

        _curAttackBuffer -= (float)GetPhysicsProcessDeltaTime();
        if (_curAttackBuffer<=0)
        {
            _curAttackBuffer = 0;
            _isTryingAttack = false;
        }
    }

    private void CheckDodgeFactor()
    {
        if(!_hasDodge)
        {
            _curDodgeCooltime -= (float)GetPhysicsProcessDeltaTime();
            if(_curDodgeCooltime <= 0)
            {
                _curDodgeCooltime = _dodgeCooltime;
                _hasDodge = true;
            }
        }

        if (!_hasDownDodge)
        {
            _curDownDodgeCooltime -= (float)GetPhysicsProcessDeltaTime();
            if (_curDownDodgeCooltime <= 0)
            {
                _curDownDodgeCooltime = _downDodgeCooltime;
                _hasDownDodge = true;
            }
        }
    }

    void CheckRunFactor()
    {
        bool flagKeyUpThisFrame = false;

        if (Input.IsActionJustReleased("player_left"))
        {
            _lastKeyDirection = DirectionH.LEFT;
            _isRunning = false;
            flagKeyUpThisFrame = true;
        }
        if (Input.IsActionJustReleased("player_right"))
        {
            _lastKeyDirection = DirectionH.RIGHT;
            _isRunning = false;
            flagKeyUpThisFrame = true;
        }

        if (flagKeyUpThisFrame)
            _curRunActiveTime = _runActiveTime;
        else
            _curRunActiveTime -= (float)GetPhysicsProcessDeltaTime();

        if (_curRunActiveTime < 0)
            _curRunActiveTime = 0;

        if(_curRunActiveTime > 0)
        {
            if (Input.IsActionPressed("player_left"))
            {
                if (_lastKeyDirection == DirectionH.LEFT)
                {
                    _isRunning = true;
                    _curRunActiveTime = 0;
                }
            }
            if (Input.IsActionPressed("player_right"))
            {
                if (_lastKeyDirection == DirectionH.RIGHT)
                {
                    _isRunning = true;
                    _curRunActiveTime = 0;
                }
            }
        }

        if(_isRunning)
        {
            if (Input.IsActionPressed("player_left") && Input.IsActionPressed("player_right"))
                _isRunning = false;
        }
    }

    void CheckJumpFactor()
    {
        if(IsGrounded)
        {
            _curCoyoteJumpTime = _coyoteJumpTime;
            if (_preVelocity.Y > -0.01f)
                _hasGroundJump = true;
            _curAirJumpCnt = _airJumpCnt;
        }
        else
        {
            _curCoyoteJumpTime -= (float)GetPhysicsProcessDeltaTime();
            if (_curCoyoteJumpTime <= 0)
            {
                _curCoyoteJumpTime = 0;
                _hasGroundJump = false;
            }
        }

        if(_flagJumpedThisPress)
        {
            _curJumpBufferTime = 0;
        }

        if (!Input.IsActionPressed("player_jump"))
        {
            _flagJumpedThisPress = false;
        }

        if(Input.IsActionJustPressed("player_jump"))
        {
            _curJumpBufferTime = _jumpBufferTime;
        }
        else
        {
            _curJumpBufferTime -= (float)GetPhysicsProcessDeltaTime();
            if (_curJumpBufferTime < 0)
                _curJumpBufferTime = 0;
        }

        _isTryingJump = false;
        if(_curJumpBufferTime > 0)
        {
            if (!_flagJumpedThisPress)
            {
                if (_isGrounded)
                {
                    if (_hasGroundJump)
                    {
                        _isTryingJump = true;
                        GD.Print("Trying Ground Jump");
                    }
                }
                else
                {
                    if (_curCoyoteJumpTime > 0 && _hasGroundJump)
                    {
                        _isTryingJump = true;
                        GD.Print("Trying Coyote Jump");
                    }
                    else if (_curAirJumpCnt > 0)
                    {
                        _isTryingJump = true;
                        GD.Print("Trying Air Jump");
                    }
                }
            }

        }
    }

    void CheckQuickLandingFactor()
    {
        if(Input.IsActionPressed("player_down"))
            _isTryingQuickLanding = true;
        else
            _isTryingQuickLanding = false;
    }

    void CheckSkillAndCooltime()
    {
        SkillAndCooltime["_dodgeCooltime"] = _dodgeCooltime;
        SkillAndCooltime["_curDodgeCooltime"] = _curDodgeCooltime;
        SkillAndCooltime["_downDodgeCooltime"] = _downDodgeCooltime;
        SkillAndCooltime["_curDownDodgeCooltime"] = _curDownDodgeCooltime;
        SkillAndCooltime["_guardCooltime"] = _guardCooltime;
        SkillAndCooltime["_curGuardCooltime"] = _curGuardCooltime;
        SkillAndCooltime["_straightCooltime"] = _straightCooltime;
        SkillAndCooltime["_curStraightCooltime"] = _curStraightCooltime;
        SkillAndCooltime["_stampCooltime"] = _stampCooltime;
        SkillAndCooltime["_curStampCooltime"] = _curStampCooltime;
        SkillAndCooltime["_rushCooltime"] = _rushCooltime;
        SkillAndCooltime["_curRushCooltime"] = _curRushCooltime;
    }

    public void SetAnimation(string id, bool flagRepeat)
    {
        if(flagRepeat)
            _AnimPlayer.CurrentAnimation = id;
        else
        {
            if(_AnimPlayer.CurrentAnimation != id)
                _AnimPlayer.CurrentAnimation = id;
        }
    }

    public void OnHit(int damage, Vector2 origin, Vector2 pushPower, int stagger)
    {
        DirectionH pushDir;
        if(GlobalPosition.X > origin.X)
            pushDir = DirectionH.RIGHT;
        else
            pushDir = DirectionH.LEFT;

        _HitList.Add(new HitData(damage, pushDir, pushPower, stagger));

        if (_isGuarding)
        {
            damage = damage / 10;
            _hasGuardBonus = true;
            _curGuardBonusTime = _guardBonusTime;
        }
        else if (_hasGuardBonus)
            damage = damage / 2;

        _curHitPoint -= damage;

        if(_curHitPoint <= 0)
        {
            _curHitPoint = 0;
            _isDead = true;
        }

        _EfctList.ActivateEffect("onHit");
        _SndList.ActivateSound("onHit");
    }

    public bool IsSkillNameValid(string name)
    {
        return SkillAndCooltime.ContainsKey(name);
    }

    public float GetSkillCooltime(string name)
    {
        if(SkillAndCooltime.ContainsKey(name))
        {
            return SkillAndCooltime[name];
        }
        else
            return 0;
    }
}
