using Godot;
using System;

public partial class afterimageEffect : Node2D, IEffect
{
    IUnitMove _baseMove => _EffectList.BaseMove;
    EffectList _EffectList;
    [Export] string _id;
    [Export] bool _isActivate;
    bool flagActivate;
    bool flagEffect;
    [Export] Sprite2D _baseSprite;
    [Export] string _singleAfterImagePath;
    PackedScene _singleAfterImage;
    [Export] float _emitImageTime;
    float _curEmitImageTime;

    public string ID => _id;

    public bool IsActivate { get => _isActivate; set => _isActivate = value; }

    public override void _Ready()
    {
        _EffectList = GetParent<EffectList>();

        flagEffect = false;

        _isActivate = false;
        flagActivate = false;

        _curEmitImageTime = 0;

        _singleAfterImage = GD.Load<PackedScene>(_singleAfterImagePath);

        GD.Print(this.Name);
    }

    void Initialize()
    {
        _curEmitImageTime = 0;
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
            }
            _curEmitImageTime -= (float)GetPhysicsProcessDeltaTime();
            if(_curEmitImageTime <= 0)
            {
                _curEmitImageTime = _emitImageTime;
                var instance = _singleAfterImage.Instantiate();
                AddChild(instance);
                singleAfterimage sai = (singleAfterimage)instance;
                sai.Texture = _baseSprite.Texture;
                sai.Offset = _baseSprite.Offset;
                sai.Scale = _baseSprite.Scale;
                sai.Hframes = _baseSprite.Hframes;
                sai.Vframes = _baseSprite.Vframes;
                sai.Frame = _baseSprite.Frame;
                sai.GlobalPosition = GlobalPosition;
                sai.Activate();
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
