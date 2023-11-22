using Godot;
using System;

public partial class TestArea : Area2D
{
    public override void _Ready()
    {
        if (!IsConnected(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered)))
            Connect(SignalName.BodyEntered, new Callable(this, MethodName.OnBodyEntered));
    }

    public void OnBodyEntered(Node2D body)
    {
        GD.Print("TestArea!");
        if(body is IUnitMove)
        {
            GD.Print("Unit Entered!");
        }
    }
}
