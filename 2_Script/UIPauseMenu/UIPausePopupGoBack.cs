using Godot;
using System;

public partial class UIPausePopupGoBack : ColorRect, IPopup
{
    [Export] string _id = "goBack";
    UIPauseMenu _UIPauseMenu;
    bool _isActive;
    [Export] Button _ConfirmButton;
    [Export] Button _CancelButton;
    [Export] string LobbyScenePath;

    public string ID => _id;

    public override void _Ready()
    {
        _UIPauseMenu = GetParent<UIPauseMenu>();

        if (!_ConfirmButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.ConfirmButtonPressed)))
            _ConfirmButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.ConfirmButtonPressed));
        if (!_CancelButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.CancelButtonPressed)))
            _CancelButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.CancelButtonPressed));

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
            _UIPauseMenu.DisablePopup(_id);
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
                ActivateFeature();
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
        _ConfirmButton.Disabled = false;
        _CancelButton.Disabled = false;
    }

    public void DisableFeature()
    {
        _ConfirmButton.Disabled = true;
        _CancelButton.Disabled = true;
    }

    public void ConfirmButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile(LobbyScenePath);
    }

    public void CancelButtonPressed()
    {
        _UIPauseMenu.DisablePopup(_id);
    }
}
