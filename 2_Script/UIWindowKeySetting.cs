using Godot;
using System;

public partial class UIWindowKeySetting : ColorRect, IWindow
{
    [Export] string _id = "keySetting";
    UIPauseMenu _UIPauseMenu;
    bool _isActive;
    [ExportGroup("Game")]
    [Export] Button _PlayerUpButton;
    [Export] Button _PlayerDownButton;
    [Export] Button _PlayerLeftButton;
    [Export] Button _PlayerRightButton;
    [Export] Button _PlayerAttackButton;
    [Export] Button _PlayerJumpButton;
    [Export] Button _PlayerDodgeButton;
    [ExportGroup("Skill")]
    [Export] Button _PlayerSkillAButton;
    [Export] Button _PlayerSkillBButton;
    [Export] Button _PlayerSkillCButton;
    [Export] Button _PlayerSkillDButton;
    [ExportGroup("Menu")]
    [Export] Button _UIUpButton;
    [Export] Button _UIDownButton;
    [Export] Button _UILeftButton;
    [Export] Button _UIRightButton;
    [Export] Button _UISelectButton;
    [Export] Button _UIAcceptButton;
    [Export] Button _UICancelButton;

    public string ID => _id;

    public override void _Ready()   //InputMap.ActionGetEvents("ui_select"), InputEventKey, InputEventJoypadButton, InputMap.ActionAddEvent("",)
    {
        _UIPauseMenu = GetParent<UIPauseMenu>();

        if (!_PlayerUpButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.KeySettingPressed)))
            _PlayerUpButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.KeySettingPressed));
        var tmp = InputMap.ActionGetEvents("ui_select");
        InputEvent tmp2 = tmp[0];
        var tmp3 = (InputEventJoypadButton)tmp2;

        var tmpA = InputMap.ActionGetEvents("player_attack");
        InputEvent tmpB = tmpA[0];
        var tmpC = (InputEventKey)tmpB;
        tmpC.Set("PhysicalKeycode", 0L);    //작동안함, _input으로 키를 받으면 피지컬이 아닌 Keycode도 받아서 한영키에 영향을 받을 수 있다.
        

        _isActive = true;   //SetActive(false)를 올바르게 작동시키기 위함. 최종 기대 결과는 false
        SetActive(false);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            InputMap.ActionEraseEvents("player_attack");
            InputMap.ActionAddEvent("player_attack", @event);
            _UIPauseMenu.ChangeCurWindow("option");
        }

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
        _PlayerUpButton.Disabled = false;
    }

    public void DisableFeature()
    {
        _PlayerUpButton.Disabled = true;
    }

    public void KeySettingPressed()
    {
        _UIPauseMenu.ChangeCurWindow("keySetting");
    }
}
