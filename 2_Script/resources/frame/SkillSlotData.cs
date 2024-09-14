using Godot;
using System;

/// <summary>
/// 임시 스킬슬롯
/// </summary>

[GlobalClass]
public partial class SkillSlotData : Resource
{
    #region 필드
    [Export] public string SkillAName { get; set; }
    [Export] public string SkillBName { get; set; }
    [Export] public string SkillCName { get; set; }
    [Export] public string SkillDName { get; set; }
    #endregion

    #region 생성자

    public SkillSlotData()
    {
        SkillAName = "";
        SkillBName = "";
        SkillCName = "";
        SkillDName = "";
    }

    #endregion
}
