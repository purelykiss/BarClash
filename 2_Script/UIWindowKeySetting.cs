using Godot;
using System;

public partial class UIWindowKeySetting : ColorRect, IWindow
{
    [Export] string _id = "keySetting";
    UIPauseMenu _UIPauseMenu;
    bool _isActive;
    //[Export] Button _KeySettingButton;

    public string ID => _id;

    public override void _Ready()
    {
        _UIPauseMenu = GetParent<UIPauseMenu>();

        //if (!_KeySettingButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.KeySettingPressed)))
        //    _KeySettingButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.KeySettingPressed));

        _isActive = true;   //SetActive(false)를 올바르게 작동시키기 위함. 최종 기대 결과는 false
        SetActive(false);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isActive)
            return;

        bool flagEventUsed = false;

        if (@event.IsActionPressed("ui_cancel"))
        {
            _UIPauseMenu.ChangeCurWindow("option");
            flagEventUsed = true;
        }

        if (flagEventUsed)
            AcceptEvent();
    }

    public void SetActive(bool flag)
    {
        if (flag)
        {
            if (!_isActive)
            {
                _isActive = true;
                Visible = true;
                SetContentsActive(!_UIPauseMenu.IsPopupActive);
                SetProcessInput(true);
            }
        }
        else
        {
            if (_isActive)
            {
                _isActive = false;
                Visible = false;
                DisableFeature();
                SetProcessInput(false);
            }
        }
    }

    public void SetContentsActive(bool flag)
    {
        if (flag && _isActive)
        {
            ActivateFeature();
        }
        else
        {
            DisableFeature();
        }
    }

    public void ActivateFeature()
    {
        //_KeySettingButton.Disabled = false;
    }

    public void DisableFeature()
    {
        //_KeySettingButton.Disabled = true;
    }

    //public void KeySettingPressed()
    //{
    //    _UIPauseMenu.ChangeCurWindow("keySetting");
    //}
}
