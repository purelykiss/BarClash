using Godot;
using System;

public partial class DebugLabel : Label
{
    [Export] PlayerMove _playerMove;

    public override void _PhysicsProcess(double delta)
    {
        Text = "CurrentState: " + _playerMove.FSM.CurrentState.ID
            + "\nPreVelocity: " + _playerMove.PreVelocity.ToString();
    }
}
