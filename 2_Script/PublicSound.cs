using Godot;
using System;

public partial class PublicSound : AudioStreamPlayer2D, ISoundEffect
{
    [Export] string _id;
    [Export] bool _isActivate;

    public string ID => _id;
    public bool IsActivate { get => _isActivate; set => _isActivate = value; }


    public override void _Ready()
    {
        if(!IsConnected(SignalName.Finished, new Callable(this, MethodName.Disable)))
            Connect(SignalName.Finished, new Callable(this, MethodName.Disable));

        _isActivate = false;
    }

    public void Activate()
    {
        _isActivate = true;
        Play();
    }

    public void Disable()
    {
        _isActivate = false;
        Playing = false;
    }
}
