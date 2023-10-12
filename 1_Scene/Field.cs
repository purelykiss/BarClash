using Godot;
using System;

public partial class Field : Node2D
{
	Sprite2D BackGround;
	Sprite2D BackGround3;

	Vector2 CameraPos;

	public override void _Ready()
	{
		CameraPos = new Vector2();

        foreach (var item in GetChildren())
		{
			if (item is Sprite2D)
			{
				if (item.Name == "BackGround")
				{
					BackGround = (Sprite2D)item;
					break;
				}
				else if (item.Name == "BackGround3")
				{
					BackGround3 = (Sprite2D)item;
					break;
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		FollowCamera();
	}

	void FollowCamera()
	{
		BackGround.Position = CameraPos;
		BackGround3.Position = new Vector2(CameraPos.X, BackGround3.Position.Y);
	}

	public void ReceiveCameraPos(Vector2 pos)
	{
		CameraPos = pos;
	}
}
