using Godot;
using System;
using System.Xml.Linq;

public partial class UIScreenSkillCooltime : TextureRect
{
    [Export] PlayerMove _playerMove;
    [Export] string skillCooltimeName;
    [Export] string curSkillCooltimeName;
    [Export] bool IsInvisibleOnReady = false;
    [Export] ColorRect _blindRect;
    [Export] Label _countLabel;
    [Export] ColorRect _redRect;
    [Export] ColorRect _yellowRect;
    [Export] ColorRect _greenRect;
    [Export] AnimationPlayer _animationPlayer;
    float _skillCooltime;
    float _curSkillCooltime;
    bool flagCooltimeOngoing;
    bool flagInitialized;


    public override void _Ready()
    {
        flagInitialized = false;
        flagCooltimeOngoing = false;
        _blindRect.Visible = false;
        _countLabel.Visible = false;
        _redRect.Visible = false;
        _yellowRect.Visible = false;
        _greenRect.Visible = false;

        if (IsInvisibleOnReady)
            Visible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!flagInitialized)
            Initialize();
        //var Prop = _playerMove.GetPropertyList();

        _curSkillCooltime = _playerMove.GetSkillCooltime(curSkillCooltimeName); ;
        if(_curSkillCooltime <= 0 || _curSkillCooltime == _skillCooltime)
        {
            if (flagCooltimeOngoing)
            {
                flagCooltimeOngoing = false;
                EndEffect();
            }
        }
        else
        {
            if (!flagCooltimeOngoing)
            {
                flagCooltimeOngoing = true;
                FirstEffect();
            }
        }

        OngoingEffect();
    }

    private void Initialize()
    {
        bool flagError = false;

        if (_playerMove == null)
            flagError = true;

        if (!_playerMove.IsSkillNameValid(skillCooltimeName))
            flagError = true;

        if (!_playerMove.IsSkillNameValid(curSkillCooltimeName))
            flagError = true;

        if (flagError)
        {
            GD.PrintErr(Name + ": Something is wrong");
            SetPhysicsProcess(false);
            Visible = false;
        }

        _skillCooltime = _playerMove.GetSkillCooltime(skillCooltimeName);
        GD.Print(_skillCooltime);    //디버그
        flagInitialized = true;
    }

    void FirstEffect()
    {
        if (IsInvisibleOnReady)
            Visible = true;
        _blindRect.Visible = true;
        _countLabel.Visible = true;
    }

    void OngoingEffect()
    {
        if(flagCooltimeOngoing)
        {
            _countLabel.Text = ((int)_curSkillCooltime).ToString();
            if (_curSkillCooltime <= 3)
                _redRect.Visible = true;
            if (_curSkillCooltime <= 2)
                _yellowRect.Visible = true;
            if (_curSkillCooltime <= 1)
                _greenRect.Visible = true;
        }
        else
        {
            _redRect.Visible = false;
            _yellowRect.Visible = false;
            _greenRect.Visible = false;
        }
    }

    void EndEffect()
    {
        if (IsInvisibleOnReady)
            Visible = false;
        _blindRect.Visible = false;
        _countLabel.Visible = false;
        _animationPlayer.Play("effect");
    }
}
