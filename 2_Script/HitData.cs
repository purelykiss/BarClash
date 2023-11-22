using Godot;
using System;

public class HitData
{
    int _damage;
    /// <summary>
    /// 피격자의 위치나 투사체의 타격점
    /// </summary>
    DirectionH _pushDir;
    /// <summary>
    /// 공격자가 왼쪽에 있을 경우 피격자가 날라가는 방향과 세기
    /// </summary>
    Vector2 _pushPower;
    /// <summary>
    /// 0:없음 1:약경직 2:강경직 3:다운
    /// </summary>
    int _stagger;

    public int Damage { get => _damage; set => _damage = value; }

    public DirectionH PushDir { get => _pushDir; set => _pushDir = value; }
    public Vector2 PushPower { get => _pushPower; set => _pushPower = value; }
    public int Stagger { get => _stagger; set => _stagger = value; }

    public HitData(int damage, DirectionH pushDir, Vector2 pushPower, int stagger)
    {
        _damage = damage;
        _pushDir = pushDir;
        _pushPower = pushPower;
        _stagger = stagger;
    }
}
