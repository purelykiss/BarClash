using Godot;
using System;

public partial class PlayerCameraFocus : Node2D
{
	public override void _Ready()
	{
    }

	public override void _Process(double delta)
	{
        SignalManager.instance.EmitSignal("Focus", GlobalPosition);
    }
}
