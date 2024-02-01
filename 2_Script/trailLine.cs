using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class trailLine : Line2D
{
    trailEffect _trailEffect;
    [Export] int _maxLength;
    float curCooltime;
    bool flagFinish;
    bool _isActivate;
    List<Vector2> _trailPoints = new List<Vector2>();

    public override void _Ready()
	{
        _trailEffect = GetParent<trailEffect>();
        GlobalPosition = Vector2.Zero;
        flagFinish = false;
        _isActivate = false;
        SetPhysicsProcess(false);
    }

    void Initialize()
    {
        _trailPoints.Clear();
        ClearPoints();
        SetPhysicsProcess(false);
        flagFinish = false;
        _isActivate = false;
    }


    public override void _PhysicsProcess(double delta)
	{
        CheckPoints();
	}

    void CheckPoints()
    {
        if (!flagFinish)
        {
            _trailPoints.Add(_trailEffect.GlobalPosition);
            if (_trailPoints.Count > _maxLength + 1)
                _trailPoints.RemoveAt(_trailPoints.Count - 1);
            ClearPoints();
            for(int i = _trailPoints.Count - 1; i >= 0; i--)
            {
                AddPoint(_trailPoints[i]);
            }
        }
        else
        {
            if (Points.Length > 0)
                RemovePoint(Points.Length - 1);
            else
                Initialize();
        }
    }

    public void Activate()
    {
        if (_isActivate && flagFinish)
            Initialize();

        SetPhysicsProcess(true);
        _isActivate = true;
        flagFinish = false;
    }

    public void Disable()
    {
        flagFinish = true;
    }
}
