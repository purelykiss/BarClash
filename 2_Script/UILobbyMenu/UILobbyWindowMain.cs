using Godot;
using System;

public partial class UILobbyWindowMain : Control, IWindow
{
    [Export] string _id = "main";
    UILobbyMenu _UILobbyMenu;
    bool _isActive;
    [Export] Button _StartButton;
    [Export] Button _ManualButton;
    [Export] Button _OptionButton;
    [Export] Button _QuitButton;
    [Export] string GameScenePath;

    public string ID => _id;

    public override void _Ready()
    {
        _UILobbyMenu = GetParent<UILobbyMenu>();

        if (!_StartButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.StartButtonPressed)))
            _StartButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.StartButtonPressed));
        if (!_ManualButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.ManualButtonPressed)))
            _ManualButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.ManualButtonPressed));
        if (!_OptionButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.OptionButtonPressed)))
            _OptionButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.OptionButtonPressed));
        if (!_QuitButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.QuitButtonPressed)))
            _QuitButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.QuitButtonPressed));

        _isActive = false;   //SetActive(false)를 올바르게 작동시키기 위함. 최종 기대 결과는 false
        SetActive(true);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isActive)
            return;

        bool flagEventUsed = false;

        if (@event.IsActionPressed("ui_select") || @event.IsActionPressed("ui_cancel"))
        {
            _UILobbyMenu.SetActive(false);
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
                SetContentsActive(!_UILobbyMenu.IsPopupActive);
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
        _StartButton.Disabled = false;
        _OptionButton.Disabled = false;
        _QuitButton.Disabled = false;
    }

    public void DisableFeature()
    {
        _StartButton.Disabled = true;
        _OptionButton.Disabled = true;
        _QuitButton.Disabled = true;
    }

    public void StartButtonPressed()
    {
        GetTree().ChangeSceneToFile(GameScenePath);
    }

    public void ManualButtonPressed()
    {
        _UILobbyMenu.ChangeCurWindow("manual");
    }

    public void OptionButtonPressed()
    {
        _UILobbyMenu.ChangeCurWindow("option");
    }

    public void QuitButtonPressed()
    {
        GetTree().Quit();
    }
}
