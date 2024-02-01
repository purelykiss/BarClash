using Godot;
using System;

public partial class oneAfterimageEffect : Node2D, IEffect
{
    IUnitMove _baseMove => _EffectList.BaseMove;
    EffectList _EffectList;
    [Export] string _id;
    [Export] bool _isActivate;
    bool flagActivate;
    bool flagEffect;
    [Export] Sprite2D _baseSprite;
    [Export] string _singleAfterImagePath;
    [Export] singleAfterimage _singleAfterImage;

    public string ID => _id;

    public bool IsActivate { get => _isActivate; set => _isActivate = value; }

    public override void _Ready()
    {
        _EffectList = GetParent<EffectList>();

        flagEffect = false;

        _isActivate = false;
        flagActivate = false;

        GD.Print(this.Name);
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

                _singleAfterImage.Texture = _baseSprite.Texture;
                _singleAfterImage.Offset = _baseSprite.Offset;
                _singleAfterImage.Scale = _baseSprite.Scale;
                _singleAfterImage.Hframes = _baseSprite.Hframes;
                _singleAfterImage.Vframes = _baseSprite.Vframes;
                _singleAfterImage.Frame = _baseSprite.Frame;
                _singleAfterImage.GlobalPosition = GlobalPosition;
                _singleAfterImage.Activate();
            }
        }
        else
        {
            if (flagEffect)
            {
                flagEffect = false;
            }
        }
    }

    public void Activate()
    {
        if (!flagActivate)
        {
            flagActivate = true;
            _isActivate = true;
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
