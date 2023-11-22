using Godot;
using System;

public class SkillInfo
{
    float _buffer;
    bool _isTryingSkill;
    string _skillName;

    public float Buffer { get => _buffer; set => _buffer = value; }
    public bool IsTryingSkill { get => _isTryingSkill; set => _isTryingSkill = value; }
    public string SkillName => _skillName;

    public SkillInfo(string skillName)
    {
        _skillName = skillName;
        _buffer = 0;
        _isTryingSkill = false;
    }
}
