using Godot;
using System;

public partial class UIScreenBlind : Control
{
    [Export] ColorRect _blindColorRect;


	public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _blindColorRect.CustomMinimumSize = GetViewportRect().Size;
        _blindColorRect.Visible = true;
        Visible = false;
        if (!SignalManager.instance.IsConnected("PauseBlind", new Callable(this, MethodName.SetBlind)))
            SignalManager.instance.Connect("PauseBlind", new Callable(this, MethodName.SetBlind));
    }

    public void SetBlind(bool flagIsPaused)
    {
        if (flagIsPaused)
            _blindColorRect.Color = new Color("000000", 0.5f);
        Visible = flagIsPaused;
        GD.Print("Pause Blind");
    }
}
