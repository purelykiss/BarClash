using Godot;
using System;

public interface IProjectile
{
    public string ID { get; }
    public int Damage { get; }
    public float AtkCooltime { get; }
    public float CurAtkCooltime { get; }
    public Vector2 PushPower { get; }
    public int Stagger { get; }
    public float ProjectileSpeed { get; }
    public bool IsActivate { get; set; }
    public Vector2 Destination { get; }
    public Vector2 CurVelocity { get; set; }
    public Vector2 PreVelocity { get; }

    /// <summary>
    /// Activate()보다 먼저 발동해서 목적지를 먼저 설정해야됨.
    /// </summary>
    /// <param name="point">목적지</param>
    public void SetDestination(Vector2 point);

    public void Activate();

    public void Disable();

}
