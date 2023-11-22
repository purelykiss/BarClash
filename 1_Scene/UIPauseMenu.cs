using Godot;
using System;
using System.Collections.Generic;

public partial class UIPauseMenu : Control
{
    bool _isEnabled;
    List<IWindow> _windows = new List<IWindow>();

	public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = true;
        foreach(var item in GetChildren())
        {
            if(item is IWindow)
                _windows.Add((IWindow)item);
        }
    }

	public override void _Process(double delta)
    {
        if (!_isEnabled)
            return;
    }

    public override void _Input(InputEvent @event)
    {
        bool flagEventUsed = false;

        if(@event.IsActionPressed("ui_select"))
        {
            if(!_isEnabled)
            {
                SetActive(true);
                flagEventUsed = true;
            }
            else
            {
                SetActive(false);
                flagEventUsed = true;
            }
        }

        if (flagEventUsed)
            AcceptEvent();
    }

    void SetActive(bool flag)
    {
        if(!flag)
        {
            if(!_isEnabled)
            {
                _isEnabled = true;
                SignalManager.instance.EmitSignal("PauseBlind", true);
                GetTree().Paused = true;
            }
        }
        else
        {
            if(_isEnabled)
            {
                _isEnabled = false;
                SignalManager.instance.EmitSignal("PauseBlind", false);
                GetTree().Paused = false;
            }
        }
    }

    public void ChangeCurWindow(string id)
    {

    }
}
