using Godot;
using System;

[GlobalClass]   //다른 resources파일을 만들고 싶다면 꼭 넣을 것

/// <summary>
/// 스킬 정보를 담는 Resources 파일
/// </summary>
/// <param name="UnitID">스킬을 소유하는 Unit ID</param>
/// <param name="SkillName">스킬 이름</param>
/// <param name="StateID">IState 스크립트 ID</param>
/// <param name="Cooltime">스킬 쿨타임</param>
public partial class SkillData : Resource
{
    #region 필드
    [Export] public string SkillName { get; set; }
    [Export] public string StateID { get; set; }
    [Export] public Texture2D Icon { get; set; }
    [Export] public float Cooltime { get; set; }
    #endregion

    #region 생성자

    public SkillData()
    {
        SkillName = "";
        StateID = "";
        Icon = null;
        Cooltime = 0f;
    }

    #endregion
}
