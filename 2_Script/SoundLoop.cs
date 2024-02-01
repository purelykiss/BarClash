using Godot;
using System;

public partial class SoundLoop : AudioStreamPlayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (!IsConnected(SignalName.Finished, new Callable(this, MethodName.Play)))
            Connect(SignalName.Finished, new Callable(this, MethodName.Play));
    }
}
