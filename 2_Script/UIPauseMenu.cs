using Godot;
using System;
using System.Collections.Generic;

public partial class UIPauseMenu : Control
{
    bool _isEnabled;
    List<IWindow> _windows = new List<IWindow>();
    IWindow _CurrentWindow;
    List<IPopup> _popups = new List<IPopup>();
    IPopup _CurrentPopup;
    bool _isPopupActive;

    public IWindow CurrentWindow => _CurrentWindow;

    public IPopup CurrentPopup => _CurrentPopup;

    public bool IsPopupActive => _isPopupActive;


    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = true;
        foreach(var item in GetChildren())
        {
            if(item is IWindow)
                _windows.Add((IWindow)item);
            else if (item is IPopup)
                _popups.Add((IPopup)item);
        }
        _CurrentWindow = _windows[0];
        _CurrentPopup = null;
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
        }

        if (flagEventUsed)
            AcceptEvent();
    }

    public void SetActive(bool flag)
    {
        if(flag)
        {
            if(!_isEnabled)
            {
                _isEnabled = true;
                SignalManager.instance.EmitSignal("PauseBlind", true);
                GetTree().Paused = true;
                _CurrentWindow.SetActive(true);
            }
        }
        else
        {
            if(_isEnabled)
            {
                _isEnabled = false;
                SignalManager.instance.EmitSignal("PauseBlind", false);
                GetTree().Paused = false;
                _CurrentWindow.SetActive(false);
                OnFinish();
            }
        }
    }

    public void ChangeCurWindow(string id)
    {
        foreach (var item in _windows)
        {
            if (item.ID == id)
            {
                _CurrentWindow.SetActive(false);
                _CurrentWindow = item;
                _CurrentWindow.SetActive(true);
                break;
            }
        }
    }

    public void ActivatePopup(string id)
    {
        foreach (var item in _popups)
        {
            if (item.ID == id)
            {
                _CurrentPopup = item;   //순서중요
                _CurrentPopup.SetActive(true);
                _isPopupActive = true;
                _CurrentWindow.SetContentsActive(false);
                break;
            }
        }
    }

    public void DisablePopup(string id)
    {
        if (_CurrentPopup == null)
            return;
        if(_CurrentPopup.ID == id)
        {
            _CurrentPopup.SetActive(false);
            _isPopupActive = false;
            _CurrentPopup = null;   //순서중요
            _CurrentWindow.SetContentsActive(true);
        }
    }

    void OnFinish()
    {
        _CurrentWindow.SetActive(false);
        _CurrentWindow = _windows[0];
        if(_CurrentPopup != null)
        {
            _CurrentPopup.SetActive(false);
            _CurrentPopup = null;
        }
    }
}
