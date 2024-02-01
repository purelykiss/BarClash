using Godot;
using System;
using System.Collections.Generic;

public partial class SoundList : Node2D
{
    IUnitMove _baseMove;
    List<ISoundEffect> _SoundEffects = new List<ISoundEffect>();

    public IUnitMove BaseMove => _baseMove;

    public override void _Ready()
    {
        _baseMove = GetParent<IUnitMove>();
        foreach (var item in GetChildren())
        {
            if (item is ISoundEffect)
            {
                _SoundEffects.Add(item as ISoundEffect);
            }
        }
    }

    public ISoundEffect GetSound(string id)
    {
        foreach (var item in _SoundEffects)
        {
            if (item.ID == id)
            {
                return item;
            }
        }
        return null;
    }

    public void ActivateSound(string id)
    {
        foreach (var item in _SoundEffects)
        {
            if (item.ID == id)
            {
                ISoundEffect SE = (ISoundEffect)item;
                SE.Activate();
                return;
            }
        }
    }

    public void DisableSound(string id)
    {
        foreach (var item in _SoundEffects)
        {
            if (item.ID == id)
            {
                ISoundEffect SE = (ISoundEffect)item;
                SE.Disable();
                return;
            }
        }
    }

    public void SetDestination(string id, Vector2 point)
    {
        foreach (var item in _SoundEffects)
        {
            if (item.ID == id && item is IProjectile)
            {
                IProjectile AtkBox = (IProjectile)item;
                AtkBox.SetDestination(point);
                return;
            }
        }
    }
}
