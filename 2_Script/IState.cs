using Godot;
using System;

public interface IState
{
    string ID { get; }

    IUnitMove BaseMove { get; set; }
    enum Cancelable { Always, SameLevel, HigherLevel, Never};
    Cancelable CancelState { get; }
    Cancelable CurCancelState { get; set; }
    int PriorityLevel { get; }
    /// <summary>
    /// 0: 없음 1: 약경직면역 2: 강경직면역 3: 다운면역
    /// </summary>
    int StaggerImmune { get; }
    int CurStaggerImmune { get; set; }


    public bool Condition();

    public void Move();

    public void Animation();

    /// <summary>
    /// FSM.CheckCondition()을 이용해 바뀔 수 있는지 여부를 체크한다.
    /// </summary>
    /// <returns>다음 State, 없으면 null</returns>
    public string Change();

    public void OnFinish();
}
