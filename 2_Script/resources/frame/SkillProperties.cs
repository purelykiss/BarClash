using Godot;
using System;

public partial class SkillProperties : Resource
{
    [Export] public string SkillName { get; set; }
    [Export] public float SkillBuffer { get; set; }
    [Export] public float CurSkillBuffer { get; set; }
    [Export] public bool FlagSkillPress { get; set; }
    [Export] public bool IsTryingSkill { get; set; }
    [Export] public float Cooltime { get; set; }
    [Export] public float CurCooltime { get; set; }
    [Export] public bool HasSkill { get; set; }


    public SkillProperties()
    {
        SkillName = "";
        SkillBuffer = 0f;
        CurSkillBuffer = 0f;
        FlagSkillPress = false;
        IsTryingSkill = false;
        Cooltime = 0f;
        CurCooltime = 0f;
        HasSkill = false;
    }
}
