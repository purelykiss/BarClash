using Godot;
using System;

//이벤트연결 어떻게할지
public partial class Camera : Camera2D
{
	[Signal] public delegate void CameraPosEventHandler(Vector2 pos);
	Vector2 Focus;

	/// <summary>
	/// Camera2D의 Position은 Limit에 구애받지 않는다.<br/>
	/// 화면의 중앙 좌표를 쉽게 구하기 위해 Limit값을 이용해 Position을 조정해야 한다.
	/// </summary>
	float leftLimit, rightLimit, topLimit, bottomLimit;


    public override void _Ready()
    {
        leftLimit = LimitLeft + (Offset.X * Zoom.X) + GetViewportRect().Size.X / Zoom.X / 2;
        rightLimit = LimitRight - (Offset.X * Zoom.X) - GetViewportRect().Size.X / Zoom.X / 2;
        topLimit = LimitTop + (Offset.Y * Zoom.Y) + GetViewportRect().Size.Y / Zoom.Y / 2;
        bottomLimit = LimitBottom - (Offset.Y * Zoom.Y) - GetViewportRect().Size.Y / Zoom.Y / 2;

        Focus = Vector2.Zero;
		SignalManager.instance.Connect("Focus", new Callable(this, MethodName.ReceiveFocus));
	}

    public override void _PhysicsProcess(double delta)
    {
		FollowFocus();
        SignalManager.instance.EmitSignal("BGPos", GlobalPosition);
    }

	void FollowFocus()
	{
		Position = Focus;
		Position = new Vector2(Mathf.Clamp(Position.X, leftLimit, rightLimit), Mathf.Clamp(Position.Y, topLimit, bottomLimit));
    }

	public void ReceiveFocus(Vector2 globalPos)
	{
		Focus = globalPos;
	}
}
