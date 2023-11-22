using Godot;
using System;

public class SkillSetInfo
{
    string _actionButton;
    bool _flagSkillSet;
    SkillInfo _skill;

    public string ActionButton => _actionButton;
    public bool flagSkillSet { get => _flagSkillSet; set => _flagSkillSet = value; }
    public SkillInfo Skill => _skill;

    public SkillSetInfo(string actionButton, SkillInfo skill)
    {
        _actionButton = actionButton;
        _flagSkillSet = false;
        _skill = skill;
    }
}
