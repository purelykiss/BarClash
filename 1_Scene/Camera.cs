using Godot;
using System;

//이벤트연결 어떻게할지
public partial class Camera : Camera2D
{
	[Signal] delegate void CameraPosEventHandler(Vector2 pos);
	Vector2 Focus;

	public override void _Ready()
	{

		Focus = Vector2.Zero;
	}

    public override void _PhysicsProcess(double delta)
    {
		FollowFocus();
    }

    public override void _Process(double delta)
	{
		EmitSignal(SignalName.CameraPos, Position);
	}

	void FollowFocus()
	{
		Position = Focus;
    }

	public void ReceiveFocus(Vector2 pos)
	{
		Focus = pos;
	}
}
