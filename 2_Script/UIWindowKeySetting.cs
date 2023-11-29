using Godot;
using System;
using System.Collections.Generic;

public partial class UIWindowKeySetting : ColorRect, IWindow
{
    [Export] string _id = "keySetting";
    UIPauseMenu _UIPauseMenu;
    bool _isActive;
    [Export] Button[] _KeySettingButton;
    List<KeySettingButton> _KeySettingButtonList = new List<KeySettingButton>();
    Dictionary<string, string> ActionAndKey = new Dictionary<string, string>();    //키보드 >> key_ 마우스 >> mus_
    Button CurrentButton = null;
    bool flagChanging;

    public string ID => _id;

    public override void _Ready()   //InputMap.ActionGetEvents("ui_select"), InputEventKey, InputEventJoypadButton, InputMap.ActionAddEvent("",)
    {
        _UIPauseMenu = GetParent<UIPauseMenu>();

        foreach(var item in _KeySettingButton)
        {
            if (item is KeySettingButton)
                _KeySettingButtonList.Add((KeySettingButton)item);
        }

        foreach(var item in _KeySettingButtonList)
        {
            string ActionID = item.ActionID;
            string ActionKey = "";
            foreach(var itemX in InputMap.ActionGetEvents(ActionID))
            {
                if (itemX is InputEventKey)
                {
                    InputEventKey tmpkey = (InputEventKey)itemX;
                    ActionKey = "key_";
                    string tmpStr = OS.GetKeycodeString(tmpkey.PhysicalKeycode);
                    if (tmpStr == "")
                        tmpStr = OS.GetKeycodeString(tmpkey.Keycode);
                    ActionKey += tmpStr;
                }
                else if (itemX is InputEventMouseButton)
                {
                    InputEventMouseButton tmpkey = (InputEventMouseButton)itemX;
                    ActionKey = "mus_";
                    ActionKey += tmpkey.ButtonIndex.ToString();
                }
            }
            ActionAndKey.Add(ActionID, ActionKey);

            if (!item.IsConnected(KeySettingButton.SignalName.KeySettingButtonPressed, new Callable(this, MethodName.OnButtonPressed)))
                item.Connect(KeySettingButton.SignalName.KeySettingButtonPressed, new Callable(this, MethodName.OnButtonPressed));
        }
        CurrentButton = null;
        flagChanging = false;
        CheckButtonText();

        _isActive = true;   //SetActive(false)를 올바르게 작동시키기 위함. 최종 기대 결과는 false
        SetActive(false);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isActive)
            return;

        if(flagChanging)
        {
            bool flagValidInput = false;
            if (@event is InputEventKey || @event is InputEventMouseButton)
                flagValidInput = true;

            if(flagValidInput)
            {
                flagChanging = false;
                ActivateFeature();
                CheckValidKey(@event);
                CheckButtonText();
                AcceptEvent();
                return;
            }
        }

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
        foreach(var item in _KeySettingButtonList)
        {
            item.Disabled = false;
        }
    }

    public void DisableFeature()
    {
        foreach (var item in _KeySettingButtonList)
        {
            item.Disabled = true;
        }
    }

    public void CheckButtonText()
    {
        foreach (var item in _KeySettingButtonList)
        {
            KeySettingButton tmpBtn = (KeySettingButton)item;
            if (tmpBtn == null)
                continue;
            item.Text = ActionAndKey[tmpBtn.ActionID];
        }
    }

    private void CheckValidKey(InputEvent @event)
    {
        //keySetting_cancel이면 캔슬
        if (@event.IsActionPressed("keySetting_cancel"))
            return;

        string curKey = "";
        //키마인지 체크, 아니면 캔슬
        if (@event is InputEventKey)
        {
            InputEventKey tmp = (InputEventKey)@event;
            curKey = "key_";
            string tmpStr = OS.GetKeycodeString(tmp.PhysicalKeycode);
            if (tmpStr == "")
                tmpStr = OS.GetKeycodeString(tmp.Keycode);
            curKey += tmpStr;
        }
        else if (@event is InputEventMouseButton)
        {
            InputEventMouseButton tmp = (InputEventMouseButton)@event;
            curKey = "mus_";
            curKey += tmp.ButtonIndex.ToString();
        }
        else
            return;

        //키 겹치는지 마지막으로 체크 후 키 변경
        if (ActionAndKey.ContainsValue(curKey))
            return;
        else
        {
            KeySettingButton tmp = (KeySettingButton)CurrentButton;
            foreach (var item in InputMap.ActionGetEvents(tmp.ActionID))
            {
                if (item is InputEventKey || item is InputEventMouseButton)
                {
                    InputMap.ActionEraseEvent(tmp.ActionID, item);
                    break;
                }
            }
            InputMap.ActionAddEvent(tmp.ActionID, @event);
            ActionAndKey[tmp.ActionID] = curKey;
        }
    }

    public void OnButtonPressed(Button button)
    {
            flagChanging = true;
            DisableFeature();
            CurrentButton = button;
            CurrentButton.Text = "Changing";
    }
}
