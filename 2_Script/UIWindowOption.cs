using Godot;
using System;

public partial class UIWindowOption : ColorRect, IWindow
{
    [Export] string _id = "option";
    UIPauseMenu _UIPauseMenu;
    bool _isActive;
    [Export] HSlider _MasterSlider;
    int MasterBusID;
    [Export] HSlider _BGMSlider;
    int BGMBusID;
    [Export] HSlider _SESlider;
    int SEBusID;
    [Export] Button _KeySettingButton;

    public string ID => _id;

    public override void _Ready()
    {
        _UIPauseMenu = GetParent<UIPauseMenu>();

        if (!_MasterSlider.IsConnected(HSlider.SignalName.ValueChanged, new Callable(this, MethodName.MasterSliderValueChanged)))
            _MasterSlider.Connect(HSlider.SignalName.ValueChanged, new Callable(this, MethodName.MasterSliderValueChanged));
        MasterBusID = AudioServer.GetBusIndex("Master");
        if (!_BGMSlider.IsConnected(HSlider.SignalName.ValueChanged, new Callable(this, MethodName.BGMSliderValueChanged)))
            _BGMSlider.Connect(HSlider.SignalName.ValueChanged, new Callable(this, MethodName.BGMSliderValueChanged));
        BGMBusID = AudioServer.GetBusIndex("BGM");
        if (!_SESlider.IsConnected(HSlider.SignalName.ValueChanged, new Callable(this, MethodName.SESliderValueChanged)))
            _SESlider.Connect(HSlider.SignalName.ValueChanged, new Callable(this, MethodName.SESliderValueChanged));
        SEBusID = AudioServer.GetBusIndex("SE");
        if (!_KeySettingButton.IsConnected(Button.SignalName.Pressed, new Callable(this, MethodName.KeySettingPressed)))
            _KeySettingButton.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.KeySettingPressed));

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
            _UIPauseMenu.ChangeCurWindow("main");
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
        _MasterSlider.Editable = true;
        _BGMSlider.Editable = true;
        _SESlider.Editable = true;
        _KeySettingButton.Disabled = false;
    }

    public void DisableFeature()
    {
        _MasterSlider.Editable = false;
        _BGMSlider.Editable = false;
        _SESlider.Editable = false;
        _KeySettingButton.Disabled = true;
    }

    public void MasterSliderValueChanged(float value)
    {
        var tmp = Mathf.Log(value) * 15f;
        if (value > 0)
            AudioServer.SetBusVolumeDb(MasterBusID, Mathf.Log(value) * 15f);
        AudioServer.SetBusMute(MasterBusID, value < _MasterSlider.Step);
    }

    public void BGMSliderValueChanged(float value)
    {
        var tmp = Mathf.Log(value) * 15f;
        if (value > 0)
            AudioServer.SetBusVolumeDb(BGMBusID, Mathf.Log(value) * 15f);
        AudioServer.SetBusMute(BGMBusID, value < _MasterSlider.Step);
    }

    public void SESliderValueChanged(float value)
    {
        var tmp = Mathf.Log(value) * 15f;
        if (value > 0)
            AudioServer.SetBusVolumeDb(SEBusID, Mathf.Log(value) * 15f);
        AudioServer.SetBusMute(SEBusID, value < _MasterSlider.Step);
    }

    public void KeySettingPressed()
    {
        _UIPauseMenu.ChangeCurWindow("keySetting");
    }
}
