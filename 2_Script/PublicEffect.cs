using Godot;
using System;
using System.Collections.Generic;

public partial class PublicEffect : Node2D, IEffect
{
    IUnitMove _baseMove => _EffectList.BaseMove;
    EffectList _EffectList;
    [Export] string _id;
    [Export] bool _isActivate;
    bool flagActivate;
    [Export] AnimationPlayer _AnimPlayer;
    [Export] DirectionH _spriteDirection = DirectionH.RIGHT;
    [Export] bool _hasFinishMotion = false;
    bool flagEffect;
    [Export] bool _isFinishMotionOver;
    bool flagFinishMotion;

    public string ID => _id;

    public bool IsActivate { get => _isActivate; set => _isActivate = value; }

    public override void _Ready()
    {
        _EffectList = GetParent<EffectList>();

        if (_AnimPlayer != null)
        {
            if (_EffectList.AttackDirection == _spriteDirection)
                Scale = new Vector2(1, 1);
            else
                Scale = new Vector2(-1, 1);
        }

        flagEffect = false;
        _isFinishMotionOver = true;
        flagFinishMotion = true;

        _isActivate = false;
        flagActivate = false;

        GD.Print(this.Name);
    }

    public void Initialize()
    {
        _isFinishMotionOver = false;
        flagFinishMotion = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckEffect();
        CheckActivate();
    }

    private void CheckActivate()
    {
        if (_isActivate)
            Activate();
        else
            Disable();
    }

    void CheckEffect()
    {
        if (_isActivate)
        {
            if (!flagEffect)
            {
                flagEffect = true;
                _AnimPlayer.CurrentAnimation = "effect";
            }
            else
            {
                if (!_AnimPlayer.IsPlaying())
                    _isActivate = false;
            }
        }
        else
        {
            if (flagEffect)
            {
                flagEffect = false;
                if (_hasFinishMotion)
                    _AnimPlayer.CurrentAnimation = "finish";
                else
                    _AnimPlayer.CurrentAnimation = "RESET";
            }
        }
    }

    public void Activate()
    {
        if (!flagActivate)
        {
            flagActivate = true;

            _isActivate = true;
            Initialize();
        }
    }

    public void Disable()
    {
        if (flagActivate)
        {
            flagActivate = false;

            _isActivate = false;
        }
    }
}
