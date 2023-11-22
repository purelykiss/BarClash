using Godot;
using System;

/// <summary>
/// 적이 고를 수 있는 공격 시작 패턴
/// </summary>
public interface IEnemyPattern
{
    string ID { get; }
    int AvailablePhase { get; }
    /// <summary>
    /// 거리 관계없이 사용 가능 여부
    /// </summary>
    bool IsUnconditional { get; }
}
