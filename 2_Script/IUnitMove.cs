using Godot;
using System;
using System.Collections.Generic;

public interface IUnitMove
{
    public FiniteStateMachine FSM { get; }

    public WallCollision WallCol { get; }
    public UnitCollision UnitCol { get; }
    public AttackList AtkList { get; }
    public EffectList EfctList { get; }

    public string Affiliation { get; }
    public string EnemyAffiliation { get; }
    public float HitPoint { get; }
    public float CurHitPoint { get; }
    public bool IsDead { get; }
    /// <summary>
    /// 0: 면역없음 1: 약피격 2: 강피격 3: 다운
    /// </summary>
    public int CurStaggerImmune { get; }

    public Vector2 GlobalPosition { get; }
    public float WalkSpeed { get; }
    public float CurWalkSpeed { get; set; }
    public float RunSpeed { get; }
    public float CurRunSpeed { get; set; }
    public float JumpSpeed { get; }
    public float CurJumpSpeed { get; set; }
    public float Gravity { get; }
    public float Repulsion { get; }
    public float CurRepulsion { get; set; }
    public float MaxCrossingUnitSpeed { get; }
    public Vector2 CurVelocity { get; set; }
    public Vector2 PreVelocity { get; }
    public bool HasGroundJump { get; set; }
    public int AirJumpCnt { get; }
    public int CurAirJumpCnt { get; set; }

    public List<HitData> HitList { get; set; }

    public bool IsRunning { get; }
    public bool IsGrounded { get; }
    public bool IsDown { get; set; }
    public DirectionH SpriteDirection { get; }
    public DirectionH CurDirection { get; set; }

    enum AIState { WAIT, CHASE, ATTACK }

    /// <summary>
    /// 애니메이션을 바꿈
    /// </summary>
    /// <param name="id">애니메이터에 적은 애니메이션 이름</param>
    /// <param name="flagRepeat">만약 동일 애니메이션일 경우 재동작 여부</param>
    public void SetAnimation(string id, bool flagRepeat);

    void CheckCollision();
    void ActivateFSM();
    void CheckDirection();
    void CheckRepulse();
    void CheckMove();
    void CheckField();

    public void OnHit(int damage, Vector2 origin, Vector2 pushPower, int stagger);
}
