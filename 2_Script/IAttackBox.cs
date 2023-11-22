using Godot;
using System;

public interface IAttackBox
{
    public string ID { get; }
    public int Damage { get; }
    public float AtkCooltime { get; }
    public float CurAtkCooltime { get; }
    public Vector2 PushPower { get; }
    public int Stagger { get; }
    public bool IsActivate { get; set; }


    public void Activate();

    public void Disable();

}
