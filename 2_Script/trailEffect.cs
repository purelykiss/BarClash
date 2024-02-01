using Godot;
using System;

public partial class trailEffect : Node2D, IEffect
{
    IUnitMove _baseMove => _EffectList.BaseMove;
    EffectList _EffectList;
    [Export] string _id;
    [Export] bool _isActivate;
    bool flagActivate;
    bool flagEffect;
    [Export] trailLine _trailLine;

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

    void Initialize()
    {

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
                _trailLine.Activate();
            }
        }
        else
        {
            if (flagEffect)
            {
                flagEffect = false;
                _trailLine.Disable();
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
