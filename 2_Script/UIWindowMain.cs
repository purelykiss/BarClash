using Godot;
using System;

public partial class UIWindowMain : ColorRect, IWindow
{
    [Export] string _id = "main";
    UIPauseMenu _UIPauseMenu;
    bool _isActive;
    [Export] Button _GoMainButton;
    [Export] Button _OptionButton;
    [Export] Button _ContinueButton;

    public string ID => _id;

	public override void _Ready()
	{
        _UIPauseMenu = GetParent<UIPauseMenu>();

        _isActive = true;   //SetActive(false)를 올바르게 작동시키기 위함. 최종 기대 결과는 false
        SetActive(false);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isActive)
            return;

        bool flagEventUsed = false;

        if (@event.IsActionPressed("ui_select")|| @event.IsActionPressed("ui_cancel"))
        {
            _UIPauseMenu.SetActive(false);
            flagEventUsed = true;
        }

        if (flagEventUsed)
            AcceptEvent();

        if (!_GoMainButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.GoMainButtonPressed)))
            _GoMainButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.GoMainButtonPressed));
        if (!_OptionButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.OptionButtonPressed)))
            _OptionButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.OptionButtonPressed));
        if (!_ContinueButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.ContinueButtonPressed)))
            _ContinueButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.ContinueButtonPressed));
    }

    public void SetActive(bool flag)
    {
        if(flag)
        {
            if(!_isActive)
            {
                _isActive = true;
                Visible = true;
                SetContentsActive(!_UIPauseMenu.IsPopupActive);
                SetProcessInput(true);
            }
        }
        else
        {
            if(_isActive)
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
        if(flag && _isActive)
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
        _GoMainButton.Disabled = false;
        _OptionButton.Disabled = false;
        _ContinueButton.Disabled = false;
    }

    public void DisableFeature()
    {
        _GoMainButton.Disabled = true;
        _OptionButton.Disabled = true;
        _ContinueButton.Disabled = true;
    }

    public void GoMainButtonPressed()
    {
        _UIPauseMenu.ActivatePopup("goBack");
    }

    public void OptionButtonPressed()
    {
        _UIPauseMenu.ChangeCurWindow("option");
    }

    public void ContinueButtonPressed()
    {
        _UIPauseMenu.SetActive(false);
    }
}
