using Godot;
using System;

public partial class KeySettingButton : Button
{
    [Export] public string ActionID;
    [Signal] public delegate void KeySettingButtonPressedEventHandler(Button button);

    public override void _Ready()
    {
        if (!IsConnected(SignalName.Pressed, new Callable(this, MethodName.OnButtonPressed)))
            Connect(SignalName.Pressed, new Callable(this, MethodName.OnButtonPressed));
    }

    void OnButtonPressed()
    {
        Button tmp = this;
        EmitSignal(SignalName.KeySettingButtonPressed, tmp);
    }
}
