using Godot;
using System;

public partial class singleAfterimage : Sprite2D
{
    [Export] float duration = 0.5f;
    [Export] Color afterimageColor = new Color(0.5f, 1, 1, 0.5f);
    [Export] bool isUsingShader = false;
    [Export] bool isOneShot = true;


    public override void _Ready()
	{
        Visible = false;
    }

    public void Activate()
    {
        Tween tween = GetTree().CreateTween();
        if(isUsingShader)
            tween.TweenProperty(Material, "shader_parameter/color", new Vector4(afterimageColor.R, afterimageColor.G, afterimageColor.B, 0), duration).From(new Vector4(afterimageColor.R, afterimageColor.G, afterimageColor.B, afterimageColor.A));
        else
            tween.TweenProperty(this, "modulate", new Color(afterimageColor, 0), duration).From(afterimageColor);
        if (isOneShot)
            tween.TweenCallback(Callable.From(this.QueueFree));
        Visible = true;
    }
}
