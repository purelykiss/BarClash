using Godot;
using System;

public partial class SkillVO
{
    //스킬 이름
    //스킬 섬네일 이름
    //동작 주체?
    //fsm ID
    //스킬 자원: 쿨타임
    //스킬 자원: 현제 쿨타임
    private String _skillName;
    private String _thumbnail;
    private String _fsmID;
    private float cooltime;
    private float curCooltime;
}
